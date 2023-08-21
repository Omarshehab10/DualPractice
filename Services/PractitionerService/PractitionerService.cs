using AutoMapper;
using Common.Enums;
using Common.Localization;
using Common.Types;
using DAL.Models;
using DTO.DTOs;
using DTO.DTOs.EmailSetting;
using DTO.DTOs.EstablishmentHLS;
using DTO.DTOs.Parameters;
using DTO.DTOs.PractitionersRegistry;
using DTO.DTOs.Responses;
using DTO.Parameters.PractitionerInfo;
using Lean.Framework.Core;
using Lean.Framework.Entities.Integration;
using Lean.Framework.Entities.Provider;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Services.Common;
using Services.ExternalServices.Anat;
using Services.ExternalServices.HLS;
using Services.ExternalServices.OrganizationRegistry;
using Services.ExternalServices.PractitionersRegistry;
using Services.ExternalServices.SehaEndPoint;
using Services.ExternalServices.SendEmailService;
using Services.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MedicalOrganizationSubCategory = DAL.Models.MedicalOrganizationSubCategory;
namespace Services.PractitionerService
{
    public class PractitionerService : IPractitionerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ILogger<PractitionerService> _logger;
        private readonly IAppResourceService _resourceService;
        private readonly IPractitionersRegistryService _pracRegistryService;
        private readonly IEstablishmentHLS _establishmentHLS;
        private readonly IEmailService _emailService;
        private readonly ICommonService _commonService;
        private readonly ISehaIntegrationService _sehaIntegrationService;
        private readonly IAnatService _anatService;
        private readonly IOrganizationRegistryService _organizationRegistryService;
        private readonly ISehaService _sehaService;

        public PractitionerService(
            IServiceProvider serviceProvider, ISehaService sehaService,
            IUnitOfWork unitOfWork, IMapper mapper,
            IConfiguration configuration, IAppResourceService resourceService,
            ILogger<PractitionerService> logger, IPractitionersRegistryService pracRegistryService,
            IEstablishmentHLS establishmentHLS,
            IEmailService emailService,
            ICommonService commonService ,IAnatService anatService, 
            IOrganizationRegistryService organizationRegistryService)
        {
            _unitOfWork = unitOfWork;
            _sehaService = sehaService;
            _mapper = mapper;
            _configuration = configuration;
            _logger = logger;
            _resourceService = resourceService;
            _pracRegistryService = pracRegistryService;
            _establishmentHLS = establishmentHLS;
            _emailService = emailService;
            _commonService = commonService;
            _sehaIntegrationService = serviceProvider.GetRequiredService<ISehaIntegrationService>();
            _anatService = anatService;
            _organizationRegistryService = organizationRegistryService;
        }


        public async Task<ResultOfAction<initialDualPracticeResponse>> GetPractitionerInfoCheck(PractitionerInfoRequest practitionerInfoRequest)
        {
            var userData = new UserTokenData();
            try
            {
                userData = _sehaIntegrationService.GetUserData();
            }
            catch (Exception e)
            {
                _logger.LogError("Exception Message : " + e.Message);
                throw new CustomHttpException(HttpStatusCode.BadRequest, _resourceService.GetResource("Seha_User_Not_Found"));
            }

            var practitionerData = await _pracRegistryService.GetPractitionerData(practitionerInfoRequest.NationalID);

            ValidatePractitionerData(practitionerData, practitionerInfoRequest);

            if (practitionerData.hls_licenses.Count !=0)
            {   
                var IsHlsLicenseValid = !CheckHlsLicense(practitionerData.hls_licenses);
                if (IsHlsLicenseValid)
                {
                    _logger.LogError("********* GetPractitionerInfoCheck -  CheckHlsLicense ******** Practitoiner Has Valid Hls License ");
                    throw new CustomHttpException(HttpStatusCode.BadRequest, _resourceService.GetResource("Practitioner_Has_Valid_Hls_License"));
                }
            }


            #region CompleteOrganizationData


            var Organization = await _unitOfWork.Repository<Organization>().Get(x => x.Id == userData.RequestInfoId)
                .Include(x => x.Subcategory)
                 .Include(x => x.ManagmentArea)
                .FirstOrDefaultAsync();

            if (Organization == null)
            {
                _logger.LogError("Organization is NULL");
                throw new CustomHttpException(HttpStatusCode.BadRequest, _resourceService.GetResource("Organization_Not_Found"));
            }

            // TODO:  remove GetOrganizationData
            var orgData = new OrganizationDataResponse();
            if (Organization.LicenseNumber == null || Organization.NameEn == null)
            {
                orgData = await _sehaService.GetOrganizationData(userData.AccessToken);
                Organization.LicenseNumber = orgData.Org.license_number;
                Organization.NameEn = orgData.Org.name_en;
            }
            var subcategory = Organization.Subcategory;
            var AdminArea = Organization.ManagmentArea;
            #endregion

            //var IsSkipHls = Convert.ToBoolean(_configuration["SkipHLS:value"]);

            var establishmentData = new PrivateestablishmentData();




            establishmentData = await GetPriInfo(Organization.OrganizationId.ToString());

            if (Organization.LicenseNumber == null)
            {
                Organization.LicenseNumber = establishmentData.license_number;
            }
            if (Organization.LicenseNumber == null)
            {
                _logger.LogError($"Organization LicenseNumber is Null");
                throw new CustomHttpException(HttpStatusCode.BadRequest, _resourceService.GetResource("Organization_LicenseNumber_Null"));
            }
            ValidateEstablishmentData(establishmentData);



            #region practitionerInfo
            var specialtyCode = "-1";
            if (int.TryParse(practitionerData.scfhs.specialty_code, out int value))
                specialtyCode = practitionerData?.scfhs.specialty_code;

            DateTime? scfhsRegistrationExpiryDate = new DateTime();

            if (practitionerData.scfhs.license_expiry_date != null)
            {
                scfhsRegistrationExpiryDate = Convert.ToDateTime(practitionerData.scfhs.license_expiry_date);
            }

            var practitionerInfo = _mapper.Map<practitionerInfo>(practitionerData);

            practitionerInfo.nationalId = practitionerInfoRequest.NationalID;

            if (practitionerData.nationality_code == "SAU")
            {
                practitionerInfo.nationalityAr = EnumHelper.GetEnumDescription(Nationalitys.Saudi);
                practitionerInfo.nationalityEn = Nationalitys.Saudi.ToString();
            }

            if (CheckExpiryDate(practitionerData.scfhs.license_expiry_date))
            {
                practitionerInfo.scfhsRegistrationStatus = "valid";
            }

            practitionerInfo.scfhsRegistrationExpiryDate = scfhsRegistrationExpiryDate;
            #endregion

            #region private Establishment Info
            var privateEstablishmentInfo = new privateEstablishmentInfo
            {
                nameEn = Organization.NameEn,

                establishmentTypeAr = subcategory.NameAr,
                establishmentTypeEn = subcategory.NameEn,
                cityAr = Organization.CityNameAr,
                cityEn = Organization.CityNameEn,
                cityId = Organization.CityId,
                regionAr = Organization.RegionNameAr,
                regionEn = Organization.RegionNameEn,
                regionId = Organization.RegionId,
                administrationNameAr = AdminArea.NameAr,
                administrationNameEn = AdminArea.NameEn,
                organizationId = Organization.OrganizationId,
                privateUserId = userData.UserId,
                licenseExpiryDate = Convert.ToDateTime(establishmentData.license_expiry_date, CultureInfo.GetCultureInfo("en-GB")),
                nameAr = establishmentData.name_ar, 
                licenseNumber = establishmentData.license_number
            };
            #endregion

            if (practitionerData.gov_organizations.FirstOrDefault()?.employer_org_id == null)
            {
                _logger.LogError($"Establishment Org Id is Null");
                throw new CustomHttpException(HttpStatusCode.BadRequest, _resourceService.GetResource("Establishment_Org_Id_Null"));
            }

            var govEstablishmentInfo = await GetGovInfo(practitionerData.gov_organizations.FirstOrDefault().employer_org_id);

            var response = new initialDualPracticeResponse()
            {
                practitionerInfo = practitionerInfo,
                privateEstablishmentInfo = privateEstablishmentInfo,
                govEstablishmentInfo = govEstablishmentInfo
            };

            var NonMohOrganizationInfo = _unitOfWork.Repository<NonMohOrganizationLookup>().Get(x => x.Orgid == govEstablishmentInfo.sehaOrganizationId).FirstOrDefault();

            var SehaIdNonMoh = int.Parse(_configuration["NationalGuardHealthAffairs:SehaId"]);
            if (NonMohOrganizationInfo != null)
            {
                govEstablishmentInfo.approvalLevel = (int)ApprovalLevels.TwoApprovals;
                govEstablishmentInfo.sehaOrganizationIdLevel2 = SehaIdNonMoh;
            }

            _unitOfWork.Repository<Organization>().Update(Organization);
            await _unitOfWork.SaveChangesAsync();

            return new ResultOfAction<initialDualPracticeResponse>((int)HttpStatusCode.OK, null, response);
        }

        public async Task<ResultOfAction<AddNewDualPractitionerResponse>> AddNewDualPractitionerRequest(AddNewDualPractitionerRequest addNewDualPractitionerRequest)
        {
            AddNewDualPractitionerResponse addNewDualPractitionerResponse = new AddNewDualPractitionerResponse();
            var userId = addNewDualPractitionerRequest.PrivateEstablishmentInfo.UserId;


            var UserInfo = await _unitOfWork.Repository<User>().Get(x => x.Id == userId).FirstOrDefaultAsync();
            var IsArabic = _commonService.IsArabicLangHeader();

            if (UserInfo == null)
            {
                _logger.LogError($"AddNewDualPractitionerRequest -  UserId Not found");
                return new ResultOfAction<AddNewDualPractitionerResponse>((int)HttpStatusCode.NoContent, _resourceService.GetResource("Seha_User_Not_Found"), addNewDualPractitionerResponse);
            }

            var OrgRequestInfo = await _unitOfWork.Repository<Organization>().Get(x => x.OrganizationId == addNewDualPractitionerRequest.PrivateEstablishmentInfo.OrganizationId).FirstOrDefaultAsync();
            var MainOrgId = await _unitOfWork.Repository<Organization>().Get(x => x.OrganizationId == addNewDualPractitionerRequest.GovEstablishmentInfo.OrganizationId).FirstOrDefaultAsync();

            if (OrgRequestInfo == null)
            {
                _logger.LogError($"AddNewDualPractitionerRequest - OrgRequestInfo Not found ");
                return new ResultOfAction<AddNewDualPractitionerResponse>((int)HttpStatusCode.NoContent, _resourceService.GetResource("Organization_Not_Found"), addNewDualPractitionerResponse);
            }
            else
            {
                OrgRequestInfo.LicenseExpiryDate = addNewDualPractitionerRequest.PrivateEstablishmentInfo.LicenseExpiryDate;
                _unitOfWork.Repository<Organization>().Update(OrgRequestInfo);
                await _unitOfWork.SaveChangesAsync();

            }

            if (MainOrgId == null)
            {
                //int SubcategoryId = int.Parse(_configuration["GovOrgSubCategory:GovOrgSubCategoryId"]);
                var NewMainOrg = new Organization
                {
                    OrganizationId = addNewDualPractitionerRequest.GovEstablishmentInfo.OrganizationId,
                    NameAr = addNewDualPractitionerRequest.GovEstablishmentInfo.NameAr,
                    NameEn = addNewDualPractitionerRequest.GovEstablishmentInfo.NameEn,
                    OrganizationTypeId = (int)OrganizationType.GovernmentOrganization,
                    CityId = addNewDualPractitionerRequest.GovEstablishmentInfo.CityId,
                    RegionId = addNewDualPractitionerRequest.GovEstablishmentInfo.RegionId,
                    ManagmentAreaId = addNewDualPractitionerRequest.GovEstablishmentInfo.ManagementAreaId,
                    SehaAdministrativeOrganizationId = addNewDualPractitionerRequest.GovEstablishmentInfo.SehaOrganizationIdLevel2,
                    SubcategoryId = 21
                };
                if (addNewDualPractitionerRequest.GovEstablishmentInfo.ClusterId != null)
                {
                    NewMainOrg.ClusterId = int.Parse(addNewDualPractitionerRequest.GovEstablishmentInfo.ClusterId);
                }
                await _unitOfWork.Repository<Organization>().InsertAsync(NewMainOrg);
                await _unitOfWork.SaveChangesAsync();
            }

            var NewPractitionerID = await _unitOfWork.Repository<Practioner>().Get(x => x.NationalId == addNewDualPractitionerRequest.PractitionerInfo.NationalId).FirstOrDefaultAsync();

            if (NewPractitionerID == null)
            {

                var DateOfBirthHijri = "";
                if (addNewDualPractitionerRequest.PractitionerInfo.DateOfBirthH != null)
                {
                    DateOfBirthHijri = addNewDualPractitionerRequest.PractitionerInfo.DateOfBirthH.Substring(0, 10);

                }
                var NewPractitioner = new Practioner
                {
                    Nationality = (int)Nationalitys.Saudi,
                    NationalId = addNewDualPractitionerRequest.PractitionerInfo.NationalId,
                    FullNameAr = addNewDualPractitionerRequest.PractitionerInfo.FullNameAr,
                    FullNameEn = addNewDualPractitionerRequest.PractitionerInfo.FullNameEn,
                    LicenseNumber = addNewDualPractitionerRequest.PractitionerInfo.LicenseNumber,
                    LicenseExpiryDate = addNewDualPractitionerRequest.PractitionerInfo.LicenseExpiryDate,
                    SpecialtyCode = addNewDualPractitionerRequest.PractitionerInfo.SpecialtyCode,
                    SpecialtyNameAr = addNewDualPractitionerRequest.PractitionerInfo.SpecialtyNameAr,
                    SpecialtyNameEn = addNewDualPractitionerRequest.PractitionerInfo.SpecialtyNameEn,
                    CreateDate = _commonService.GetCurrentDateTime(),
                    UpdateDate = _commonService.GetCurrentDateTime(),
                    CreatedBy = userId,
                    UpdatedBy = userId,
                    Gender = addNewDualPractitionerRequest.PractitionerInfo.Gender,
                    ScfhsRegistrationNumber = addNewDualPractitionerRequest.PractitionerInfo.ScfhsRegistrationNumber,
                    ScfhsRegistrationExpiryDate = addNewDualPractitionerRequest.PractitionerInfo.ScfhsRegistrationExpiryDate,
                    DateOfBirthH = DateOfBirthHijri,
                    DateOfBirthG = addNewDualPractitionerRequest.PractitionerInfo.DateOfBirthG,
                    ScfhsCategoryAr = addNewDualPractitionerRequest.PractitionerInfo.ScfhsCategoryAr,
                    ScfhsCategoryEn = addNewDualPractitionerRequest.PractitionerInfo.ScfhsCategoryEn,
                };
                string[] NamesAr = SpiltFullName(addNewDualPractitionerRequest.PractitionerInfo.FullNameAr);
                try
                {
                    NewPractitioner.FirstNameAr = NamesAr[0];
                    NewPractitioner.SecondNameAr = NamesAr[1];
                    NewPractitioner.ThirdNameAr = NamesAr[2];
                    NewPractitioner.LastNameAr = NamesAr[3];
                }
                catch (Exception)
                {
                }
                string[] NamesEn = SpiltFullName(addNewDualPractitionerRequest.PractitionerInfo.FullNameEn);

                try
                {
                    NewPractitioner.FirstNameEn = NamesEn[0];
                    NewPractitioner.SecondNameEn = NamesEn[1];
                    NewPractitioner.ThirdNameEn = NamesEn[2];
                    NewPractitioner.LastNameEn = NamesEn[3];
                }
                catch (Exception)
                {
                }
                _unitOfWork.Repository<Practioner>().Insert(NewPractitioner);
                _unitOfWork.SaveChanges();

            }

            var RequestList = await _unitOfWork.Repository<DpRequest>().Get(x => x.PractionerIdNumber == addNewDualPractitionerRequest.PractitionerInfo.NationalId).ToListAsync();

            var RequestListSameOrg = await _unitOfWork.Repository<DpRequest>().Get(x => x.PractionerIdNumber == addNewDualPractitionerRequest.PractitionerInfo.NationalId
            && x.RequestingOrgId == OrgRequestInfo.Id).ToListAsync();



            var RequestListOtherOrg = await _unitOfWork.Repository<DpRequest>().Get(x => x.PractionerIdNumber == addNewDualPractitionerRequest.PractitionerInfo.NationalId
            && x.RequestingOrgId != OrgRequestInfo.Id).ToListAsync();



            bool WaitingOrNew = RequestListSameOrg.AsQueryable().Any(x =>
            x.RequestStatus == (int)RequestStatus.New ||
            x.RequestStatus == (int)RequestStatus.Waiting_Pu_Accept1 ||
            x.RequestStatus == (int)RequestStatus.Waiting_Pu_Accept2 ||
            x.RequestStatus == (int)RequestStatus.Waiting_Pr_Pay_2 ||
            x.RequestStatus == (int)RequestStatus.Renewed
            );
            if (WaitingOrNew)
            {
                _logger.LogError($"AddNewDualPractitionerRequest - the facility has a previous request for the same practitioner ");

                return new ResultOfAction<AddNewDualPractitionerResponse>((int)HttpStatusCode.BadRequest, _resourceService.GetResource("Open_Request_Practitioner_Same_Organization"), null );

            }

            bool RequestApprovedSameOrg = RequestListSameOrg.AsQueryable().Any(x =>
            x.RequestStatus == (int)RequestStatus.Approved
            );
            if (RequestApprovedSameOrg)
            {

                _logger.LogError($"AddNewDualPractitionerRequest - This practitioner is already added to your facilities ");

                return new ResultOfAction<AddNewDualPractitionerResponse>((int)HttpStatusCode.BadRequest, _resourceService.GetResource("Practitioner_Already_Added"), null);

            }
            bool RequestApprovedLeveloneOtherOrg = RequestListOtherOrg.AsQueryable().Any(x =>
            x.RequestStatus == (int)RequestStatus.Waiting_Pu_Accept2 || x.RequestStatus == (int)RequestStatus.Waiting_Pr_Pay_2

            );
            if (RequestApprovedLeveloneOtherOrg)
            {
                _logger.LogError($"AddNewDualPractitionerRequest - the practitioner has an existing request ");

                return new ResultOfAction<AddNewDualPractitionerResponse>((int)HttpStatusCode.BadRequest, _resourceService.GetResource("Practitioner_Has_Existing_Request"), null);
            }
            bool RequestApproved = RequestListOtherOrg.AsQueryable().Any(x =>
            x.RequestStatus == (int)RequestStatus.Approved 

            );
            if (RequestApproved)
            {
                _logger.LogError($"AddNewDualPractitionerRequest - the practitioner has valid Approved ");

                return new ResultOfAction<AddNewDualPractitionerResponse>((int)HttpStatusCode.BadRequest, _resourceService.GetResource("Practitioner_Already_Approved"), null);
            }



            if (addNewDualPractitionerRequest.DayScheduleInfo.Count != 0)
            {
                foreach (var item in addNewDualPractitionerRequest.DayScheduleInfo)
                {
                    if (item.TotalHours > 8 )
                    {
                        _logger.LogError($"AddNewDualPractitionerRequest - The number of hours per day should not exceed 8 hours  Day : " + item.Day + " TotalHours : " + item.TotalHours);

                        return new ResultOfAction<AddNewDualPractitionerResponse>((int)HttpStatusCode.BadRequest, _resourceService.GetResource("Max_Total_Hours_Per_Day"), null);
                    }

                }

                var Practitionerid = await _unitOfWork.Repository<Practioner>().Get(x => x.NationalId == addNewDualPractitionerRequest.PractitionerInfo.NationalId).FirstOrDefaultAsync();
                if (MainOrgId == null)
                {
                    MainOrgId = await _unitOfWork.Repository<Organization>().Get(x => x.OrganizationId == addNewDualPractitionerRequest.GovEstablishmentInfo.OrganizationId).FirstOrDefaultAsync();
                }
                
                var NewDualPractitionerRequest = new DpRequest
                {
                    RequestStatus = (int)RequestStatus.New,
                    Duration = addNewDualPractitionerRequest.DPRequestInfo.Duration,
                    CreateDate = _commonService.GetCurrentDateTime(),
                    CreatedBy = userId,
                    ApprovalLevel = addNewDualPractitionerRequest.GovEstablishmentInfo.ApprovalLevel,
                    TotalWeeklyHours = addNewDualPractitionerRequest.DPRequestInfo.TotalWeeklyHours,

                    RequestingOrgId = OrgRequestInfo.Id,
                    PractionerMainOrgId = MainOrgId.Id,
                    PractionerIdNumber = Practitionerid.NationalId,
                    MohLevel2SehaOrgId = addNewDualPractitionerRequest.GovEstablishmentInfo.SehaOrganizationIdLevel2
                };
                var Code = new ServiceCode
                {
                    Code = _commonService.GetNextCode(),
                    Date = _commonService.GetCurrentDateTime(),
                    Prefix = "DUP"
                };
                NewDualPractitionerRequest.ServiceCodeCode = Code.Code;
                NewDualPractitionerRequest.ServiceCodeDate = Code.Date;
                NewDualPractitionerRequest.ServiceCodePrefix = Code.Prefix;
                NewDualPractitionerRequest.NormalizedServiceCode = await Format(Code);


                _unitOfWork.Repository<DpRequest>().Insert(NewDualPractitionerRequest);
                _unitOfWork.SaveChanges();

                var DpRequestInfoid = await _unitOfWork.Repository<DpRequest>().Get(x => x.NormalizedServiceCode == NewDualPractitionerRequest.NormalizedServiceCode).Include(x => x.PractionerIdNumberNavigation).FirstOrDefaultAsync();
                addNewDualPractitionerResponse.ReqServiceCode = DpRequestInfoid.NormalizedServiceCode;

                foreach (var DayScheduleInfo in addNewDualPractitionerRequest.DayScheduleInfo)
                {
                    DaySchedule NewDayScheduleInfo = new DaySchedule
                    {
                        DprequestId = DpRequestInfoid.Id,
                        Day = DayScheduleInfo.Day,
                        From = Convert.ToDateTime(DayScheduleInfo.From),
                        To = Convert.ToDateTime(DayScheduleInfo.To),
                        TotalHours = DayScheduleInfo.TotalHours
                    };
                    await _unitOfWork.Repository<DaySchedule>().InsertAsync(NewDayScheduleInfo);
                }
                await _unitOfWork.SaveChangesAsync();

                // Send Anat Notification
                var PractionerName = DpRequestInfoid.PractionerIdNumberNavigation.FullNameEn;
                var SendNotificationTitle = "New Request";
                var SendNotificationBody = $" Dear {PractionerName}. {Environment.NewLine} You have a request from Khabeer Alseha, pending your approval. </div>";
                var SMSTxtBody = $" Dear {PractionerName}.{Environment.NewLine} You have a request from Khabeer Alseha, pending your approval. {Environment.NewLine} You can view the details of the request and take the necessary action through Anat application link: https://anat.page.link/job-marketplace";

                if (IsArabic)
                {
                    PractionerName = DpRequestInfoid.PractionerIdNumberNavigation.FullNameAr;
                    SendNotificationTitle = "طلب جديد";
                    SendNotificationBody = $" عزيزى {PractionerName}. {Environment.NewLine} يوجد لديك طلب عمل في القطاع الخاص خارج أوقات العمل الرسمية بانتظار موافقتك. ";
                    SMSTxtBody = $" عزيزى {PractionerName}. {Environment.NewLine} يوجد لديك طلب عمل في القطاع الخاص خارج أوقات العمل الرسمية بانتظار موافقتك. {Environment.NewLine} بإمكانك الاطلاع على تفاصيل الطلب واتخاذ الاجراء اللازم عن طريق تطبيق أناة على الرابط: https://anat.page.link/job-marketplace";
                }

                var NewRequestNotification = new NotificationToAnatRequest
                {
                    ServiceName = _resourceService.GetResource("ServiceName"),
                    Title = SendNotificationTitle,
                    Body = SendNotificationBody,
                    NumberId = new List<string> { DpRequestInfoid.PractionerIdNumberNavigation.NationalId },
                    requestInfo = new RequestInfo
                    {
                        Type = "dual_practice",
                        DualPractice = new DualPractice
                        {
                            RequestId = DpRequestInfoid.NormalizedServiceCode
                        }
                    },
                    SendSms = new SMS
                    {
                        IsSent = true,
                        SmsBody = SMSTxtBody
                    }
                };
                try
                {

                    var result = await _anatService.SendNotificationToAnat(NewRequestNotification);
                }
                catch (Exception e)
                {
                    _logger.LogError($"Exception Message : {e}");
                    _logger.LogError($"SendNotificationToAnat - Failed to send Notification To Anat NumberId :" + NewRequestNotification.NumberId + " RequestId : " + NewRequestNotification.requestInfo.DualPractice.RequestId);
                    return new ResultOfAction<AddNewDualPractitionerResponse>((int)HttpStatusCode.OK, _resourceService.GetResource("Request_Sent"), addNewDualPractitionerResponse);
                }


                return new ResultOfAction<AddNewDualPractitionerResponse>((int)HttpStatusCode.OK, _resourceService.GetResource("Request_Sent"), addNewDualPractitionerResponse);
            }

            _logger.LogError($"AddNewDualPractitionerRequest - Failed to add request");
            addNewDualPractitionerResponse.Success = false;
            return new ResultOfAction<AddNewDualPractitionerResponse>((int)HttpStatusCode.BadRequest, _resourceService.GetResource("Add_Request_Failed"), addNewDualPractitionerResponse);

        }

        public async Task<DP_Request> GetDualPracticeReport(string serviceCode)
        {
            try
            {
                var DpRequest = await _unitOfWork.Repository<DpRequest>().Get(
                x => x.NormalizedServiceCode == serviceCode)
                .Include(x => x.PractionerIdNumberNavigation)
                .Include(x => x.PractionerMainOrg)
                .Include(x => x.RequestingOrg)
                .Include(x => x.PractionerMainOrg.Subcategory)
                .Include(x => x.RequestingOrg.Subcategory)
                .Include(x => x.DaySchedule)
                .FirstOrDefaultAsync();

                if (DpRequest == null)
                {
                    _logger.LogError("Dual Practice Request Not Found");
                    throw new CustomHttpException(HttpStatusCode.BadRequest, _resourceService.GetResource("DUP_Report_Not_Found"));
                }

                var model = new DP_Request
                {
                    fullNameAr = DpRequest.PractionerIdNumberNavigation.FullNameAr,
                    fullNameEn = DpRequest.PractionerIdNumberNavigation.FullNameEn,
                    specialityAr = DpRequest.PractionerIdNumberNavigation.SpecialtyNameAr,
                    specialityEn = DpRequest.PractionerIdNumberNavigation.SpecialtyNameEn,
                    scfhsCategoryAr = DpRequest.PractionerIdNumberNavigation.ScfhsCategoryAr,
                    scfhsCategoryEn = DpRequest.PractionerIdNumberNavigation.ScfhsCategoryEn,

                    totalWeeklyHours = Convert.ToDecimal(DpRequest.TotalWeeklyHours),
                    createDate = Convert.ToDateTime(DpRequest.CreateDate),
                    serviceCode = DpRequest.NormalizedServiceCode,
                    durationMonths = DpRequest.Duration,
                    from = Convert.ToDateTime(DpRequest.FromDate),
                    to = Convert.ToDateTime(DpRequest.ToDate),

                    govOrgNameAr = DpRequest.PractionerMainOrg.NameAr,
                    govOrgNameEn = DpRequest.PractionerMainOrg.NameEn,
                    govOrgTypeNameAr = DpRequest.PractionerMainOrg.Subcategory.NameAr,
                    govOrgTypeNameEn = DpRequest.PractionerMainOrg.Subcategory.NameEn,

                    priOrgNameAr = DpRequest.RequestingOrg.NameAr,
                    priOrgNameEn = DpRequest.RequestingOrg.NameEn,
                    priOrgTypeNameAr = DpRequest.RequestingOrg.Subcategory.NameAr,
                    priOrgTypeNameEn = DpRequest.RequestingOrg.Subcategory.NameEn,
                    licenseNumber = DpRequest.RequestingOrg.LicenseNumber,

                    reportUrl = DpRequest.ApprovedReportUrl,

                    workDays = _mapper.Map<List<WorkDay>>(DpRequest.DaySchedule),

                    status = DpRequest.RequestStatus
                };
                return model;
            }
            catch (Exception e)
            {
                _logger.LogError($"Error while getting & mapping pdf model , Exception Message : {e.Message} ");
                throw new CustomHttpException(HttpStatusCode.BadRequest, _resourceService.GetResource("Error_Generate_Pdf"));
            }
            
        }

        public async Task<string> SaveDualPracticeUrl(string serviceCode, string QrUrl, int reportStatus)
        {
            var DpRequest = await _unitOfWork.Repository<DpRequest>().Get(
               x => x.NormalizedServiceCode == serviceCode)
               .FirstOrDefaultAsync();

            if (DpRequest == null)
            {
                _logger.LogError("Dual Practice Request Not Found");
                throw new CustomHttpException(HttpStatusCode.BadRequest, _resourceService.GetResource("DUP_Report_Not_Found"));
            }

            DpRequest.ApprovedReportUrl = QrUrl;
            _unitOfWork.Repository<DpRequest>().Update(DpRequest);
            await _unitOfWork.SaveChangesAsync();

            return QrUrl;
        }
        public async Task<ResultOfAction<DefaultModel>> GetPractitionerReviewFromAnat(GetRequestInfo getRequestInfo)
        {

            var user = _configuration["AnatUser:AnatUserId"];
            _logger.LogInformation("********* Get User Info From Seha = (" + user + ")", user);
            var RequestInfo = await _unitOfWork.Repository<DpRequest>().Get(x => x.NormalizedServiceCode == getRequestInfo.RequestServiceCode).Include(y => y.PractionerMainOrg).Include(y => y.RequestingOrg).Include(y => y.PractionerIdNumberNavigation).FirstOrDefaultAsync();

            AddLogRequest addLogRequest = new AddLogRequest();
            addLogRequest.NormalizedServiceCode = RequestInfo.NormalizedServiceCode;
            addLogRequest.UserId = int.Parse(user);


            if (RequestInfo == null)
            {
                _logger.LogError($"GetPractitionerReviewFromAnat - This Request Is Not Found");

                throw new CustomHttpException(HttpStatusCode.BadRequest, _resourceService.GetResource("This_Request_Is_Not_Found"));
            }

            if (RequestInfo.RequestStatus != (int)RequestStatus.New)
            {
                _logger.LogError($"GetPractitionerReviewFromAnat - This Request status is not waiting practitioner approval");

                throw new CustomHttpException(HttpStatusCode.BadRequest, _resourceService.GetResource("Request_Status_Is_Not_Correct"));
            }
            var checkIfHaseAccept1 = await _unitOfWork.Repository<DpRequest>().Get(x => x.PractionerIdNumber == RequestInfo.PractionerIdNumber)
            .Where(x => x.RequestStatus == (int)RequestStatus.Waiting_Pu_Accept2
             || x.RequestStatus == (int)RequestStatus.Waiting_Pr_Pay_2).ToListAsync();

            if (checkIfHaseAccept1.Count != 0)
            {
                _logger.LogError($"GetPractitionerReviewFromAnat - the practitioner has an existing request");

                throw new CustomHttpException(HttpStatusCode.BadRequest, _resourceService.GetResource("Practitioner_Has_Existing_Request"));
            }
            EmailContent emailContent = new EmailContent();
            UsersInfoRequest usersInfoRequest = new UsersInfoRequest();
            var RolePerId = _configuration["UserPermissionId:AddDPReq"];

            if (getRequestInfo.Approval == false)
            {
                RequestInfo.RequestStatus = (int)RequestStatus.Rejected_Prac1;
                RequestInfo.Comment.Add(new Comment
                {
                    Text = getRequestInfo.Comment,
                    DprequestId = RequestInfo.Id,
                    CreatedBy = int.Parse(user),
                    CreateDate = DateTime.Now
                });
                RequestInfo.UpdatedBy = int.Parse(user);
                RequestInfo.UpdateDate = DateTime.Now;
                _logger.LogInformation("********* update or Add Request Info  = (" + RequestInfo + ")", RequestInfo);
                _unitOfWork.Repository<DpRequest>().Update(RequestInfo);
                await _unitOfWork.SaveChangesAsync();

                //Add Action Log
                addLogRequest.ActionType = AddLogs.Practitioner_Reject_request;
                await _commonService.AddActionRequestLog(addLogRequest);

                // Sending Email updating Refuse to Priv Org

                _logger.LogInformation("********* Get Role Permission Of the User = (" + RolePerId + ")", RolePerId);

                usersInfoRequest.PermissionId = int.Parse(RolePerId);
                usersInfoRequest.OrganizationId = RequestInfo.RequestingOrg.OrganizationId;
                usersInfoRequest.ServiceId = 47;

                _logger.LogInformation("********* Get users Info Request Of the User to SendMail = (" + usersInfoRequest.OrganizationId + ")", usersInfoRequest.OrganizationId);

                emailContent.RequestId = RequestInfo.NormalizedServiceCode;
                emailContent.EmailSubjectOrg = _resourceService.GetResource("Email_Update_On_TheStatus_Of_request_Subj");
                emailContent.EmailBody = $"<div dir='rtl' lang='ar'> نفيدكم بأن الطلب {RequestInfo.NormalizedServiceCode} <br> المرسل للمارس {RequestInfo.PractionerIdNumberNavigation.FullNameAr}<br> قد تم رفضه من قبل الممارس<br> للاطلاع على أسباب الرفض الرجاء الضغط على الرابط أدناه<br> رابط الطلب: https://dualpractice.seha.sa/sehaaccount<br> للاستفسارات الرجاء التواصل مع فريق الدعم support@seha.sa </div>";
                emailContent.Contents = File.ReadAllText(_commonService.MapPath(@"StaticFiles/SendOrganizationNewRequest/SendOrganisationNewReqEmailHtmlBody.html"));

                _logger.LogInformation("********* Get Email Content Request Of the User to SendMail = (" + emailContent.EmailSubjectOrg + " **** " + emailContent.RequestId + ")", usersInfoRequest.OrganizationId);

                await _emailService.SendMailOrg(usersInfoRequest, emailContent);

                return new ResultOfAction<DefaultModel>((int)HttpStatusCode.OK, _resourceService.GetResource("Request_Rejected"), new DefaultModel());
            }
            RequestInfo.RequestStatus = (int)RequestStatus.Waiting_Pu_Accept1;
            RequestInfo.UpdatedBy = int.Parse(user);
            RequestInfo.UpdateDate = DateTime.Now;
            _unitOfWork.Repository<DpRequest>().Update(RequestInfo);
            await _unitOfWork.SaveChangesAsync();

            //Add Action Log
            addLogRequest.ActionType = AddLogs.Practitioner_Accept_request;
            await _commonService.AddActionRequestLog(addLogRequest);

            //Sending Email New Request To Gov Organinzation

            RolePerId = _configuration["UserPermissionId:GovOrgReviewDPReq"];
            _logger.LogInformation("********* Get Role Permission Of the User = (" + RolePerId + ")", RolePerId);

            usersInfoRequest.PermissionId = int.Parse(RolePerId);
            usersInfoRequest.OrganizationId = RequestInfo.PractionerMainOrg.OrganizationId;
            usersInfoRequest.ServiceId = 47;

            _logger.LogInformation("********* Get users Info Request Of the User to SendMail = (" + usersInfoRequest.OrganizationId + ")", usersInfoRequest.OrganizationId);

            emailContent.RequestId = RequestInfo.NormalizedServiceCode;
            emailContent.EmailSubjectOrg = _resourceService.GetResource("Email_Subject_New_Request");
            emailContent.EmailBody = $"<div dir='rtl' lang='ar'> يوجد لديك طلب جديد {RequestInfo.NormalizedServiceCode} <br> بانتظار مراجعتك فى نظام خبير الصحه. <br> رابط الخدمة: https://dualpractice.seha.sa/sehaaccount <br> للاستفسارات الرجاء التواصل مع فريق الدعم support@seha.sa </div>";
            emailContent.Contents = File.ReadAllText(_commonService.MapPath(@"StaticFiles/SendOrganizationNewRequest/SendOrganisationNewReqEmailHtmlBody.html"));

            _logger.LogInformation("********* Get Email Content Request Of the User to SendMail = (" + emailContent.EmailSubjectOrg + " **** " + emailContent.RequestId + ")", usersInfoRequest.OrganizationId);
            await _emailService.SendMailOrg(usersInfoRequest, emailContent);

            // Sending Email updating approve to Priv Org
            RolePerId = _configuration["UserPermissionId:AddDPReq"];

            _logger.LogInformation("********* Get Role Permission Of the User = (" + RolePerId + ")", RolePerId);

            usersInfoRequest.PermissionId = int.Parse(RolePerId);
            usersInfoRequest.OrganizationId = RequestInfo.RequestingOrg.OrganizationId;

            _logger.LogInformation("********* Get users Info Request Of the User to SendMail = (" + usersInfoRequest.OrganizationId + ")", usersInfoRequest.OrganizationId);

            emailContent.EmailSubjectOrg = _resourceService.GetResource("Email_Update_On_TheStatus_Of_request_Subj");
            emailContent.EmailBody = $"<div dir='rtl' lang='ar'> نفيدكم بأن الطلب {RequestInfo.NormalizedServiceCode} <br> المرسل للمارس {RequestInfo.PractionerIdNumberNavigation.FullNameAr}<br> قد تم قبوله من قبل الممارس <br> رابط الطلب: https://dualpractice.seha.sa/sehaaccount <br> للاستفسارات الرجاء التواصل مع فريق الدعم support@seha.sa </div>";
            emailContent.Contents = File.ReadAllText(_commonService.MapPath(@"StaticFiles/SendOrganizationNewRequest/SendOrganisationNewReqEmailHtmlBody.html"));

            _logger.LogInformation("********* Get Email Content Request Of the User to SendMail = (" + emailContent.EmailSubjectOrg + " **** " + emailContent.RequestId + ")", usersInfoRequest.OrganizationId);

            await _emailService.SendMailOrg(usersInfoRequest, emailContent);

            return new ResultOfAction<DefaultModel>((int)HttpStatusCode.OK, null, new DefaultModel());
        }


        public async Task<ResultOfAction<DefaultModel>> GetPractitionerReviewFromAnatV2(GetRequestInfoV2 getRequestInfo)
        {

            var user = _configuration["AnatUser:AnatUserId"];
            _logger.LogInformation("********* Get User Info From Seha = (" + user + ")", user);
            var RequestInfo = await _unitOfWork.Repository<DpRequest>().Get(x => x.NormalizedServiceCode == getRequestInfo.RequestServiceCode).Include(y=>y.PractionerMainOrg).Include(y => y.RequestingOrg).Include(y => y.PractionerIdNumberNavigation).FirstOrDefaultAsync();
            
            AddLogRequest addLogRequest = new AddLogRequest();
            addLogRequest.NormalizedServiceCode = RequestInfo.NormalizedServiceCode;
            addLogRequest.UserId = int.Parse(user);


            if (RequestInfo == null)
            {
                _logger.LogError($"GetPractitionerReviewFromAnat - This Request Is Not Found");

                throw new CustomHttpException(HttpStatusCode.BadRequest, _resourceService.GetResource("This_Request_Is_Not_Found"));
            }

            if (RequestInfo.RequestStatus != (int)RequestStatus.New)
            {
                _logger.LogError($"GetPractitionerReviewFromAnat - This Request status is not waiting practitioner approval");

                throw new CustomHttpException(HttpStatusCode.BadRequest, _resourceService.GetResource("Request_Status_Is_Not_Correct"));
            }
            var checkIfHaseAccept1 = await _unitOfWork.Repository<DpRequest>().Get(x => x.PractionerIdNumber == RequestInfo.PractionerIdNumber)
            .Where(x => x.RequestStatus == (int)RequestStatus.Waiting_Pu_Accept2
             || x.RequestStatus == (int)RequestStatus.Waiting_Pr_Pay_2).ToListAsync();

            if (checkIfHaseAccept1.Count != 0)
            {
                _logger.LogError($"GetPractitionerReviewFromAnat - the practitioner has an existing request");

                throw new CustomHttpException(HttpStatusCode.BadRequest, _resourceService.GetResource("Practitioner_Has_Existing_Request"));
            }
            EmailContent emailContent = new EmailContent();
            UsersInfoRequest usersInfoRequest = new UsersInfoRequest();
            var RolePerId = _configuration["UserPermissionId:AddDPReq"];

            if (getRequestInfo.Approval == false)
            {
                if (getRequestInfo.RejectionReasonId == null)
                {
                  throw new CustomHttpException(HttpStatusCode.BadRequest, _resourceService.GetResource("Rejection_Reason_Id_required"));
                }
                RequestInfo.RequestStatus = (int)RequestStatus.Rejected_Prac1;
                var RejectionReason = await _unitOfWork.Repository<RejectionReasonsLookup>().Get(x => x.Id == getRequestInfo.RejectionReasonId).FirstOrDefaultAsync();
                if (RejectionReason == null)
                {
                    throw new CustomHttpException(HttpStatusCode.BadRequest, _resourceService.GetResource("Wrong_Rejection_Reason_Id"));
                }
                if (getRequestInfo.RejectionReasonId != 1)
                {
                    getRequestInfo.Comment = RejectionReason.ReasonAr;
                }
                RequestInfo.Comment.Add(new Comment
                {
                    Text = getRequestInfo.Comment,
                    DprequestId = RequestInfo.Id,
                    CreatedBy = int.Parse(user),
                    RejectionReasonId = getRequestInfo.RejectionReasonId,
                    CreateDate = DateTime.Now
                });
                RequestInfo.UpdatedBy = int.Parse(user);
                RequestInfo.UpdateDate = DateTime.Now;
                _logger.LogInformation("********* update or Add Request Info  = (" + RequestInfo + ")", RequestInfo);
                _unitOfWork.Repository<DpRequest>().Update(RequestInfo);
                await _unitOfWork.SaveChangesAsync();

                //Add Action Log
                addLogRequest.ActionType = AddLogs.Practitioner_Reject_request;
                await _commonService.AddActionRequestLog(addLogRequest);

                // Sending Email updating Refuse to Priv Org

                _logger.LogInformation("********* Get Role Permission Of the User = (" + RolePerId + ")", RolePerId);

                usersInfoRequest.PermissionId = int.Parse(RolePerId);
                usersInfoRequest.OrganizationId = RequestInfo.RequestingOrg.OrganizationId;
                usersInfoRequest.ServiceId = 47;

                _logger.LogInformation("********* Get users Info Request Of the User to SendMail = (" + usersInfoRequest.OrganizationId + ")", usersInfoRequest.OrganizationId);

                emailContent.RequestId = RequestInfo.NormalizedServiceCode;
                emailContent.EmailSubjectOrg = _resourceService.GetResource("Email_Update_On_TheStatus_Of_request_Subj");
                emailContent.EmailBody = $"<div dir='rtl' lang='ar'> نفيدكم بأن الطلب {RequestInfo.NormalizedServiceCode} <br> المرسل للمارس {RequestInfo.PractionerIdNumberNavigation.FullNameAr}<br> قد تم رفضه من قبل الممارس<br> للاطلاع على أسباب الرفض الرجاء الضغط على الرابط أدناه<br> رابط الطلب: https://dualpractice.seha.sa/sehaaccount<br> للاستفسارات الرجاء التواصل مع فريق الدعم support@seha.sa </div>";
                emailContent.Contents = File.ReadAllText(_commonService.MapPath(@"StaticFiles/SendOrganizationNewRequest/SendOrganisationNewReqEmailHtmlBody.html"));

                _logger.LogInformation("********* Get Email Content Request Of the User to SendMail = (" + emailContent.EmailSubjectOrg + " **** " + emailContent.RequestId + ")", usersInfoRequest.OrganizationId);

                await _emailService.SendMailOrg(usersInfoRequest, emailContent);

                return new ResultOfAction<DefaultModel>((int)HttpStatusCode.OK, _resourceService.GetResource("Request_Rejected"), new DefaultModel());
            }
            RequestInfo.RequestStatus = (int)RequestStatus.Waiting_Pu_Accept1;
            RequestInfo.UpdatedBy = int.Parse(user);
            RequestInfo.UpdateDate = DateTime.Now;
            _unitOfWork.Repository<DpRequest>().Update(RequestInfo);
            await _unitOfWork.SaveChangesAsync();

            //Add Action Log
            addLogRequest.ActionType = AddLogs.Practitioner_Accept_request;
            await _commonService.AddActionRequestLog(addLogRequest);

            //Sending Email New Request To Gov Organinzation
            
            RolePerId = _configuration["UserPermissionId:GovOrgReviewDPReq"];
            _logger.LogInformation("********* Get Role Permission Of the User = (" + RolePerId + ")", RolePerId);
            
            usersInfoRequest.PermissionId = int.Parse(RolePerId);
            usersInfoRequest.OrganizationId = RequestInfo.PractionerMainOrg.OrganizationId;
            usersInfoRequest.ServiceId = 47;
            
            _logger.LogInformation("********* Get users Info Request Of the User to SendMail = (" + usersInfoRequest.OrganizationId + ")", usersInfoRequest.OrganizationId);

            emailContent.RequestId = RequestInfo.NormalizedServiceCode;
            emailContent.EmailSubjectOrg = _resourceService.GetResource("Email_Subject_New_Request");
            emailContent.EmailBody = $"<div dir='rtl' lang='ar'> يوجد لديك طلب جديد {RequestInfo.NormalizedServiceCode} <br> بانتظار مراجعتك فى نظام خبير الصحه. <br> رابط الخدمة: https://dualpractice.seha.sa/sehaaccount <br> للاستفسارات الرجاء التواصل مع فريق الدعم support@seha.sa </div>";
            emailContent.Contents = File.ReadAllText(_commonService.MapPath(@"StaticFiles/SendOrganizationNewRequest/SendOrganisationNewReqEmailHtmlBody.html"));
            
            _logger.LogInformation("********* Get Email Content Request Of the User to SendMail = (" + emailContent.EmailSubjectOrg +" **** "+ emailContent.RequestId+ ")", usersInfoRequest.OrganizationId);
            await _emailService.SendMailOrg(usersInfoRequest, emailContent);

            // Sending Email updating approve to Priv Org
            RolePerId = _configuration["UserPermissionId:AddDPReq"];
            
            _logger.LogInformation("********* Get Role Permission Of the User = (" + RolePerId + ")", RolePerId);
            
            usersInfoRequest.PermissionId = int.Parse(RolePerId);
            usersInfoRequest.OrganizationId = RequestInfo.RequestingOrg.OrganizationId;
           
            _logger.LogInformation("********* Get users Info Request Of the User to SendMail = (" + usersInfoRequest.OrganizationId + ")", usersInfoRequest.OrganizationId);
           
            emailContent.EmailSubjectOrg = _resourceService.GetResource("Email_Update_On_TheStatus_Of_request_Subj");
            emailContent.EmailBody = $"<div dir='rtl' lang='ar'> نفيدكم بأن الطلب {RequestInfo.NormalizedServiceCode} <br> المرسل للمارس {RequestInfo.PractionerIdNumberNavigation.FullNameAr}<br> قد تم قبوله من قبل الممارس <br> رابط الطلب: https://dualpractice.seha.sa/sehaaccount <br> للاستفسارات الرجاء التواصل مع فريق الدعم support@seha.sa </div>";
            emailContent.Contents = File.ReadAllText(_commonService.MapPath(@"StaticFiles/SendOrganizationNewRequest/SendOrganisationNewReqEmailHtmlBody.html"));
           
            _logger.LogInformation("********* Get Email Content Request Of the User to SendMail = (" + emailContent.EmailSubjectOrg + " **** " + emailContent.RequestId + ")", usersInfoRequest.OrganizationId);
            
            await _emailService.SendMailOrg(usersInfoRequest, emailContent);

            return new ResultOfAction<DefaultModel>((int)HttpStatusCode.OK, null, new DefaultModel());
        }

        
        public async Task<ResultOfAction<PractitionerAllRequestsResponse>> GetPractitionerAllRequests()
        {
            var practitionerNid = _commonService.GetNIdHeader(true);

            _logger.LogInformation($"***** GetPractitionerAllRequests - This Practitioner ID = {practitionerNid}");
            var PractitionerInfo = await _unitOfWork.Repository<Practioner>().Get(x => x.NationalId == practitionerNid).FirstOrDefaultAsync();
            var IsCultureArabic = _commonService.IsArabicLangHeader();
            _logger.LogInformation($"***** GetPractitionerAllRequests - This Practitioner IsCultureArabic = {IsCultureArabic}");

            if (PractitionerInfo == null)
            {
                _logger.LogError($"GetPractitionerAllRequests - This Practitioner Is Not Found");

                throw new CustomHttpException(HttpStatusCode.NotFound, _resourceService.GetResource("This_Practitioner_Is_Not_Found"));
            }
            var PractitionerRequestsList = await _unitOfWork.Repository<DpRequest>().Get(x => x.PractionerIdNumber == practitionerNid)
                .Include(y => y.PractionerIdNumberNavigation).Include(z => z.PractionerMainOrg).Include(t => t.RequestingOrg).Include(r => r.DaySchedule).Include(z => z.PractionerMainOrg.Subcategory).Include(t => t.RequestingOrg.Subcategory).OrderByDescending(o=>o.CreateDate).ToListAsync();
            if (PractitionerRequestsList.Count == 0)
            {
                _logger.LogError($"GetPractitionerAllRequests - This Practitioner requests Is Not Found");

                throw new CustomHttpException(HttpStatusCode.NotFound, _resourceService.GetResource("Practitioner_requests_Not_Found"));
            }
            PractitionerAllRequestsResponse practitionerAllRequestsResponse = new PractitionerAllRequestsResponse();          

            if (IsCultureArabic)
            {
                foreach (var item in PractitionerRequestsList)
                {
                    DoctorRequestsDto doctorRequestsDto = new DoctorRequestsDto
                    {
                        ApprovalDuration = item.Duration,
                        GovCityName = item.PractionerMainOrg.CityNameAr,
                        GovOrgName = item.PractionerMainOrg.NameAr,
                        PrvOrgName = item.RequestingOrg.NameAr,
                        PrivateCityName = item.RequestingOrg.CityNameAr,
                        PrvOrgCategoryName = item.RequestingOrg.Subcategory.NameAr,
                        GovOrgCategoryName = item.PractionerMainOrg.Subcategory.NameAr,
                        TotalWeeklyHours = item.TotalWeeklyHours,
                        PrvOrgLicenseNumber = item.RequestingOrg.LicenseNumber,
                        SpecialtyName = item.PractionerIdNumberNavigation.SpecialtyNameAr,
                        ApprovedReportUrl = item.ApprovedReportUrl,
                        Requestid = item.NormalizedServiceCode,
                        ReqStatus = item.RequestStatus,
                        DoctorDaySchedule = GetDoctorDaySchedule(item.DaySchedule)
                    };
                    practitionerAllRequestsResponse.request.Add(doctorRequestsDto);
                }
            }
            else
            {
                foreach (var item in PractitionerRequestsList)
                {
                    DoctorRequestsDto doctorRequestsDto = new DoctorRequestsDto
                    {
                        ApprovalDuration = item.Duration,
                        GovCityName = item.PractionerMainOrg.CityNameEn,
                        GovOrgName = item.PractionerMainOrg.NameEn,
                        PrvOrgName = item.RequestingOrg.NameEn,
                        PrivateCityName = item.RequestingOrg.CityNameEn,
                        PrvOrgCategoryName = item.RequestingOrg.Subcategory.NameEn,
                        GovOrgCategoryName = item.PractionerMainOrg.Subcategory.NameEn,
                        TotalWeeklyHours = item.TotalWeeklyHours,
                        PrvOrgLicenseNumber = item.RequestingOrg.LicenseNumber,
                        SpecialtyName = item.PractionerIdNumberNavigation.SpecialtyNameEn,
                        ApprovedReportUrl = item.ApprovedReportUrl,
                        Requestid = item.NormalizedServiceCode,
                        ReqStatus = item.RequestStatus,
                        DoctorDaySchedule = GetDoctorDaySchedule(item.DaySchedule)
                    };
                    practitionerAllRequestsResponse.request.Add(doctorRequestsDto);
                }
            }
            return new ResultOfAction<PractitionerAllRequestsResponse>((int)HttpStatusCode.OK, null, practitionerAllRequestsResponse);
        }

        public async Task<ResultOfAction<DetailsOfRequestsResponse>> GetDetailsOfRequest(RequestServiceCode requestServiceCode)
        {
            DetailsOfRequestsResponse response = new DetailsOfRequestsResponse();
            var RequestInfo = await _unitOfWork.Repository<DpRequest>().Get(x => x.NormalizedServiceCode == requestServiceCode.ReqServiceCode)
                .Include(x => x.PractionerIdNumberNavigation)
                .Include(y => y.RequestingOrg)
                .Include(y => y.RequestingOrg.ManagmentArea)
                .Include(z => z.PractionerMainOrg)
                .Include(x => x.DaySchedule)
                .Include(x => x.Comment)
                .Include(y => y.RequestingOrg.Subcategory)
                .Include(z => z.PractionerMainOrg.Subcategory)
                .FirstOrDefaultAsync();

            if (RequestInfo == null)
            {
                _logger.LogError("Dual Practice Request Not Found");
                throw new CustomHttpException(HttpStatusCode.BadRequest, _resourceService.GetResource("This_Request_Is_Not_Found"));
            }
            var IsCultureArabic = _commonService.IsArabicLangHeader();

            if (IsCultureArabic)
            {
                response = new DetailsOfRequestsResponse()
                {
                    PractitionerFullName = RequestInfo.PractionerIdNumberNavigation.FullNameAr,
                    SpecialtyName = RequestInfo.PractionerIdNumberNavigation.SpecialtyNameAr,
                    PractitionerLicenseNumber = RequestInfo.PractionerIdNumberNavigation.ScfhsRegistrationNumber,
                    ScfhsRegistrationExpiryDate = RequestInfo.PractionerIdNumberNavigation.ScfhsRegistrationExpiryDate,
                    Requestid = RequestInfo.NormalizedServiceCode,

                    PrvOrgName = RequestInfo.RequestingOrg.NameAr,
                    PrvOrgCategoryName = RequestInfo.RequestingOrg.Subcategory.NameAr,
                    PrivateCityName = RequestInfo.RequestingOrg.CityNameAr,
                    PrivateEstablishmentLicenseExpiryDate = RequestInfo.RequestingOrg.LicenseExpiryDate,
                    PrvOrgLicenseNumber = RequestInfo.RequestingOrg.LicenseNumber,
                    Directorate = RequestInfo.RequestingOrg.ManagmentArea?.NameAr,

                    GovOrgName = RequestInfo.PractionerMainOrg.NameAr,
                    GovCityName = RequestInfo.PractionerMainOrg.CityNameAr,
                    GovOrgCategoryName = RequestInfo.PractionerMainOrg.Subcategory.NameAr,

                    ApprovalStartDate = RequestInfo.FromDate,
                    ApprovalEndDate = RequestInfo.ToDate,
                    ReqStatus = RequestInfo.RequestStatus,
                    ApprovalDuration = RequestInfo.Duration,
                    TotalWeeklyHours = RequestInfo.TotalWeeklyHours,
                };
                if (RequestInfo.RequestStatus == (int)RequestStatus.Approved)
                {
                    response.ApprovedReportUrl = RequestInfo.ApprovedReportUrl;
                }
                if (RequestInfo.RequestStatus == (int)RequestStatus.Rejected_Prac1)
                {
                    response.RefusalSide = RequestInfo.PractionerIdNumberNavigation.FullNameAr;
                }
                else if (RequestInfo.RequestStatus == (int)RequestStatus.Cancelled_Pr1 || RequestInfo.RequestStatus == (int)RequestStatus.Rejected_Pr2)
                {
                    response.RefusalSide = RequestInfo.RequestingOrg.NameAr;
                }
                else if (RequestInfo.RequestStatus == (int)RequestStatus.Cancelled_Pu2 || RequestInfo.RequestStatus == (int)RequestStatus.Rejected_Pu3)
                {
                    response.RefusalSide = RequestInfo.PractionerMainOrg.NameAr;
                }
                else if (RequestInfo.RequestStatus == (int)RequestStatus.Rejected_Pu4)
                {
                    response.RefusalSide = RequestInfo.RequestingOrg.ManagmentArea.NameAr;
                }
            }
            else
            {
                response = new DetailsOfRequestsResponse()
                {
                    PractitionerFullName = RequestInfo.PractionerIdNumberNavigation.FullNameEn,
                    SpecialtyName = RequestInfo.PractionerIdNumberNavigation.SpecialtyNameEn,
                    PractitionerLicenseNumber = RequestInfo.PractionerIdNumberNavigation.ScfhsRegistrationNumber,
                    ScfhsRegistrationExpiryDate = RequestInfo.PractionerIdNumberNavigation.LicenseExpiryDate,
                    Requestid = RequestInfo.NormalizedServiceCode,

                    PrvOrgName = RequestInfo.RequestingOrg.NameEn,
                    PrvOrgCategoryName = RequestInfo.RequestingOrg.Subcategory.NameEn,
                    PrivateCityName = RequestInfo.RequestingOrg.CityNameEn,
                    PrivateEstablishmentLicenseExpiryDate = RequestInfo.RequestingOrg.LicenseExpiryDate,
                    PrvOrgLicenseNumber = RequestInfo.RequestingOrg.LicenseNumber,
                    Directorate = RequestInfo.RequestingOrg.ManagmentArea?.NameEn,

                    GovOrgName = RequestInfo.PractionerMainOrg.NameEn,
                    GovCityName = RequestInfo.PractionerMainOrg.CityNameEn,
                    GovOrgCategoryName = RequestInfo.PractionerMainOrg.Subcategory.NameEn,

                    ApprovalStartDate = RequestInfo.FromDate,
                    ApprovalEndDate = RequestInfo.ToDate,
                    ReqStatus = RequestInfo.RequestStatus,
                    ApprovalDuration = RequestInfo.Duration,
                    TotalWeeklyHours = RequestInfo.TotalWeeklyHours,

                };
                if (RequestInfo.RequestStatus == (int)RequestStatus.Rejected_Prac1)
                {
                    response.RefusalSide = RequestInfo.PractionerIdNumberNavigation.FullNameEn;
                }
                else if (RequestInfo.RequestStatus == (int)RequestStatus.Cancelled_Pr1 || RequestInfo.RequestStatus == (int)RequestStatus.Rejected_Pr2)
                {
                    response.RefusalSide = RequestInfo.RequestingOrg.NameEn;
                }
                else if (RequestInfo.RequestStatus == (int)RequestStatus.Cancelled_Pu2 || RequestInfo.RequestStatus == (int)RequestStatus.Rejected_Pu3)
                {
                    response.RefusalSide = RequestInfo.PractionerMainOrg.NameEn;
                }
                else if (RequestInfo.RequestStatus == (int)RequestStatus.Rejected_Pu4)
                {
                    response.RefusalSide = RequestInfo.RequestingOrg.ManagmentArea.NameEn;
                }
            }
            foreach (var item in RequestInfo.DaySchedule)
            {

                var DayScheduleInfo = new DoctorDailySchedule
                {
                    From = item.From,
                    To = item.To,
                    TotalHours = (int)item.TotalHours,
                    Day = item.Day
                };

                response.DoctorDaySchedule.Add(DayScheduleInfo);
            }

            if (RequestInfo.Comment.Count != 0)
            {
                foreach (var item in RequestInfo.Comment)
                {
                    response.RejectionDate = item?.CreateDate;
                    response.RrejectionReason = item?.Text;
                }
            }
            return new ResultOfAction<DetailsOfRequestsResponse>((int)HttpStatusCode.OK, null, response);
        }

        private List<DoctorDailySchedule> GetDoctorDaySchedule(ICollection<DaySchedule> daySchedule)
        {
            List<DoctorDailySchedule> dailySchedulesList = new List<DoctorDailySchedule>();
            foreach (var item in daySchedule)
            {
                DoctorDailySchedule doctorDailySchedule = new DoctorDailySchedule()
                {
                    Day = item.Day,
                    From = item.From,
                    To = item.To,
                    TotalHours = item.TotalHours
                };
                dailySchedulesList.Add(doctorDailySchedule);
            }
            return dailySchedulesList;
        }

        private void ValidatePractitionerData(PractitionerDataV3 data, PractitionerInfoRequest practitionerInfoRequest)
        {
            if (practitionerInfoRequest.IsGregorian)
            {
                if (data.birth_date_gregorian != null)
                {

                    var PractitionerDataDoB = Convert.ToDateTime(data.birth_date_gregorian).ToString("yyyy-MM-dd", CultureInfo.GetCultureInfo("en-GB"));
                    if (PractitionerDataDoB != practitionerInfoRequest.DateOfBirth.Trim())
                    {
                        // check +2 days or -2 days of the birth date
                        CheckRangeDoB(practitionerInfoRequest.DateOfBirth, Convert.ToDateTime(data.birth_date_gregorian).ToString("yyyy-MM-dd"), true);
                    }
                }
                else
                {
                    _logger.LogError("Practioner Gregorian date of birth not added in registry");
                    throw new CustomHttpException(HttpStatusCode.BadRequest, _resourceService.GetResource("Gregorian_DOB_Not_Added"));
                }
            }
            else
            {
                if (data.birth_date_hijri != null)
                {
                    var PractitionerDataDoB = data.birth_date_hijri;
                    if (PractitionerDataDoB != practitionerInfoRequest.DateOfBirth.Trim())
                    {
                        // check +2 days or -2 days of the birth date
                        CheckRangeDoB(practitionerInfoRequest.DateOfBirth, data.birth_date_hijri, false);

                    }
                }
                else
                {
                    _logger.LogError("Practioner hijri date of birth not added in registry");
                    throw new CustomHttpException(HttpStatusCode.BadRequest, _resourceService.GetResource("Hijri_DOB_Not_Added"));
                }
            }


            if (!CheckExpiryDate(data.scfhs.license_expiry_date))
            {
                _logger.LogError("Scfhs Registration Date is Expired");
                throw new CustomHttpException(HttpStatusCode.BadRequest, _resourceService.GetResource("Scfhs_Registration_Expired"));
            }
            if (data.nationality_code == null)
            {
                _logger.LogError("Practitioner Nationality Not Found");
                throw new CustomHttpException(HttpStatusCode.BadRequest, _resourceService.GetResource("Practitioner_Nationality_Not_Found"));
            }
            if (data.nationality_code != "SAU")
            {
                _logger.LogError("Practitioner Nationality Not Saudi");
                throw new CustomHttpException(HttpStatusCode.BadRequest, _resourceService.GetResource("Practitioner_Not_Saudi"));
            }
            if (data.scfhs.rank_name_ar == null && data.scfhs.rank_name_en == null)
            {
                _logger.LogError("SCFHS category is NUL");
                throw new CustomHttpException(HttpStatusCode.BadRequest, _resourceService.GetResource("Practitioner_Not_Consultant"));
            }
            if (!data.scfhs.rank_name_ar.Contains("استشاري") || !data.scfhs.rank_name_en.Contains("Consultant")) //scfhs_category_code 20 or 9
            {
                _logger.LogError("Practitioner Must be a Medical Consultant");
                throw new CustomHttpException(HttpStatusCode.BadRequest, _resourceService.GetResource("Practitioner_Not_Consultant"));
            }
        }

        private void ValidateEstablishmentData(PrivateestablishmentData data)
        {
            if (data.license_expiry_date == null || data.license_expiry_date == "")
            {
                _logger.LogError("Establishment License Date is Null");
                throw new CustomHttpException(HttpStatusCode.BadRequest, _resourceService.GetResource("Establishment_License_Expired"));
            }
            if (!CheckExpiryDate(DateTime.Parse(data.license_expiry_date, CultureInfo.GetCultureInfo("en-GB"))))
            {
                _logger.LogError("Establishment License Expired");
                throw new CustomHttpException(HttpStatusCode.BadRequest, _resourceService.GetResource("Establishment_License_Expired"));
            }
            if (data.license_status != "VALID")
            {
                _logger.LogError("Establishment Status Not VALID");
                throw new CustomHttpException(HttpStatusCode.BadRequest, _resourceService.GetResource("Establishment_Status_Error"));
            }
        }

        public bool CheckExpiryDate(DateTime? expiry_date)
        {
            if (expiry_date == null) return false;

            else if (expiry_date != null)
            {
                if (expiry_date < DateTime.Today) return false;
            }

            return true;
        }
        private async Task<string> Format(ServiceCode code)
        {
            if (!await IsValid(code))
                return string.Empty;

            return $"{code.Prefix}{code.Date.Value.ToString("yyMMdd", CultureInfo.InvariantCulture)}{code.Code:D5}";
        }
        private Task<bool> IsValid(ServiceCode code)
        {
            return Task.FromResult(!string.IsNullOrWhiteSpace(code.Prefix) && code.Date != null && code.Code != 0);
        }

        private string[] SpiltFullName(string FullName)
        {
            var Names = FullName.Split(" ");

            return Names;
        }

        private async Task<govEstablishmentInfo> GetGovInfo(string establishmentOrgId)
        {
            var govEstData = await _organizationRegistryService.GetOrganizationData(establishmentOrgId);

            var city = await _unitOfWork.Repository<DAL.Models.City>().Get(x => x.NameAr.Contains(govEstData.city_ar) || x.NameEn.Contains(govEstData.city_en)).FirstOrDefaultAsync();
            var region = await _unitOfWork.Repository<Region>().Get(x => x.NameAr.Contains(govEstData.region_ar) || x.NameEn.Contains(govEstData.region_en)).FirstOrDefaultAsync();
            if (city == null)
            {
                city = await _unitOfWork.Repository<DAL.Models.City>().Get().FirstOrDefaultAsync();
            } 
            if (region == null)
            {
                region = await _unitOfWork.Repository<Region>().Get().FirstOrDefaultAsync();
            }

            if (!CheckIsGovPractitioner(govEstData.license_number))
            {
                _logger.LogError("********* GetGovInfo ******** is gov or prv, so license_number = ( " + govEstData.license_number + "----"+ govEstData.sector_ar + ")", govEstData.sector_en);
                throw new CustomHttpException(HttpStatusCode.BadRequest, _resourceService.GetResource("Practitioner_Not_In_Gov_Est"));
            }

            if (govEstData.seha_id == null )
            {
                _logger.LogError("********* GetGovInfo ******** Practitoiner Organnization Id comes from OrgRegistry Null");
                throw new CustomHttpException(HttpStatusCode.BadRequest, _resourceService.GetResource("Practitioner_Gov_Org_Is_Null"));
            }

            #region gov Establishment Info
            var govEstablishmentInfo = new govEstablishmentInfo
            {
                nameAr = govEstData.name_ar,
                nameEn = govEstData.name_en,
                sehaOrganizationId = Convert.ToInt32(govEstData.seha_id),
                managementAreaId = Convert.ToInt32(govEstData.health_directory_seha_id),
                cityId = Convert.ToInt32(city.CityId),
                cityAr = govEstData.city_ar,
                cityEn = govEstData.city_en,
                regionId = Convert.ToInt32(govEstData.region_code),
                clusterId = govEstData.cluster_code,

                approvalLevel = (govEstData.sector_identifier == "H") ? 
                (int)ApprovalLevels.TwoApprovals : (int)ApprovalLevels.OneApproval,
                facilitySector = govEstData.sector_identifier
            };

            if (govEstablishmentInfo.approvalLevel == (int)ApprovalLevels.TwoApprovals)
            {
                govEstablishmentInfo.sehaOrganizationIdLevel2 = await GetAdministrativeOrgId(govEstData);
            }
            #endregion

            return govEstablishmentInfo;
        }

        private async Task<PrivateestablishmentData> GetPriInfo(string establishmentOrgId)
        {
            var priEstData = await _organizationRegistryService.GetOrganizationData(establishmentOrgId);

            if (priEstData.license_expiry_date == null && priEstData.license_status ==  null)
            {
                _logger.LogError("********* GetPriInfo ******** license status and license expiry date  comes from OrgRegistry Null");
                throw new CustomHttpException(HttpStatusCode.BadRequest, _resourceService.GetResource("License_Pri_Org_Is_Null"));
            }
            var priEstablishmentInfo = new PrivateestablishmentData
            {
                name_ar = priEstData.name_ar,
                name_en = priEstData.name_en,
                license_number = priEstData.license_number,
                license_status = priEstData.license_status,
                license_expiry_date = priEstData.license_expiry_date,
            };

            return priEstablishmentInfo;
        }


        private async Task<int?> GetAdministrativeOrgId(DTO.DTOs.OrganizationRegistry.OrganizationData govEstData)
        {
            var adminsteractiveOrgId = new AdminstrativeOrgLookup();
            if (govEstData.cluster_code != null)
            {
                adminsteractiveOrgId = await _unitOfWork.Repository<AdminstrativeOrgLookup>().Get(x =>
                x.RegistryId == Convert.ToInt32(govEstData.cluster_code) &&
                x.IsCluster == true).FirstOrDefaultAsync();

                if (adminsteractiveOrgId != null)
                    return adminsteractiveOrgId.SehaOrganizationId;
            }

            adminsteractiveOrgId = await _unitOfWork.Repository<AdminstrativeOrgLookup>().Get(x =>
            x.RegistryId == Convert.ToInt32(govEstData.health_directory_seha_id) &&
            x.IsCluster == false).FirstOrDefaultAsync();

            if (adminsteractiveOrgId != null)
                return adminsteractiveOrgId.SehaOrganizationId;

            return null;
            //throw new CustomHttpException(HttpStatusCode.BadRequest, _resourceService.GetResource("Adminsteractive_Org_Not_Found"))
        }

        public bool CheckIsGovPractitioner(string licenseNumber)
        {
            if (licenseNumber != null) return false;

            return true;
        }

        private bool CheckRangeDoB(string RequestDoB, string RegistryDoB, bool IsGregorian)
        {
            if (!IsGregorian)
            {
                RequestDoB = DateHelper.ConvertToGregorian(RequestDoB);
                RegistryDoB = DateHelper.ConvertToGregorian(RegistryDoB);
            }

            if (Convert.ToDateTime(RequestDoB).AddDays(-1).ToString("yyyy-MM-dd") != RegistryDoB)
            {
                if (Convert.ToDateTime(RequestDoB).AddDays(-2).ToString("yyyy-MM-dd") != RegistryDoB)
                {
                    if (Convert.ToDateTime(RequestDoB).AddDays(1).ToString("yyyy-MM-dd") != RegistryDoB)
                    {
                        if (Convert.ToDateTime(RequestDoB).AddDays(2).ToString("yyyy-MM-dd") != RegistryDoB)
                        {
                            _logger.LogError("Entered Date Of Birth is Wrong");
                            throw new CustomHttpException(HttpStatusCode.BadRequest, _resourceService.GetResource("Wrong_DOB"));
                        }
                    }
                }
            }

            return true;

        }



        private bool CheckHlsLicense(List<HlsLicense> hlsLicenses)
        {
            
            foreach (var item in hlsLicenses)
            {

                var test = CheckExpiryDate(item.license_expiry_date);

                var ExpirationStatus = item.expiration_status.ToUpper();
                if (ExpirationStatus != "EXPIRED" && ExpirationStatus != "CANCELLED")
                {
                    return false;
                }
            }
            return true;
        }

        public async Task<ResultOfAction<List<RejectionReasonsResponse>>> GetRejectionReasonsList()
        {
            List<RejectionReasonsResponse> Response = new List<RejectionReasonsResponse>();

            var rejectionReasonsLookup = await _unitOfWork.Repository<RejectionReasonsLookup>().Get().ToListAsync();
            var IsCultureArabic = _commonService.IsArabicLangHeader();

            if (IsCultureArabic)
            {
                foreach (var item in rejectionReasonsLookup)
                {
                    Response.Add(
                        new RejectionReasonsResponse
                        {
                            id = item.Id,
                            text = item.ReasonAr
                        }
                   );
                }
            }
            else
            {
                foreach (var item in rejectionReasonsLookup)
                {
                    Response.Add(
                        new RejectionReasonsResponse
                        {
                            id = item.Id,
                            text = item.ReasonEn
                        }
                   );
                }
            }

            Response.Reverse(); 

            return new ResultOfAction<List<RejectionReasonsResponse>>((int)HttpStatusCode.OK, null, Response);
        }

        public async Task<ResultOfAction<DefaultModel>> SendAnatSmsAndNotificationAfterFiveDays(SendAnatNotificationParameters sendAnatNotificationParameters)
        {
            var IsArabic = _commonService.IsArabicLangHeader();
            // Send Anat Notification
            var PractionerName = sendAnatNotificationParameters.PractFullNameEn;
            var SendNotificationTitle = "Pending practitioner approval” exceeded 5 days";
            var SendNotificationBody = $"Dear {PractionerName}. {Environment.NewLine} You have a request from Khabeer Alseha, pending your approval. {Environment.NewLine} We wish you all the best. ";
            var SMSTxtBody = $"Dear {PractionerName}. {Environment.NewLine} You have a request from Khabeer Alseha, pending your approval. {Environment.NewLine} You can view the details of the request and take the necessary action through Anat application link: https://anat.page.link/job-marketplace";

            if (IsArabic)
            {
                PractionerName = sendAnatNotificationParameters.PractFullNameAr;
                SendNotificationTitle = "فى حالة مرور 5 ايام وحالة الطلب - فى انتظار قبول الممارس";
                SendNotificationBody = $"عزيزى {PractionerName}. {Environment.NewLine} يوجد لديك طلب عمل في القطاع الخاص خارج أوقات العمل الرسمية بانتظار موافقتك. {Environment.NewLine} متمنين لكم دوام التوفيق. ";
                SMSTxtBody = $"عزيزى {PractionerName}. {Environment.NewLine} يوجد لديك طلب عمل في القطاع الخاص خارج أوقات العمل الرسمية بانتظار موافقتك. {Environment.NewLine} بإمكانك الاطلاع على تفاصيل الطلب واتخاذ الاجراء اللازم عن طريق تطبيق أناة على الرابط:https://anat.page.link/job-marketplace";
            }

            var NewRequestNotification = new NotificationToAnatRequest
            {
                ServiceName = _resourceService.GetResource("ServiceName"),
                Title = SendNotificationTitle,
                Body = SendNotificationBody,
                NumberId = new List<string> { sendAnatNotificationParameters.PractNationalId },
                requestInfo = new RequestInfo
                {
                    Type = "dual_practice",
                    DualPractice = new DualPractice
                    {
                        RequestId = sendAnatNotificationParameters.RequestId
                    }
                },
                SendSms = new SMS
                {
                    IsSent = true,
                    SmsBody = SMSTxtBody
                }
            };
            try
            {

                var result = await _anatService.SendNotificationToAnat(NewRequestNotification);
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception Message : {e}");
                _logger.LogError($"CancelRequestByPrivateOrg - SendNotificationToAnat - Failed to send Notification To Anat , NumberId : {NewRequestNotification.NumberId} , RequestId : {NewRequestNotification.requestInfo.DualPractice.RequestId}");
                return new ResultOfAction<DefaultModel>((int)HttpStatusCode.OK, _resourceService.GetResource("Request_Canceled"), new DefaultModel());
            }
            return new ResultOfAction<DefaultModel>((int)HttpStatusCode.OK, _resourceService.GetResource("Request_Canceled"), new DefaultModel());
        }
    }


}
