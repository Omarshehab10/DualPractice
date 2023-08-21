using Common.Localization;
using Common.Types;
using DTO.DTOs.Parameters;
using DTO.DTOs.Responses;
using DTO.DTOs.SehaIntegration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Lean.Framework.Entities.Provider;
using Lean.Framework.Entities.Integration;
using System.Text;
using Services.BaseHttpService;
using DTO.Models;



namespace Services.ExternalServices.SehaEndPoint
{
    public class SehaService :  ISehaService 
    {
        private readonly IAppResourceService resourceService;
        private readonly ILogger<ISehaService> logger;
        private readonly IConfiguration configuration;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ISehaIntegrationService sehaIntegrationService;
        private readonly IBaseHttpService baseHttpService;
        private readonly string _schema;
        private readonly string _token;


        public SehaService(IAppResourceService _resourceService,
            ILogger<ISehaService> _logger,
            IConfiguration _configuration, 
            IHttpClientFactory _httpClientFactory, 
            ISehaIntegrationService _sehaIntegrationService,
            IBaseHttpService _baseHttpService)
        {
            resourceService = _resourceService;
            logger = _logger;
            configuration = _configuration; 
            httpClientFactory = _httpClientFactory;
            sehaIntegrationService = _sehaIntegrationService;
            baseHttpService = _baseHttpService;
            _schema = configuration["GetOrganizationUsersByPermission:Schema"];
            _token = configuration["GetOrganizationUsersByPermission:Token"];
        }

        public async Task<ServicePointsResponse> PaymentGetServicePoint(string ServiceProcessId, string accessToken)
        {
            var client = httpClientFactory.CreateClient();

            string url = configuration["SehaIntegration:BaseUrl:OAuth"] + configuration["SehaIntegration:PathUrl:ServicePoints"];

            var formContent = new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>("access_token", accessToken),
                new KeyValuePair<string, string>("service_processid", ServiceProcessId)
            });

            var response = await client.PostAsync(url, formContent);

            var contentResponse = await response.Content.ReadAsStringAsync();

            if (contentResponse.Contains("\"error\": \"true\""))
            {
                logger.LogError($" GetDeductPoints - error while calling Seha Get Service Point Endpoint, Seha contentResponse : {contentResponse}");
                throw new CustomHttpException(HttpStatusCode.BadRequest, resourceService.GetResource("Seha_ServicePoints_Error"));
            }

            if (!response.IsSuccessStatusCode)
            {
                logger.LogError($" GetDeductPoints- error while calling Seha Get Service Point Endpoint , Seha contentResponse : {contentResponse}");
                throw new CustomHttpException(HttpStatusCode.BadRequest, resourceService.GetResource("Seha_ServicePoints_Error"));
            }
            var servicePointsResponse = new ServicePointsResponse();
            try
            {
                servicePointsResponse = JsonConvert.DeserializeObject<ServicePointsResponse>(contentResponse);
            }
            catch (Exception e)
            {
                logger.LogError($"Exception Message : {e.Message}");
                throw new CustomHttpException(HttpStatusCode.BadRequest, "Exception when Deserialize Object, Exception Message : " + e);
            }
            return servicePointsResponse;
        }

        public async Task<MedicalOrgSubCategoryResponse> GetMedicalOrganizationSubCategory(string accessToken)
        {
            var client = httpClientFactory.CreateClient();

            string url = configuration["SehaIntegration:BaseUrl:OAuth"] + configuration["SehaIntegration:PathUrl:MedicalOrganizationSubCategory"];

            var formContent = new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>("access_token", accessToken)
            });

            var response = await client.PostAsync(url, formContent);

            var contentResponse = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                logger.LogError($"DeductBalance - error while calling Seha Deduct Points Endpoint");
                throw new CustomHttpException(HttpStatusCode.BadRequest, "Error - Not Successful Status Code");
            }
            var medicalOrgSubCategoryResponse = new MedicalOrgSubCategoryResponse();
            try
            {
                medicalOrgSubCategoryResponse = JsonConvert.DeserializeObject<MedicalOrgSubCategoryResponse>(contentResponse);
            }
            catch (Exception e)
            {
                throw new CustomHttpException(HttpStatusCode.BadRequest, "Exception when Deserialize Object, Exception Message : " + e);
            }
            return medicalOrgSubCategoryResponse;
        }

        public async Task<OrganizationDataResponse> GetOrganizationData(string accessToken)
        {
            var client = httpClientFactory.CreateClient();

            string url = configuration["SehaIntegration:BaseUrl:OAuth"] + configuration["SehaIntegration:PathUrl:Organization"];

            var formContent = new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>("access_token", accessToken)
            });

            var response = await client.PostAsync(url, formContent);

            var contentResponse = await response.Content.ReadAsStringAsync();

            if (contentResponse.Contains("\"error\": \"true\""))
            {
                logger.LogError($"GetOrganizationData - Seha contentResponse : {contentResponse}");
                throw new CustomHttpException(HttpStatusCode.BadRequest, resourceService.GetResource("Seha_GetOrganizationData_Error"));
            }

            if (!response.IsSuccessStatusCode)
            {
                logger.LogError($"GetOrganizationData - Seha contentResponse : {contentResponse}");
                throw new CustomHttpException(HttpStatusCode.BadRequest, resourceService.GetResource("Seha_GetOrganizationData_Error"));
            }
            var organizationDataResponse = new OrganizationDataResponse();
            try
            {
                organizationDataResponse = JsonConvert.DeserializeObject<OrganizationDataResponse>(contentResponse);
            }
            catch (Exception e)
            {
                logger.LogError($"Exception Message : {e.Message}");
                throw new CustomHttpException(HttpStatusCode.BadRequest, "Exception when Deserialize Object, Exception Message : " + e);
            }
            return organizationDataResponse;
        }

        public async Task<InquireBalanceResponse> InquireBalance(InquireBalanceRequest request)
        {
            var client = httpClientFactory.CreateClient();

            string url = configuration["SehaIntegration:BaseUrl:BaseUrl"] + configuration["SehaIntegration:PathUrl:InquireBalance"];

            var formContent = new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>("access_token", request.access_token),
                new KeyValuePair<string, string>("service_processid", request.service_processid)
            });

            var response = await client.PostAsync(url, formContent);

            var contentResponse = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                logger.LogError($"InquireBalance - error while calling Seha Deduct Points Endpoint , Seha contentResponse : {contentResponse}");
                throw new CustomHttpException(response.StatusCode, resourceService.GetResource("Seha_InquireBalance_Error"));
            }
            var inquireBalanceResponse = new InquireBalanceResponse();
            try
            {
                inquireBalanceResponse = JsonConvert.DeserializeObject<InquireBalanceResponse>(contentResponse); 
            }
            catch (Exception e)
            {
                logger.LogError($"Exception Message : {e.Message}");
                throw new CustomHttpException(HttpStatusCode.BadRequest, "Exception when Deserialize Object, Exception Message : " + e);
            }
            return inquireBalanceResponse;
        }

        public async Task<UsersInfoResponse> GetUsersInfo(UsersInfoRequest usersInfoRequest)
        {

            UsersInfoResponse EndpointResponse = new UsersInfoResponse();
            try
            {
                string url = configuration["GetOrganizationUsersByPermission:Url"];
                string jsonObject = JsonConvert.SerializeObject(usersInfoRequest);

                var response = await baseHttpService.MakeHttpRequestAsync(
                  jsonObject, url, new IntegrationModel { Scheme = _schema, Token = _token },
                    false, "GetUsersInfo");


                if (!response.isSuccessStatusCode)
                {
                    logger.LogError(" GetUsersInfo - error while calling Get Organization Users By Permission Endpoint , Seha contentResponse : " + response.message);
                    throw new CustomHttpException(HttpStatusCode.BadRequest, "An error occurred when connecting to Seha:  " + response.message);
                }
                 EndpointResponse = JsonConvert.DeserializeObject<UsersInfoResponse>(response.content);

            }
            catch (Exception e)
            {
                logger.LogError($"Exception Message : {e.Message}");
                throw new CustomHttpException(HttpStatusCode.BadRequest, "Exception when Deserialize Object, Exception Message : " + e);

            }
            return EndpointResponse;
        }
    }
}
