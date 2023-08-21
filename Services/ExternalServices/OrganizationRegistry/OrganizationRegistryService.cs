using Common.Localization;
using Common.Types;
using DTO.DTOs.OrganizationRegistry;
using DTO.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Services.ExternalServices.OrganizationRegistry
{
    public class OrganizationRegistryService : BaseHttpService.BaseHttpService, IOrganizationRegistryService
    {
        private readonly string _serviceUrl;
        private readonly string _serviceUrl2;
        private readonly string _schema;
        private readonly string _token;
        private readonly IAppResourceService _resourceService;

        public OrganizationRegistryService(
            IHttpClientFactory httpClientFactory,
            IHostEnvironment environment,
            ILogger<BaseHttpService.BaseHttpService> logger,
            IConfiguration configuration,
            IAppResourceService resourceService) : base(httpClientFactory, logger)
        {
            _serviceUrl = configuration["OrganizationRegistry:Url"];
            _serviceUrl2 = configuration["OrganizationRegistryV2:Url"];
            _schema = configuration["OrganizationRegistry:Schema"];
            _token = configuration["OrganizationRegistry:Token"];
            _resourceService = resourceService;
        }
        public async Task<OrganizationData> GetOrganizationData(string parameter)
        {
            var response = await MakeHttpRequestAsync(
               string.Empty, $"{_serviceUrl.TrimEnd('/')}/{parameter.Trim()}", new IntegrationModel { Scheme = _schema, Token = _token },
               true, "profile");

            if (!response.isSuccessStatusCode)
            {
                throw new CustomHttpException(response.statusCode, response.message);
            }
            var organizationRegistryResponse = new OrganizationRegistryResponse();
            try
            {
                organizationRegistryResponse = JsonConvert.DeserializeObject<OrganizationRegistryResponse>(response.content);
            }
            catch (Exception e)
            {
                throw new CustomHttpException(HttpStatusCode.BadRequest, "Exception when Deserialize Object, Exception Message : " + e);
            }
            return organizationRegistryResponse.Data;
        }

        public async Task<OrganizationDataV2> GetOrganizationDataV2(string parameter)
        {
            var response = await MakeHttpRequestAsync(
               string.Empty, $"{_serviceUrl2.TrimEnd('/')}/{parameter.Trim()}", new IntegrationModel { Scheme = _schema, Token = _token },
               true, "profile");

            if (!response.isSuccessStatusCode)
            {
                throw new CustomHttpException(response.statusCode, response.message);
            }
            var organizationRegistryV2Response = new OrganizationRegistryV2Response();
            try
            {
                organizationRegistryV2Response = JsonConvert.DeserializeObject<OrganizationRegistryV2Response>(response.content);
            }
            catch (Exception e)
            {
                throw new CustomHttpException(HttpStatusCode.BadRequest, "Exception when Deserialize Object, Exception Message : " + e);
            }
            return organizationRegistryV2Response.Data;
        }
    }
}
