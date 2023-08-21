using Common.Localization;
using Common.Types;
using DTO.DTOs.PractitionersRegistry;
using DTO.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Services.ExternalServices.PractitionersRegistry
{
    public class PractitionersRegistryService : BaseHttpService.BaseHttpService, IPractitionersRegistryService
    {
        private readonly string _serviceUrl;
        private readonly string _schema;
        private readonly string _token;
        private readonly IAppResourceService _resourceService;
        private readonly ILogger<BaseHttpService.BaseHttpService> _logger;

        public PractitionersRegistryService(
            IHttpClientFactory httpClientFactory,
            IHostEnvironment environment,
            ILogger<BaseHttpService.BaseHttpService> logger,
            IConfiguration configuration,
            IAppResourceService resourceService) : base(httpClientFactory, logger)
        {
            _serviceUrl = configuration["PractitionersRegistry:Url"];
            _schema = configuration["PractitionersRegistry:Schema"];
            _token = configuration["PractitionersRegistry:Token"];
            _resourceService = resourceService;
            _logger = logger;
        }

        public async Task<PractitionerDataV3> GetPractitionerData(string NationalId)
        {
            var response = await MakeHttpRequestAsync(
                string.Empty, $"{_serviceUrl.TrimEnd('/')}/{NationalId.Trim()}", new IntegrationModel { Scheme = _schema, Token = _token },
                true, "profile");

            if ((!response.isSuccessStatusCode) && response.message == "Bad Request")
            {
                _logger.LogError("Practitioner Data Not Found");
                throw new CustomHttpException(response.statusCode, _resourceService.GetResource("This_Practitioner_Is_Not_Found_In_Ethaq"));
            }

            if (!response.isSuccessStatusCode)
            {
                _logger.LogError("UnSuccessful Status Code : " + response.statusCode + " , Response Message : " + response.message);
                throw new CustomHttpException(response.statusCode, response.message);
            }
            var practitionerRegistryResponse = new PractitionerRegistryResponseV1();
            try
            {
                practitionerRegistryResponse = JsonConvert.DeserializeObject<PractitionerRegistryResponseV1>(response.content);
            }
            catch (Exception e)
            {
                _logger.LogError("Exception Message : " + e.Message);
                throw new CustomHttpException(HttpStatusCode.BadRequest, "Exception when Deserialize Object, Exception Message : " + e);
            }
            return practitionerRegistryResponse.Data;
        }
    }
}
