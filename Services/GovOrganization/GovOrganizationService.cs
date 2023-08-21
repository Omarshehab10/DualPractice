using AutoMapper;
using Common.Enums;
using Common.Localization;
using Common.Types;
using DAL.Models;
using DTO.DTOs;
using DTO.DTOs.EmailSetting;
using DTO.DTOs.Parameters;
using DTO.DTOs.Responses;
using Lean.Framework.Entities.Provider;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Services.Common;
using Services.ExternalServices.Anat;
using Services.ExternalServices.SendEmailService;
using Services.UnitOfWork;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Services.GovOrganization
{
    public class GovOrganizationService : IGovOrganizationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ILogger<GovOrganizationService> _logger;
        private readonly IAppResourceService _resourceService;
        private readonly ISehaIntegrationService _sehaIntegrationService;
        private readonly ICommonService _commonService;
        private readonly IEmailService _emailService;
        private readonly IAnatService _anatService;
        private readonly IPdfService _pdfService;


        public GovOrganizationService(
            IUnitOfWork unitOfWork, IMapper mapper,
            IConfiguration configuration, IAppResourceService resourceService,
            ILogger<GovOrganizationService> logger, IEmailService emailService, IServiceProvider serviceProvider,
            ICommonService commonService, IPdfService pdfService,
             IAnatService anatService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
            _logger = logger;
            _emailService = emailService;
            _resourceService = resourceService;
            _commonService = commonService;
            _sehaIntegrationService = serviceProvider.GetRequiredService<ISehaIntegrationService>();
            _anatService = anatService;
            _pdfService = pdfService;

        }

        public async Task<ResultOfAction<DefaultModel>> GovOrganisationReview(GetRequestInfo getRequestInfo)
        {
            var user = _sehaIntegrationService.GetUserData();

            _logger.LogInformation("********* Get User Info From Seha = (" + user.UserId + ")", user.UserId);
            _logger.LogInformation("********* Get Request Service Code Info From request = (" + getRequestInfo.RequestServiceCode + ")", getRequestInfo.RequestServiceCode);

            var RequestInfo = await _unitOfWork.Repository<DpRequest>().Get(x => x.NormalizedServiceCode == getRequestInfo.RequestServiceCode).Include(y => y.RequestingOrg).Include(y => y.PractionerMainOrg).Include(y => y.PractionerIdNumberNavigation).FirstOrDefaultAsync();

            AddLogRequest addLogRequest = new AddLogRequest();
            addLogRequest.NormalizedServiceCode = RequestInfo.NormalizedServiceCode;
            addLogRequest.UserId = user.UserId;
            var IsArabic = _commonService.IsArabicLangHeader();

            var PractionerRequestsList = await _unitOfWork.Repository<DpRequest>().Get(x => x.PractionerIdNumber == RequestInfo.PractionerIdNumber).ToListAsync();

            if (RequestInfo == null)
            {
                _logger.LogError($"GovOrganisationReview - This Request Is Not Found");
                throw new CustomHttpException(HttpStatusCode.BadRequest, _resourceService.GetResource("This_Request_Is_Not_Found"));
            }

            if (RequestInfo.RequestStatus != (int)RequestStatus.Waiting_Pu_Accept1)
            {
                _logger.LogError($"GovOrganisationReview - Request Status Must Be Waiting for Government Review 1");
                throw new CustomHttpException(HttpStatusCode.BadRequest, _resourceService.GetResource("Request_Status_Not_Waiting_Gov1_Review"));
            }

            EmailContent emailContent = new EmailContent();
            UsersInfoRequest usersInfoRequest = new UsersInfoRequest();
            var RolePerId = _configuration["UserPermissionId:AddDPReq"];

            if (getRequestInfo.Approval == false)
            {
                RequestInfo.RequestStatus = (int)RequestStatus.Rejected_Pu3;
                RequestInfo.Comment.Add(new Comment
                {
                    Text = getRequestInfo.Comment,
                    DprequestId = RequestInfo.Id,
                    CreatedBy = user.UserId,
                    CreateDate = DateTime.Now
                });
                RequestInfo.UpdatedBy = user.UserId;
                RequestInfo.UpdateDate = DateTime.Now;
                _logger.LogInformation("********* update or Add Request Info Reject  = (" + RequestInfo + ")", RequestInfo);
                _unitOfWork.Repository<DpRequest>().Update(RequestInfo);
                await _unitOfWork.SaveChangesAsync();

                // Send Anat Notification
                var PractionerName = RequestInfo.PractionerIdNumberNavigation.FullNameEn;
                var SendNotificationTitle = "Request Is Rejected By Government Level 1";
                var SendNotificationBody = $" Dear {PractionerName}. {Environment.NewLine} We would like to inform you that Khabeer Alseha request No.: {RequestInfo.NormalizedServiceCode} was rejected by the government health establishment. {Environment.NewLine} We wish you all the best.";
                var SMSTxtBody = $" Dear {PractionerName}. {Environment.NewLine} We would like to inform you that Khabeer Alseha request No.: {RequestInfo.NormalizedServiceCode} was rejected by the government health establishment. {Environment.NewLine} You can view the details of the request and take the necessary action through Anat application link: https://anat.page.link/job-marketplace";
                if (IsArabic)
                {
                    PractionerName = RequestInfo.PractionerIdNumberNavigation.FullNameAr;
                    SendNotificationTitle = "الطلب مرفوض من قبل المنشأة الحكومية المستوى الأول";
                    SendNotificationBody = $" عزيزى {PractionerName}. {Environment.NewLine} نود التنويه أنه تم رفض طلب خبير الصحة رقم: {RequestInfo.NormalizedServiceCode}، من قِبل المنشأة الصحية الحكومية. {Environment.NewLine} متمنين لكم دوام التوفيق. ";
                    SMSTxtBody = $" عزيزى {PractionerName}. {Environment.NewLine} نود التنويه أنه تم رفض طلب خبير الصحة رقم: {RequestInfo.NormalizedServiceCode}، من قِبل المنشأة الصحية الحكومية. {Environment.NewLine} بإمكانك الاطلاع على تفاصيل الطلب واتخاذ الاجراء اللازم عن طريق تطبيق أناة على الرابط:https://anat.page.link/job-marketplace";

                }

                var NewRequestNotification = new NotificationToAnatRequest
                {
                    ServiceName = _resourceService.GetResource("ServiceName"),
                    Title = SendNotificationTitle,
                    Body = SendNotificationBody,
                    NumberId = new List<string> { RequestInfo.PractionerIdNumberNavigation.NationalId },
                    requestInfo = new RequestInfo
                    {
                        Type = "dual_practice",
                        DualPractice = new DualPractice
                        {
                            RequestId = RequestInfo.NormalizedServiceCode
                        }
                    },
                    SendSms = new SMS
                    {
                        IsSent = true,
                        SmsBody = SMSTxtBody
                    }
                };
                await _anatService.SendNotificationToAnat(NewRequestNotification);

                //Add Action Log
                addLogRequest.ActionType = AddLogs.Level1_GovEst_Reject_the_request;
                await _commonService.AddActionRequestLog(addLogRequest);

                // Sending Email updating Refuse to Priv Org

                _logger.LogInformation("********* Get Role Permission Of the User = (" + RolePerId + ")", RolePerId);

                usersInfoRequest.PermissionId = int.Parse(RolePerId);
                usersInfoRequest.OrganizationId = RequestInfo.RequestingOrg.OrganizationId;
                usersInfoRequest.ServiceId = 47;

                _logger.LogInformation("********* Get users Info Request Of the User to SendMail = (" + usersInfoRequest.OrganizationId + ")", usersInfoRequest.OrganizationId);

                emailContent.RequestId = RequestInfo.NormalizedServiceCode;
                emailContent.EmailSubjectOrg = _resourceService.GetResource("Email_Update_On_TheStatus_Of_request_Subj");
                emailContent.EmailBody = $"<div dir='rtl' lang='ar'> نفيدكم بأن الطلب {RequestInfo.NormalizedServiceCode} <br> المرسل للمارس {RequestInfo.PractionerIdNumberNavigation.FullNameAr}<br> قد تم رفضه من قبل المنشأة الحكومية/ {RequestInfo.PractionerMainOrg.NameAr} <br> للاطلاع على أسباب الرفض الرجاء الضغط على الرابط أدناه<br> رابط الطلب: https://dualpractice.seha.sa/sehaaccount<br> للاستفسارات الرجاء التواصل مع فريق الدعم support@seha.sa </div>";
                emailContent.Contents = File.ReadAllText(_commonService.MapPath(@"StaticFiles/SendOrganizationNewRequest/SendOrganisationNewReqEmailHtmlBody.html"));

                _logger.LogInformation("********* Get Email Content Request Of the User to SendMail = (" + emailContent.EmailSubjectOrg + " **** " + emailContent.RequestId + ")", usersInfoRequest.OrganizationId);

                await _emailService.SendMailOrg(usersInfoRequest, emailContent);

                return new ResultOfAction<DefaultModel>((int)HttpStatusCode.OK, _resourceService.GetResource("Request_Rejected"), new DefaultModel());
            }

            bool RequestApprovedLeveloneOtherOrg = PractionerRequestsList.AsQueryable().Any(x =>
            x.RequestStatus == (int)RequestStatus.Waiting_Pu_Accept2 || x.RequestStatus == (int)RequestStatus.Waiting_Pr_Pay_2

            );
            if (RequestApprovedLeveloneOtherOrg)
            {
                _logger.LogError($"GovOrganisationReview - the practitioner has an existing request");

                throw new CustomHttpException((int)HttpStatusCode.BadRequest, _resourceService.GetResource("Practitioner_Has_Existing_Request"));
            }
            bool RequestApproved = PractionerRequestsList.AsQueryable().Any(x =>
            x.RequestStatus == (int)RequestStatus.Approved

            );
            if (RequestApproved)
            {
                _logger.LogError($"GovOrganisationReview - the practitioner has valid Approved ");

                throw new CustomHttpException((int)HttpStatusCode.BadRequest, _resourceService.GetResource("Practitioner_Already_Approved"));
            }

            var Waitinglevelone = PractionerRequestsList.AsQueryable().Where(x =>
            x.RequestStatus == (int)RequestStatus.Waiting_Pu_Accept1
            ).ToList();
            if (Waitinglevelone.Count > 1)
            {
                _logger.LogError($"GovOrganisationReview - the practitioner has more than one request Waiting gov leve lone to accept ");

                throw new CustomHttpException(HttpStatusCode.BadRequest, _resourceService.GetResource("There_Two_Request_Same_Practitioner"));
            }
            RequestInfo.RequestStatus = (int)RequestStatus.Waiting_Pr_Pay_2;

            if (RequestInfo.ApprovalLevel == 2)
            {
                RequestInfo.RequestStatus = (int)RequestStatus.Waiting_Pu_Accept2;
            }
            RequestInfo.UpdatedBy = user.UserId;
            RequestInfo.UpdateDate = DateTime.Now;
            _logger.LogInformation("********* update or Add Request Info Approve  = (" + RequestInfo + ")", RequestInfo);
            _unitOfWork.Repository<DpRequest>().Update(RequestInfo);
            await _unitOfWork.SaveChangesAsync();

            //Add Action Log
            addLogRequest.ActionType = AddLogs.Level1_GovEst_Accept_the_request;
            await _commonService.AddActionRequestLog(addLogRequest);

            //Sending Email

            RolePerId = _configuration["UserPermissionId:GovLvl2OrgReviewDPReq"];
            _logger.LogInformation("********* Get Role Permission Of the User = (" + RolePerId + ")", RolePerId);

            usersInfoRequest.PermissionId = int.Parse(RolePerId);
            if (RequestInfo.MohLevel2SehaOrgId !=null)
            {
                usersInfoRequest.OrganizationId = (int)RequestInfo.MohLevel2SehaOrgId;
            }
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
            emailContent.EmailBody = $"<div dir='rtl' lang='ar'> نفيدكم بأن الطلب {RequestInfo.NormalizedServiceCode} <br> المرسل للمارس {RequestInfo.PractionerIdNumberNavigation.FullNameAr}<br> قد تم قبوله من قبل المنشأة الحكومية/ {RequestInfo.PractionerMainOrg.NameAr}  <br> رابط الطلب: https://dualpractice.seha.sa/sehaaccount <br> للاستفسارات الرجاء التواصل مع فريق الدعم support@seha.sa </div>";
            emailContent.Contents = File.ReadAllText(_commonService.MapPath(@"StaticFiles/SendOrganizationNewRequest/SendOrganisationNewReqEmailHtmlBody.html"));

            _logger.LogInformation("********* Get Email Content Request Of the User to SendMail = (" + emailContent.EmailSubjectOrg + " **** " + emailContent.RequestId + ")", usersInfoRequest.OrganizationId);

            await _emailService.SendMailOrg(usersInfoRequest, emailContent);

            return new ResultOfAction<DefaultModel>((int)HttpStatusCode.OK, _resourceService.GetResource("Request_Aproved"), new DefaultModel());
        }
        public async Task<ResultOfAction<DefaultModel>> GovOrganisationReviewLevelTwo(GetRequestInfo getRequestInfo)
        {
            var user = _sehaIntegrationService.GetUserData();

            _logger.LogInformation("********* Get User Info From Seha = (" + user.UserId + ")", user.UserId);
            _logger.LogInformation("********* Get Request Service Code Info From request = (" + getRequestInfo.RequestServiceCode + ")", getRequestInfo.RequestServiceCode);

            var RequestInfo = await _unitOfWork.Repository<DpRequest>().Get(x => x.NormalizedServiceCode == getRequestInfo.RequestServiceCode).Include(y => y.RequestingOrg).Include(y => y.PractionerIdNumberNavigation).FirstOrDefaultAsync();
            AddLogRequest addLogRequest = new AddLogRequest();
            addLogRequest.NormalizedServiceCode = RequestInfo.NormalizedServiceCode;
            addLogRequest.UserId = user.UserId;
            var IsArabic = _commonService.IsArabicLangHeader();

            if (RequestInfo == null)
            {
                _logger.LogError($"GovOrganisationReviewLevelTwo - This Request Is Not Found");
                throw new CustomHttpException(HttpStatusCode.BadRequest, _resourceService.GetResource("This_Request_Is_Not_Found"));
            }

            if (RequestInfo.RequestStatus != (int)RequestStatus.Waiting_Pu_Accept2)
            {
                _logger.LogError($"GovOrganisationReviewLevelTwo - Request Status Must Be Waiting for Government Review Level 2");
                throw new CustomHttpException(HttpStatusCode.BadRequest, _resourceService.GetResource("Request_Status_Not_Waiting_Gov2_Review"));
            }
            
            // Prepare Sending email objects properties
            var GovOrganizationLvl2 = await _unitOfWork.Repository<Organization>().Get(x => x.OrganizationId == RequestInfo.MohLevel2SehaOrgId).FirstOrDefaultAsync();
            var GovOrganizationLvl2Name = GovOrganizationLvl2.NameAr;
            UsersInfoRequest usersInfoRequest = new UsersInfoRequest();
            var RolePerId = _configuration["UserPermissionId:AddDPReq"];
            _logger.LogInformation("********* Get Role Permission Of the User = (" + RolePerId + ")", RolePerId);
            usersInfoRequest.PermissionId = int.Parse(RolePerId);
            usersInfoRequest.OrganizationId = RequestInfo.RequestingOrg.OrganizationId;
            usersInfoRequest.ServiceId = 47;
            _logger.LogInformation("********* Get users Info Request Of the User to SendMail = (" + usersInfoRequest.OrganizationId + ")", usersInfoRequest.OrganizationId);
            EmailContent emailContent = new EmailContent();
            emailContent.RequestId = RequestInfo.NormalizedServiceCode;
            emailContent.EmailSubjectOrg = _resourceService.GetResource("Email_Update_On_TheStatus_Of_request_Subj");
            emailContent.Contents = File.ReadAllText(_commonService.MapPath(@"StaticFiles/SendOrganizationNewRequest/SendOrganisationNewReqEmailHtmlBody.html"));

            if (getRequestInfo.Approval == false)
            {
                RequestInfo.RequestStatus = (int)RequestStatus.Rejected_Pu4;
                RequestInfo.Comment.Add(new Comment
                {
                    Text = getRequestInfo.Comment,
                    DprequestId = RequestInfo.Id,
                    CreatedBy = user.UserId,
                    CreateDate = DateTime.Now
                });
                RequestInfo.UpdatedBy = user.UserId;
                RequestInfo.UpdateDate = DateTime.Now;
                _logger.LogInformation("********* update or Add Request Info Reject  = (" + RequestInfo + ")", RequestInfo);
                _unitOfWork.Repository<DpRequest>().Update(RequestInfo);
                await _unitOfWork.SaveChangesAsync();


                // Send Anat Notification
                var PractionerName = RequestInfo.PractionerIdNumberNavigation.FullNameEn;
                var SendNotificationTitle = "Request Is Rejected By Government Level 2";
                var SendNotificationBody = $" Dear {PractionerName}. {Environment.NewLine} We would like to inform you that Khabeer Alseha request No.: {RequestInfo.NormalizedServiceCode} was rejected by the government entity Cluster/Directorate. {Environment.NewLine} We wish you all the best.";
                var SMSTxtBody = $" Dear {PractionerName}. {Environment.NewLine} We would like to inform you that Khabeer Alseha request No.: {RequestInfo.NormalizedServiceCode} was rejected by the government entity Cluster/Directorate. {Environment.NewLine} You can view the details of the request and take the necessary action through Anat application link: https://anat.page.link/job-marketplace";
                if (IsArabic)
                {
                    PractionerName = RequestInfo.PractionerIdNumberNavigation.FullNameAr;
                    SendNotificationTitle = "الطلب مرفوض من قبل المنشأة الحكومية المستوى الثانى";
                    SendNotificationBody = $" عزيزى {PractionerName}. {Environment.NewLine} نود التنويه أنه تم رفض طلب خبير الصحة رقم: {RequestInfo.NormalizedServiceCode}، من قِبل الجهة الحكومية المديرية/التجمع. {Environment.NewLine} متمنين لكم دوام التوفيق. ";
                    SMSTxtBody = $" عزيزى {PractionerName}. {Environment.NewLine} نود التنويه أنه تم رفض طلب خبير الصحة رقم: {RequestInfo.NormalizedServiceCode}، من قِبل الجهة الحكومية المديرية/التجمع. {Environment.NewLine} بإمكانك الاطلاع على تفاصيل الطلب واتخاذ الاجراء اللازم عن طريق تطبيق أناة على الرابط:https://anat.page.link/job-marketplace";
                }

                var NewRequestNotification = new NotificationToAnatRequest
                {
                    ServiceName = _resourceService.GetResource("ServiceName"),
                    Title = SendNotificationTitle,
                    Body = SendNotificationBody,
                    NumberId = new List<string> { RequestInfo.PractionerIdNumberNavigation.NationalId },
                    requestInfo = new RequestInfo
                    {
                        Type = "dual_practice",
                        DualPractice = new DualPractice
                        {
                            RequestId = RequestInfo.NormalizedServiceCode
                        }
                    },
                    SendSms = new SMS
                    {
                        IsSent = true,
                        SmsBody = SMSTxtBody
                    }
                };
                await _anatService.SendNotificationToAnat(NewRequestNotification);

                //Add Action Log
                addLogRequest.ActionType = AddLogs.Level2_GovEst_Reject_the_request;
                await _commonService.AddActionRequestLog(addLogRequest);

                // Sending Email updating Refuse to Priv Org

                emailContent.EmailBody = $"<div dir='rtl' lang='ar'> نفيدكم بأن الطلب {RequestInfo.NormalizedServiceCode} <br> المرسل للمارس {RequestInfo.PractionerIdNumberNavigation.FullNameAr}<br> قد تم رفضه من قبل المنشأة الحكومية المستوى الثانى/ {GovOrganizationLvl2Name} <br> للاطلاع على أسباب الرفض الرجاء الضغط على الرابط أدناه<br> رابط الطلب: https://dualpractice.seha.sa/sehaaccount<br> للاستفسارات الرجاء التواصل مع فريق الدعم support@seha.sa </div>";

                _logger.LogInformation("********* Get Email Content Request Of the User to SendMail = (" + emailContent.EmailSubjectOrg + " **** " + emailContent.RequestId + ")", usersInfoRequest.OrganizationId);

                await _emailService.SendMailOrg(usersInfoRequest, emailContent);

                return new ResultOfAction<DefaultModel>((int)HttpStatusCode.OK, _resourceService.GetResource("Request_Rejected"), new DefaultModel());
            }
            RequestInfo.RequestStatus = (int)RequestStatus.Waiting_Pr_Pay_2;
            RequestInfo.UpdatedBy = user.UserId;
            RequestInfo.UpdateDate = DateTime.Now;
            _logger.LogInformation("********* update or Add Request Info Approve = (" + RequestInfo + ")", RequestInfo);
            _unitOfWork.Repository<DpRequest>().Update(RequestInfo);
            await _unitOfWork.SaveChangesAsync();

            //Add Action Log
            addLogRequest.ActionType = AddLogs.Level2_GovEst_Accept_the_request;
            await _commonService.AddActionRequestLog(addLogRequest);

            // Sending Email updating approve to Priv Org

            emailContent.EmailBody = $"<div dir='rtl' lang='ar'> نفيدكم بأن الطلب {RequestInfo.NormalizedServiceCode} <br> المرسل للمارس {RequestInfo.PractionerIdNumberNavigation.FullNameAr}<br> قد تم قبوله من قبل المنشأة الحكومية المستوى الثانى/ {GovOrganizationLvl2Name}  <br> رابط الطلب: https://dualpractice.seha.sa/sehaaccount <br> للاستفسارات الرجاء التواصل مع فريق الدعم support@seha.sa </div>";

            _logger.LogInformation("********* Get Email Content Request Of the User to SendMail = (" + emailContent.EmailSubjectOrg + " **** " + emailContent.RequestId + ")", usersInfoRequest.OrganizationId);

            await _emailService.SendMailOrg(usersInfoRequest, emailContent);

            return new ResultOfAction<DefaultModel>((int)HttpStatusCode.OK, _resourceService.GetResource("Request_Aproved"), new DefaultModel());
        }

        public async Task<ResultOfAction<List<RequestsOfPractionerMainOrgResponse>>> GetListRequestsOfPractionerLevelOneOrg()
        {
            List<RequestsOfPractionerMainOrgResponse> Response = new List<RequestsOfPractionerMainOrgResponse>();

            var userData = _sehaIntegrationService.GetUserData();
            var orgId = userData.RequestInfoId;
            var IsCultureArabic = _commonService.IsArabicLangHeader();

            var DPRequests = await _unitOfWork.Repository<DpRequest>().Get(x => x.PractionerMainOrg.Id == orgId)
               .Where(x => x.RequestStatus != (int)RequestStatus.New && x.RequestStatus != (int)RequestStatus.Renewed)
               .Include(x => x.RequestingOrg)
               .Include(x => x.PractionerIdNumberNavigation)
               .OrderByDescending(o => o.CreateDate)
               .ToListAsync();


            if (DPRequests.Count == 0)
            {
                _logger.LogError($"GetListRequestsOfPractionerLevelOneOrg  There are no requests");

                return new ResultOfAction<List<RequestsOfPractionerMainOrgResponse>>((int)HttpStatusCode.NoContent, _resourceService.GetResource("There_No_Resuests"), Response);
            }

            if (IsCultureArabic)
            {
                foreach (var DPRequest in DPRequests)
                {
                    Response.Add(
                        new RequestsOfPractionerMainOrgResponse
                        {
                            ReqServiceCode = DPRequest.NormalizedServiceCode,
                            NationalId = DPRequest.PractionerIdNumberNavigation.NationalId,
                            PractionerName = DPRequest.PractionerIdNumberNavigation.FullNameAr,
                            DateOfRequset = DPRequest.CreateDate.ToString(),
                            TotalWeeklyHours = Convert.ToDecimal(DPRequest.TotalWeeklyHours),
                            RequestingOrgId = DPRequest.RequestingOrg.Id,
                            RequestingOrgName = DPRequest.RequestingOrg.NameAr,
                            ResquestStatus = DPRequest.RequestStatus,
                            ApprovalEndDate = DPRequest.ToDate
                        }
                        );
                }
            }
            else
            {
                foreach (var DPRequest in DPRequests)
                {
                    Response.Add(
                        new RequestsOfPractionerMainOrgResponse
                        {
                            ReqServiceCode = DPRequest.NormalizedServiceCode,
                            NationalId = DPRequest.PractionerIdNumberNavigation.NationalId,
                            PractionerName = DPRequest.PractionerIdNumberNavigation.FullNameEn,
                            DateOfRequset = DPRequest.CreateDate.ToString(),
                            TotalWeeklyHours = Convert.ToDecimal(DPRequest.TotalWeeklyHours),
                            RequestingOrgId = DPRequest.RequestingOrg.Id,
                            RequestingOrgName = DPRequest.RequestingOrg.NameEn,
                            ResquestStatus = DPRequest.RequestStatus,
                            ApprovalEndDate = DPRequest.ToDate
                        }
                        );
                }
            }

            return new ResultOfAction<List<RequestsOfPractionerMainOrgResponse>>((int)HttpStatusCode.OK, null, Response);
        }

        public async Task<ResultOfAction<List<RequestsOfPractionerMainOrgResponse>>> GetListRequestsOfPractionerLevelTwoOrg()
        {
            List<RequestsOfPractionerMainOrgResponse> Response = new List<RequestsOfPractionerMainOrgResponse>();

            var userData = _sehaIntegrationService.GetUserData();
            var orgId = userData.RequestInfoId;
            var IsCultureArabic = _commonService.IsArabicLangHeader();

            var sehaOrganizationId = await _unitOfWork.Repository<Organization>().Get(x => x.Id == orgId).Select(x => x.OrganizationId).FirstOrDefaultAsync();

            var DPRequests = await _unitOfWork.Repository<DpRequest>().Get(x => x.MohLevel2SehaOrgId == sehaOrganizationId)
            .Where(x => x.RequestStatus != (int)RequestStatus.New && x.RequestStatus != (int)RequestStatus.Renewed && x.RequestStatus != (int)RequestStatus.Waiting_Pu_Accept1)
            .Include(x => x.RequestingOrg)
            .Include(x => x.PractionerIdNumberNavigation)
            .OrderByDescending(o => o.CreateDate)
            .ToListAsync();


            if (DPRequests.Count == 0)
            {
                _logger.LogError($"GetListRequestsOfPractionerLevelTwoOrg  There are no requests");

                return new ResultOfAction<List<RequestsOfPractionerMainOrgResponse>>((int)HttpStatusCode.NoContent, _resourceService.GetResource("There_No_Resuests"), Response);
            }

            if (IsCultureArabic)
            {
                foreach (var DPRequest in DPRequests)
                {
                    Response.Add(
                        new RequestsOfPractionerMainOrgResponse
                        {
                            ReqServiceCode = DPRequest.NormalizedServiceCode,
                            NationalId = DPRequest.PractionerIdNumberNavigation.NationalId,
                            PractionerName = DPRequest.PractionerIdNumberNavigation.FullNameAr,
                            DateOfRequset = DPRequest.CreateDate.ToString(),
                            TotalWeeklyHours = Convert.ToDecimal(DPRequest.TotalWeeklyHours),
                            RequestingOrgId = DPRequest.RequestingOrg.Id,
                            RequestingOrgName = DPRequest.RequestingOrg.NameAr,
                            ResquestStatus = DPRequest.RequestStatus,
                            ApprovalEndDate = DPRequest.ToDate
                        }
                        );
                }
            }
            else
            {
                foreach (var DPRequest in DPRequests)
                {
                    Response.Add(
                        new RequestsOfPractionerMainOrgResponse
                        {
                            ReqServiceCode = DPRequest.NormalizedServiceCode,
                            NationalId = DPRequest.PractionerIdNumberNavigation.NationalId,
                            PractionerName = DPRequest.PractionerIdNumberNavigation.FullNameEn,
                            DateOfRequset = DPRequest.CreateDate.ToString(),
                            TotalWeeklyHours = Convert.ToDecimal(DPRequest.TotalWeeklyHours),
                            RequestingOrgId = DPRequest.RequestingOrg.Id,
                            RequestingOrgName = DPRequest.RequestingOrg.NameEn,
                            ResquestStatus = DPRequest.RequestStatus,
                            ApprovalEndDate = DPRequest.ToDate
                        }
                        );
                }
            }

            return new ResultOfAction<List<RequestsOfPractionerMainOrgResponse>>((int)HttpStatusCode.OK, null, Response);

        }

        public async Task<ResultOfAction<DefaultModel>> CancelRequestByGovOrg(RequestServiceCode requestServiceCode)
        {
            DateTime LicToDate;
            var DpRequest = await _unitOfWork.Repository<DpRequest>().Get(x => x.NormalizedServiceCode == requestServiceCode.ReqServiceCode)
               .Include(x => x.PractionerIdNumberNavigation).Include(x => x.PractionerMainOrg).Include(x => x.RequestingOrg).FirstOrDefaultAsync();

            var IsArabic = _commonService.IsArabicLangHeader();

            if (DpRequest == null)
            {
                _logger.LogError($"CancelRequestByGovOrg - Request Number {requestServiceCode.ReqServiceCode} Not Found");
                throw new CustomHttpException(HttpStatusCode.BadRequest, _resourceService.GetResource("This_Request_Is_Not_Found"));
            }

            if (DpRequest.RequestStatus != (int)RequestStatus.Approved)
            {
                _logger.LogError($"CancelRequestByGovOrg - Request Status Must Be Approved");
                throw new CustomHttpException(HttpStatusCode.BadRequest, _resourceService.GetResource("Request_Status_Not_Approved"));
            }

            if (DpRequest.ToDate != null)
            {
                LicToDate = DpRequest.ToDate.Value;
                var RemainingDays = (LicToDate).Subtract(DateTime.Now).TotalDays;
                if (RemainingDays < 30)
                {
                    _logger.LogError($"CancelRequestByGovOrg - the approval cannot be canceled, because the remaining days for the approval to end are less than 30 days");
                    throw new CustomHttpException(HttpStatusCode.BadRequest, _resourceService.GetResource("Approval_Is_Less_Than_30_Days"));
                }
            }

            var user = _sehaIntegrationService.GetUserData();

            _logger.LogInformation("*********CancelRequestByGovOrg Get User Info From Seha = (" + user.UserId + ")", user.UserId);

            try
            {
                DpRequest.RequestStatus = (int)RequestStatus.Cancelled_Pu2;
                DpRequest.ToDate = DateTime.Now.AddDays(30);
                DpRequest.UpdatedBy = user.UserId;
                DpRequest.UpdateDate = DateTime.Now;

                _unitOfWork.Repository<DpRequest>().Update(DpRequest);
                await _unitOfWork.SaveChangesAsync();

                var CanceledReqDate = DpRequest.ToDate?.ToString("yyyy-MM-dd");
                try
                {
                    var RolePerId = _configuration["UserPermissionId:AddDPReq"];
                    _logger.LogInformation("********* Get Role Permission Of the User = (" + RolePerId + ")", RolePerId);
                    UsersInfoRequest usersInfoRequest = new UsersInfoRequest
                    {
                        PermissionId = int.Parse(RolePerId),
                        OrganizationId = DpRequest.RequestingOrg.OrganizationId,
                        ServiceId = 47
                    };
                    _logger.LogInformation("********* Get users Info Request Of the User to SendMail = (" + usersInfoRequest.OrganizationId + ")", usersInfoRequest.OrganizationId);
                    EmailContent emailContent = new EmailContent
                    {
                        RequestId = DpRequest.NormalizedServiceCode,
                        EmailSubjectOrg = _resourceService.GetResource("Email_Subject_Cancel_Request_By_GovOrg"),
                        EmailBody = $"<div dir='rtl' lang='ar'> نفيدكم بأنه تم إلغاء موافقة الممارس {DpRequest.PractionerIdNumberNavigation.FullNameAr}. <br> من قبل المنشأة الحكومية {DpRequest.PractionerMainOrg.NameAr}. <br> علما بأن الإلغاء سيتم  بعد 30 يوم  الموافق {CanceledReqDate}. <br> مع التحية. </div>",
                        Contents = File.ReadAllText(_commonService.MapPath(@"StaticFiles/SendOrganizationNewRequest/SendOrganisationNewReqEmailHtmlBody.html")),
                    };
                    _logger.LogInformation("********* Get Email Content Request Of the User to SendMail = (" + emailContent.EmailSubjectOrg + " **** " + emailContent.RequestId + ")", usersInfoRequest.OrganizationId);
                    await _emailService.SendMailOrg(usersInfoRequest, emailContent);
                }
                catch (Exception e)
                {
                    _logger.LogError($"Exception Message : {e}");
                    _logger.LogError($"CancelRequestByPrivateOrg - SendUserEmail - Failed to send User Email, RequestId : {DpRequest.NormalizedServiceCode}");
                    return new ResultOfAction<DefaultModel>((int)HttpStatusCode.OK, _resourceService.GetResource("Request_Canceled"), new DefaultModel());
                }


                // Send Anat Notification
                var PractionerName = DpRequest.PractionerIdNumberNavigation.FullNameEn;
                var SendNotificationTitle = "Canceled Request By The Government Establishment ";
                var SendNotificationBody = $"Dear {PractionerName}. {Environment.NewLine} We would like to inform you that Khabeer Alseha request No.: {DpRequest.NormalizedServiceCode} has been canceled by the government health establishment. {Environment.NewLine} We wish you all the best. ";
                var SMSTxtBody = $"Dear {PractionerName}. {Environment.NewLine} We would like to inform you that Khabeer Alseha request No.: {DpRequest.NormalizedServiceCode} has been canceled by the government health establishment. {Environment.NewLine} You can view the details of the request and take the necessary action through Anat application link: https://anat.page.link/job-marketplace";

                if (IsArabic)
                {
                    PractionerName = DpRequest.PractionerIdNumberNavigation.FullNameAr;
                    SendNotificationTitle = "طلب ملغى من المنشأه الحكوميه";
                    SendNotificationBody = $" عزيزى {PractionerName}. {Environment.NewLine} نود التنويه أنه تم الغاء طلب خبير الصحة رقم: {DpRequest.NormalizedServiceCode}، من قِبل المنشأة الصحية الحكومية. {Environment.NewLine} متمنين لكم دوام التوفيق. ";
                    SMSTxtBody = $" عزيزى {PractionerName}. {Environment.NewLine} نود التنويه أنه تم الغاء طلب خبير الصحة رقم: {DpRequest.NormalizedServiceCode}، من قِبل المنشأة الصحية الحكومية. {Environment.NewLine} بإمكانك الاطلاع على تفاصيل الطلب واتخاذ الاجراء اللازم عن طريق تطبيق أناة على الرابط:https://anat.page.link/job-marketplace";
                }

                var NewRequestNotification = new NotificationToAnatRequest
                {
                    ServiceName = _resourceService.GetResource("ServiceName"),
                    Title = SendNotificationTitle,
                    Body = SendNotificationBody,
                    NumberId = new List<string> { DpRequest.PractionerIdNumberNavigation.NationalId },
                    requestInfo = new RequestInfo
                    {
                        Type = "dual_practice",
                        DualPractice = new DualPractice
                        {
                            RequestId = DpRequest.NormalizedServiceCode
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
            catch (Exception e)
            {
                _logger.LogError($"Exception Message : {e}");
                _logger.LogError($"CancelRequestByPrivateOrg : Cancel Request Failed ({requestServiceCode.ReqServiceCode})");

                throw new CustomHttpException(HttpStatusCode.BadRequest, _resourceService.GetResource("Cancel_Request_Failed"));
            }
        }
    }
}
