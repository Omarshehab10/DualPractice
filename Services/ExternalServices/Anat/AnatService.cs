using Common.Localization;
using Common.Types;
using DTO.DTOs.Parameters;
using DTO.DTOs.Responses;
using DTO.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;


namespace Services.ExternalServices.Anat
{
    public class AnatService : BaseHttpService.BaseHttpService, IAnatService
    {
        private readonly string _serviceUrl;
        private readonly string _schema;
        private readonly string _token;
        private readonly HttpClient _httpClient;
        private readonly IAppResourceService _resourceService;
        private readonly ILogger<BaseHttpService.BaseHttpService> _logger;

        public AnatService(
            IHttpClientFactory httpClientFactory,
            IHostEnvironment environment,
            ILogger<BaseHttpService.BaseHttpService> logger,
            IConfiguration configuration,
            IAppResourceService resourceService) : base(httpClientFactory, logger)
        {
            _serviceUrl = configuration["AnatIntegration:Url"];
            _schema = configuration["AnatIntegration:Schema"];
            _token = configuration["AnatIntegration:Token"];
            _resourceService = resourceService;
            _httpClient = httpClientFactory.CreateClient();
            _logger = logger;
        }
        

        public async Task<NotificationToAnatResponse> SendNotificationToAnat(NotificationToAnatRequest notificationToAnatRequest)
        {
            var url = $"{_serviceUrl.TrimEnd()}";
            string jsonObject = JsonConvert.SerializeObject(notificationToAnatRequest);

            var response = await MakeHttpRequestAsync(
              jsonObject, url, new IntegrationModel { Scheme = _schema, Token = _token },
                false, "SendNotificationToAnat");


            if (!response.isSuccessStatusCode)
            {
                _logger.LogError($"SendNotificationToAnat Not Successful StatusCode, Response statusCode : {response.statusCode} , Response Message : {response.message}");
                throw new CustomHttpException((int)response.statusCode, $"SendNotificationToAnat Not Successful StatusCode, Response statusCode : {response.statusCode} , Response Message : {response.message}");
            }
            var notificationToAnatResponse = new NotificationToAnatResponse();
            try
            {
                notificationToAnatResponse = JsonConvert.DeserializeObject<NotificationToAnatResponse>(response.content);
            }
            catch (Exception e)
            {
                _logger.LogError($"SendNotificationToAnat Exception - Exception Message : {e}");
                throw new CustomHttpException(HttpStatusCode.BadRequest, "Exception when Deserialize Object, Exception Message : " + e);
            }

            return notificationToAnatResponse;

        }
    }
}
