using Common.Localization;
using Common.Types;
using DTO.DTOs.EstablishmentHLS;
using DTO.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Services.ExternalServices.HLS
{
    public class EstablishmentHLS : BaseHttpService.BaseHttpService, IEstablishmentHLS
    {
        private readonly string _serviceUrl;
        private readonly string _schema;
        private readonly string _token;
        private readonly HttpClient _httpClient;
        private readonly IAppResourceService _resourceService;
        private readonly ILogger<BaseHttpService.BaseHttpService> _logger;

        public EstablishmentHLS(
            IHttpClientFactory httpClientFactory,
            IHostEnvironment environment,
            ILogger<BaseHttpService.BaseHttpService> logger,
            IConfiguration configuration,
            IAppResourceService resourceService) : base(httpClientFactory, logger)
        {
            _serviceUrl = configuration["EstablishmentHLS:Url"];
            _schema = configuration["EstablishmentHLS:Schema"];
            _token = configuration["EstablishmentHLS:Token"];
            _resourceService = resourceService;
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient();
        }
        public async Task<HlsResponse> GetEstablishmentData(string licenseNumber)
        {
            var url = $"{_serviceUrl.TrimEnd('/')}?license_number={licenseNumber.Trim()}";
            var response = await MakeHttpRequestAsync(
                string.Empty, url, new IntegrationModel { Scheme = _schema, Token = _token },
                true, "profile");

            if (response.statusCode == (int)HttpStatusCode.NotFound)
            {
                _logger.LogError("HLS Data Not Found");
                throw new CustomHttpException(response.statusCode, _resourceService.GetResource("HLS_Data_Not_Found"));
            }
            if (!response.isSuccessStatusCode)
            {
                _logger.LogError("UnSuccessful Status Code : " + response.statusCode + " , Response Message : " + response.message);
                throw new CustomHttpException(response.statusCode, response.message);
            }
            var hlsResponse = new HlsResponse();
            try
            {
                hlsResponse = JsonConvert.DeserializeObject<HlsResponse>(response.content);
            }
            catch (Exception e)
            {
                _logger.LogError("Exception Message : " + e.Message);
                throw new CustomHttpException(HttpStatusCode.BadRequest, "Exception when Deserialize Object, Exception Message : " + e);
            }

            return hlsResponse;
        }
    }
}
