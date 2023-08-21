using AutoMapper;
using Common.Enums;
using Common.Localization;
using Common.Types;
using DAL.Models;
using DTO.DTOs;
using DTO.DTOs.EmailSetting;
using DTO.DTOs.Parameters;
using DTO.DTOs.Responses;
using Lean.Framework.Entities.Integration;
using Lean.Framework.Entities.Provider;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Services.Common;
using Services.ExternalServices.Anat;
using Services.ExternalServices.SendEmailService;
using Services.PaymentService;
using Services.S3Storage;
using Services.UnitOfWork;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Services.PrivateOrganization
{
    public class PrivateOrganizationService : IPrivateOrganizationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PrivateOrganizationService> _logger;
        private readonly IAppResourceService _resourceService;
        private readonly IPaymentService _paymentService;
        private readonly ISehaIntegrationService _sehaIntegrationService;
        private readonly IMapper _mapper;
        private readonly IS3StorageService _storageService;
        private readonly IPdfService _pdfService;
        private readonly IAnatService _anatService;
        private readonly IConfiguration _configuration;
        private readonly ICommonService _commonService;
        private readonly IEmailService _emailService;



        public PrivateOrganizationService(
            IUnitOfWork unitOfWork, ILogger<PrivateOrganizationService> logger,
            IAppResourceService resourceService, IPaymentService paymentService, 
            IServiceProvider serviceProvider, IMapper mapper, 
            IS3StorageService storageService, IPdfService pdfService,
             IAnatService anatService, IConfiguration configuration ,
            ICommonService commonService, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _resourceService = resourceService;
            _paymentService = paymentService;
            _sehaIntegrationService = serviceProvider.GetRequiredService<ISehaIntegrationService>();
            _mapper = mapper;
            _storageService = storageService;
            _pdfService = pdfService;
            _anatService = anatService;
            _configuration = configuration;
            _commonService = commonService;
            _emailService = emailService;
        }
        public async Task<ResultOfAction<DefaultModel>> AcceptOrCancelRequest(GetRequestInfo getRequestInfo)
        {
            // Check Request Status
            var DpRequest = await _unitOfWork.Repository<DpRequest>().Get(x => x.NormalizedServiceCode == getRequestInfo.RequestServiceCode)
                .Include(x => x.PractionerIdNumberNavigation).FirstOrDefaultAsync();
            var IsArabic = _commonService.IsArabicLangHeader();

            if (DpRequest.RequestStatus != (int)RequestStatus.Waiting_Pr_Pay_2)
            {
                _logger.LogError($"AcceptOrCancelRequest - Request Status Must Be Waiting for Payment");
                throw new CustomHttpException(HttpStatusCode.BadRequest, _resourceService.GetResource("Request_Status_Not_Waiting_Payment"));
            }

            var user = _sehaIntegrationService.GetUserData();
            _logger.LogInformation("********* Get User Info From Seha = (" + user.UserId + ")", user.UserId);

            AddLogRequest addLogRequest = new AddLogRequest();
            addLogRequest.NormalizedServiceCode = DpRequest.NormalizedServiceCode;
            addLogRequest.UserId = user.UserId;

            var PractionerName = DpRequest.PractionerIdNumberNavigation.FullNameEn;
            var SendNotificationTitle = "Accepted Request";
            var SendNotificationBody = $"Dear {PractionerName}. {Environment.NewLine} Your request to work in the private sector outside official working hours has been successfully approved through Khabeer Alseha Service. {Environment.NewLine} We would like to point out that Khabeer Alseha approval on its own is not sufficient for the government practitioner to work in the private sector, in addition a work license must be issued through Health licensing system. {Environment.NewLine} Thank you for your cooperation.";
            var SMSTxtBody = $" Dear {PractionerName}.{Environment.NewLine} Your request to work in the private sector outside official working hours has been successfully approved through Khabeer Alseha Service. {Environment.NewLine} We would like to point out that Khabeer Alseha approval on its own is not sufficient for the government practitioner to work in the private sector, in addition a work license must be issued through Health licensing system. {Environment.NewLine} You can view the details of the request and take the necessary action through Anat application link: https://anat.page.link/job-marketplace";

            // Request Rejected
            if (getRequestInfo.Approval == false)
            {
                DpRequest.RequestStatus = (int)RequestStatus.Rejected_Pr2;
                DpRequest.Comment.Add(new Comment
                {
                    Text = getRequestInfo.Comment,
                    DprequestId = DpRequest.Id,
                    CreatedBy = user.UserId,
                    CreateDate = DateTime.Now
                });
                DpRequest.UpdatedBy = user.UserId;
                DpRequest.UpdateDate = DateTime.Now;

                _unitOfWork.Repository<DpRequest>().Update(DpRequest);
                await _unitOfWork.SaveChangesAsync();

                //Add Action Log
                addLogRequest.ActionType = AddLogs.Private_Est_Reject_with_Payment;
                await _commonService.AddActionRequestLog(addLogRequest);


                // Send Anat Notification
                SendNotificationTitle = "Request Is Rejected By Private Establishment";
                SendNotificationBody = $" Dear {PractionerName}. {Environment.NewLine} We would like to inform you that Khabeer Alseha request No.: {DpRequest.NormalizedServiceCode} was rejected by the private health establishment. {Environment.NewLine} We wish you all the best. ";
                SMSTxtBody = $" Dear {PractionerName}.{Environment.NewLine} We would like to inform you that Khabeer Alseha request No.: {DpRequest.NormalizedServiceCode} was rejected by the private health establishment. {Environment.NewLine} You can view the details of the request and take the necessary action through Anat application link: https://anat.page.link/job-marketplace";
                
                if (IsArabic)
                {
                    PractionerName = DpRequest.PractionerIdNumberNavigation.FullNameAr;
                    SendNotificationTitle = "الطلب مرفوض من قبل المنشأة الخاصه";
                    SendNotificationBody = $" عزيزى {PractionerName}. {Environment.NewLine} نود التنويه أنه تم رفض طلب خبير الصحة رقم: {DpRequest.NormalizedServiceCode}، من قِبل المنشأة الصحية الخاصة. {Environment.NewLine} متمنين لكم دوام التوفيق. ";
                    SMSTxtBody = $" عزيزى {PractionerName}. {Environment.NewLine} نود التنويه أنه تم رفض طلب خبير الصحة رقم: {DpRequest.NormalizedServiceCode}، من قِبل المنشأة الصحية الخاصة. {Environment.NewLine} بإمكانك الاطلاع على تفاصيل الطلب واتخاذ الاجراء اللازم عن طريق تطبيق أناة على الرابط:https://anat.page.link/job-marketplace";
                }

                var RequestNotification = new NotificationToAnatRequest
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
                await _anatService.SendNotificationToAnat(RequestNotification);
                return new ResultOfAction<DefaultModel>((int)HttpStatusCode.OK, _resourceService.GetResource("Request_Rejected"), new DefaultModel());
            }

            // Request Accepted
            var paymentResult = await _paymentService.DeductPoints(DpRequest);
            if (paymentResult == null)
            {
                _logger.LogError($"AcceptOrCancelRequest - Error with Seha payment Result");
                throw new CustomHttpException(HttpStatusCode.BadRequest, "AcceptOrCancelRequest - Error with Seha payment Result");
            }

            #region Update DpRequest
            DpRequest.RequestStatus = (int)RequestStatus.Approved;
            DpRequest.FromDate = DateTime.Now;
            DpRequest.ToDate = DateTime.Now.AddMonths(DpRequest.Duration);
            DpRequest.UpdatedBy = user.UserId;
            DpRequest.UpdateDate = DateTime.Now;
            _unitOfWork.Repository<DpRequest>().Update(DpRequest);
            await _unitOfWork.SaveChangesAsync();

            #endregion
            _logger.LogInformation($"Updated in DataBase and deducted Done******** ");

            //var privateOrgReview = _mapper.Map<PrivateOrgReview>(paymentResult);
            //privateOrgReview.Accepted = true;

            //Add Action Log
            addLogRequest.ActionType = AddLogs.Private_Est_done_with_Payment;
            await _commonService.AddActionRequestLog(addLogRequest);

            // Send Anat Notification
            if (IsArabic)
            {
                PractionerName = DpRequest.PractionerIdNumberNavigation.FullNameAr;
                SendNotificationTitle = "طلب مقبول";
                SendNotificationBody = $" عزيزى  {PractionerName}. {Environment.NewLine} تم إصدار موافقة عمل في القطاع الخاص خارج أوقات الدوام الرسمي من خدمة خبير الصحة بنجاح. {Environment.NewLine} نود التنويه أن موافقة خبير الصحة الصادرة لا تكفي لعمل الممارس الحكومي في القطاع الخاص، ويجب استخراج ترخيص مزاولة مهنة من نظام التراخيص الصحية وذلك عبر التواصل مع المنشأة الخاصة. {Environment.NewLine} شاكرين لكم تعاونكم. ";
                SMSTxtBody = $" عزيزى  {PractionerName}. {Environment.NewLine} تم إصدار موافقة عمل في القطاع الخاص خارج أوقات الدوام الرسمي من خدمة خبير الصحة بنجاح. {Environment.NewLine} نود التنويه أن موافقة خبير الصحة الصادرة لا تكفي لعمل الممارس الحكومي في القطاع الخاص، ويجب استخراج ترخيص مزاولة مهنة من نظام التراخيص الصحية وذلك عبر التواصل مع المنشأة الخاصة. {Environment.NewLine} بإمكانك الاطلاع على تفاصيل الطلب واتخاذ الاجراء اللازم عن طريق تطبيق أناة على الرابط:https://anat.page.link/job-marketplace";
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
                _logger.LogError($"AcceptOrCancelRequest - SendNotificationToAnat - Failed to send Notification To Anat , NumberId : {NewRequestNotification.NumberId} , RequestId : {NewRequestNotification.requestInfo.DualPractice.RequestId}");
                return new ResultOfAction<DefaultModel>((int)HttpStatusCode.OK, _resourceService.GetResource("Request_Sent"), new DefaultModel());
            }
            return new ResultOfAction<DefaultModel>((int)HttpStatusCode.OK, null, new DefaultModel());
        }

        public async Task<ResultOfAction<GetLicenseToHLSResponse>> GetDPlicensetoHLS(ParctitionerNId parctitionerNId)
        {
            var RequestInfo = await _unitOfWork.Repository<DpRequest>().Get(x => x.PractionerIdNumber == parctitionerNId.ParctNId && x.RequestStatus == (int)RequestStatus.Approved).Include(y=>y.RequestingOrg).FirstOrDefaultAsync();

            if (RequestInfo == null)
            {
                _logger.LogError($"GetDPlicensetoHLS - This practitioner has no approved requests");

                throw new CustomHttpException(HttpStatusCode.BadRequest, _resourceService.GetResource("Practitioner_Has_No_Approved_Requests"));
            }
            GetLicenseToHLSResponse getLicenseToHLSResponse = new GetLicenseToHLSResponse
            {
                LicenseNumber = RequestInfo.NormalizedServiceCode,
                EstablismentName = RequestInfo.RequestingOrg.NameAr,
                LicenseEndDate = RequestInfo.ToDate,
                LicenseStatus = RequestInfo.RequestStatus,
                EstablishmentLicenseNumber = RequestInfo.RequestingOrg.LicenseNumber
            };
            return new ResultOfAction<GetLicenseToHLSResponse>((int)HttpStatusCode.OK, null, getLicenseToHLSResponse);

        }

        public async Task<ResultOfAction<List<RequestsOfRequestingOrgResponse>>> GetListRequestsOfRequestingOrg()
        {
            List<RequestsOfRequestingOrgResponse> Response = new List<RequestsOfRequestingOrgResponse>();

            var userData = _sehaIntegrationService.GetUserData();
            var orgId = userData.RequestInfoId;
            var IsCultureArabic = _commonService.IsArabicLangHeader();

            var DPRequests = await _unitOfWork.Repository<DpRequest>().Get(x => x.RequestingOrg.Id == orgId)
                .Include(x => x.PractionerMainOrg)
                .Include(x => x.PractionerIdNumberNavigation)
                .OrderByDescending(o => o.CreateDate)
                .ToListAsync();

            if (DPRequests.Count == 0)
            {
                return new ResultOfAction<List<RequestsOfRequestingOrgResponse>>((int)HttpStatusCode.NoContent, _resourceService.GetResource("There_No_Resuests"), Response);
            }

            if (IsCultureArabic)
            {
                foreach (var DPRequest in DPRequests)
                {
                    Response.Add(
                        new RequestsOfRequestingOrgResponse
                        {
                            ReqServiceCode = DPRequest.NormalizedServiceCode,
                            NationalId = DPRequest.PractionerIdNumberNavigation.NationalId,
                            PractionerName = DPRequest.PractionerIdNumberNavigation.FullNameAr,
                            DateOfRequset = DPRequest.CreateDate.ToString(),
                            PractionerMainOrgId = DPRequest.PractionerMainOrg.Id,
                            PractionerMainOrgName = DPRequest.PractionerMainOrg.NameAr,
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
                        new RequestsOfRequestingOrgResponse
                        {
                            ReqServiceCode = DPRequest.NormalizedServiceCode,
                            NationalId = DPRequest.PractionerIdNumberNavigation.NationalId,
                            PractionerName = DPRequest.PractionerIdNumberNavigation.FullNameEn,
                            DateOfRequset = DPRequest.CreateDate.ToString(),
                            PractionerMainOrgId = DPRequest.PractionerMainOrg.Id,
                            PractionerMainOrgName = DPRequest.PractionerMainOrg.NameEn,
                            ResquestStatus = DPRequest.RequestStatus,
                            ApprovalEndDate = DPRequest.ToDate
                        }
                        );
                }

            }


            return new ResultOfAction<List<RequestsOfRequestingOrgResponse>>((int)HttpStatusCode.OK, null, Response);
        }

        public async Task<ResultOfAction<DefaultModel>> CancelRequestByPrivateOrg(RequestServiceCode requestServiceCode)
        {

            var DpRequest = await _unitOfWork.Repository<DpRequest>().Get(x => x.NormalizedServiceCode == requestServiceCode.ReqServiceCode)
                           .Include(x => x.PractionerIdNumberNavigation).Include(x => x.PractionerMainOrg).Include(x => x.RequestingOrg).FirstOrDefaultAsync();
            var IsArabic = _commonService.IsArabicLangHeader();


            if (DpRequest == null)
            {
                _logger.LogError($"CancelRequestByPrivateOrg - Request Number {requestServiceCode.ReqServiceCode} Not Found");
                throw new CustomHttpException(HttpStatusCode.BadRequest, _resourceService.GetResource("This_Request_Is_Not_Found"));
            }

            if (DpRequest.RequestStatus != (int)RequestStatus.Approved)
            {
                _logger.LogError($"CancelRequestByPrivateOrg - Request Status Must Be Approved");
                throw new CustomHttpException(HttpStatusCode.BadRequest, _resourceService.GetResource("Request_Status_Not_Approved"));
            }

            var user = _sehaIntegrationService.GetUserData();
            _logger.LogInformation("*********CancelRequestByPrivateOrg Get User Info From Seha = (" + user.UserId + ")", user.UserId);


            try
            {
                DpRequest.RequestStatus = (int)RequestStatus.Cancelled_Pr1;
                DpRequest.ToDate = DateTime.Now;
                DpRequest.UpdatedBy = user.UserId;
                DpRequest.UpdateDate = DateTime.Now;

                try
                {
                    await _pdfService.GetDualPracticeReportUrl(DpRequest.NormalizedServiceCode);

                }
                catch (Exception e)
                {

                    _logger.LogError($"Exception Message : {e}");
                    _logger.LogError($"CancelRequestByPrivateOrg : Cancel Request Failed (GetDualPracticeReportUrl) ({requestServiceCode.ReqServiceCode})");

                    throw new CustomHttpException(HttpStatusCode.BadRequest, _resourceService.GetResource("Cancel_Request_Failed"));
                }


                _unitOfWork.Repository<DpRequest>().Update(DpRequest);
                await _unitOfWork.SaveChangesAsync();

                var CanceledReqDate = DpRequest.ToDate?.ToString("yyyy-MM-dd");
                try
                {
                    var RolePerId = _configuration["UserPermissionId:GovOrgReviewDPReq"];
                    _logger.LogInformation("********* Get Role Permission Of the User = (" + RolePerId + ")", RolePerId);
                    UsersInfoRequest usersInfoRequest = new UsersInfoRequest
                    {
                        PermissionId = int.Parse(RolePerId),
                        OrganizationId = DpRequest.PractionerMainOrg.OrganizationId,
                        ServiceId = 47
                    };
                    _logger.LogInformation("********* Get users Info Request Of the User to SendMail = (" + usersInfoRequest.OrganizationId + ")", usersInfoRequest.OrganizationId);
                    EmailContent emailContent = new EmailContent
                    {
                        RequestId = DpRequest.NormalizedServiceCode,
                        EmailSubjectOrg = _resourceService.GetResource("Email_Subject_Cancel_Request_By_PrivOrg"),
                        EmailBody = $"<div dir='rtl' lang='ar'> نفيدكم بأنه تم إلغاء موافقة الممارس  {DpRequest.PractionerIdNumberNavigation.FullNameAr}. <br> من قبل المنشأة الخاصة {DpRequest.RequestingOrg.NameAr}. <br> مع التحية. </div>",
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
                var SendNotificationTitle = "Canceled Request By Private Establishment";
                var SendNotificationBody = $"Dear {PractionerName}. {Environment.NewLine} We would like to inform you that Khabeer Alseha request No.: {DpRequest.NormalizedServiceCode} has been canceled by the private health establishment. {Environment.NewLine} We wish you all the best. ";
                var SMSTxtBody = $"Dear {PractionerName}. {Environment.NewLine} We would like to inform you that Khabeer Alseha request No.: {DpRequest.NormalizedServiceCode} has been canceled by the private health establishment. {Environment.NewLine} You can view the details of the request and take the necessary action through Anat application link: https://anat.page.link/job-marketplace";

                if (IsArabic)
                {
                    PractionerName = DpRequest.PractionerIdNumberNavigation.FullNameAr;
                    SendNotificationTitle = "طلب ملغى من المنشأه الصحيه الخاصه";
                    SendNotificationBody = $"عزيزى {PractionerName}. {Environment.NewLine} نود التنويه أنه تم الغاء طلب خبير الصحة رقم: {DpRequest.NormalizedServiceCode}، من قِبل المنشأة الصحية الخاصة. {Environment.NewLine} متمنين لكم دوام التوفيق. ";
                    SMSTxtBody = $"عزيزى {PractionerName}. {Environment.NewLine} نود التنويه أنه تم الغاء طلب خبير الصحة رقم: {DpRequest.NormalizedServiceCode}، من قِبل المنشأة الصحية الخاصة. {Environment.NewLine} بإمكانك الاطلاع على تفاصيل الطلب واتخاذ الاجراء اللازم عن طريق تطبيق أناة على الرابط:https://anat.page.link/job-marketplace";
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
