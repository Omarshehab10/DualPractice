using Common.Localization;
using Common.Types;
using DAL.Models;
using DTO.DTOs.Parameters;
using DTO.DTOs.Responses;
using Lean.Framework.Entities.Integration;
using Lean.Framework.Entities.Provider;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Services.ExternalServices.SehaEndPoint;
using Services.UnitOfWork;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Services.PaymentService
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISehaIntegrationService _sehaIntegrationService;
        private readonly ILogger<PaymentService> _logger;
        private readonly IAppResourceService _resourceService;
        private readonly IConfiguration _configuration;
        private readonly ISehaService _sehaService;

        public PaymentService(
            IServiceProvider serviceProvider,
            IUnitOfWork unitOfWork,
            ILogger<PaymentService> logger,
            IAppResourceService resourceService,
            IConfiguration configuration,
            ISehaService sehaService)
        {
            _unitOfWork = unitOfWork;
            _sehaIntegrationService = serviceProvider.GetRequiredService<ISehaIntegrationService>();
            _logger = logger;
            _resourceService = resourceService;
            _configuration = configuration;
            _sehaService = sehaService;
        }
        public async Task<ResultOfAction<ServicePointsResponse>> GetServicePoints(string serviceCode)
        {
            //var points = await _sehaIntegrationService.PaymentGetServicePoint();
            var DpRequest = await _unitOfWork.Repository<DpRequest>().Get(x => x.NormalizedServiceCode == serviceCode).Select(x => new { x.Duration }).FirstOrDefaultAsync();
            if (DpRequest == null)
            {
                
                _logger.LogError($"GetServicePoints - This Request Is Not Found");
                throw new CustomHttpException(HttpStatusCode.BadRequest, _resourceService.GetResource("This_Request_Is_Not_Found"));
            }
            // Get ServiceProccessId
            var serviceProccessId = _configuration["SehaIntegrationConfig:PrivateServiceProcessId_6Months"];
            if (DpRequest.Duration == 12)
            {
                serviceProccessId = _configuration["SehaIntegrationConfig:PrivateServiceProcessId_12Months"];
            }

            var userToken = _sehaIntegrationService.GetUserData().AccessToken;
            var response = await _sehaService.PaymentGetServicePoint(serviceProccessId, userToken);
            if (response.error == true || response.update_token == true)
            {
                _logger.LogError($"GetServicePoints - error while calling Seha to get the Service Points");
                throw new CustomHttpException(HttpStatusCode.BadRequest, response.error_description);
            }
            return new ResultOfAction<ServicePointsResponse>((int)HttpStatusCode.OK, null, response);
        }

        public async Task<ResultOfAction<PaymentTransactionDetail>> DeductPoints(DpRequest dpRequest)
        {
            // Get ServiceProccessId
            var serviceProccessId = _configuration["SehaIntegrationConfig:PrivateServiceProcessId_6Months"];
            if (dpRequest.Duration == 12)
            {
                serviceProccessId = _configuration["SehaIntegrationConfig:PrivateServiceProcessId_12Months"];
            }

            #region Check valid Balance
            var userToken = _sehaIntegrationService.GetUserData().AccessToken;
            var inquireResponse = await _sehaService.InquireBalance(new InquireBalanceRequest { access_token = userToken , service_processid = serviceProccessId });

            if (inquireResponse.valid_balance <= 0)
            {
                _logger.LogError($"InquireBalance - Balance not enough");
                throw new CustomHttpException(HttpStatusCode.BadRequest, _resourceService.GetResource("Balance_Not_Enough"));
            }
            if (!inquireResponse.success)
            {
                _logger.LogError($"InquireBalance - error while calling Seha to Inquire Balance. Error Message : " + inquireResponse.error_description);
                throw new CustomHttpException(HttpStatusCode.BadRequest, $"Seha Error Message : {inquireResponse.error_description}");
            }
            #endregion



            // Deduct Balance

            _logger.LogInformation("********* DeductPoints, service Proccess Id sent to SEHA = (" + serviceProccessId + ")", serviceProccessId);

            _logger.LogInformation("********* DeductPoints, Normalized Service Code sent to SEHA = (" + dpRequest.NormalizedServiceCode + ")", dpRequest.NormalizedServiceCode);

            var deductPointsResponse =await _sehaIntegrationService.PaymentDeductingBalance(dpRequest.NormalizedServiceCode, serviceProccessId);



            if (_configuration["deductPoints:IsTest"].ToLower() == "true")
            {

                deductPointsResponse = new AppResponse<DeductingBalanceApiResponse>();
                var Data = new DeductingBalanceApiResponse();

                deductPointsResponse.Data = Data;
                deductPointsResponse.Data.TransactionId = 106289;
                deductPointsResponse.Data.TransactionCode = "DUPDUPP2306120001";
                deductPointsResponse.Data.TransactionId = 10;
                deductPointsResponse.Data.Success = true;
            }



            if (!deductPointsResponse.Data.Success)
            {
                _logger.LogError($"DeductPoints - error while calling Seha to Deduct Points. Error Message : " + deductPointsResponse.ErrorMessage);
                throw new CustomHttpException(HttpStatusCode.BadRequest, $"Seha Error Message : {deductPointsResponse.ErrorCode} , {deductPointsResponse.ErrorMessage}");
            }

            var user = _sehaIntegrationService.GetUserData();

            var paymentTransaction = new PaymentTransactionDetail
            {
                DprequestId = dpRequest.Id,
                TransactionId = deductPointsResponse.Data.TransactionId,
                TransactionCode = deductPointsResponse.Data.TransactionCode,
                ServiceCode = dpRequest.NormalizedServiceCode,
                DpprocessId = Convert.ToInt32(serviceProccessId),
                DeductedPoints = deductPointsResponse.Data.Consumed,
                CreatedBy = user.UserId,
                CreateDate = DateTime.Now
            };

            dpRequest.PaymentTransactionDetail.Add(paymentTransaction);
            await _unitOfWork.SaveChangesAsync();

            return new ResultOfAction<PaymentTransactionDetail>((int)HttpStatusCode.OK, null, paymentTransaction);
        }       
    }
}
