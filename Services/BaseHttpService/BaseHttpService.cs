using DTO.Models;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Services.BaseHttpService
{
    public class BaseHttpService : IBaseHttpService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger _logger;
        public BaseHttpService(IHttpClientFactory httpClientFactory, ILogger<BaseHttpService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<(int statusCode, bool isSuccessStatusCode, string message, string content)> MakeHttpRequestAsync(string content, string url, IntegrationModel auth, bool isGet, string actionName)
        {
            _logger.LogInformation($"{actionName} - starting sending the request.");

            var client = _httpClientFactory.CreateClient();

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(auth.Scheme, auth.Token);

            //_logger.LogInformation($"{actionName} - token info is {JsonConvert.SerializeObject(auth)}");

            HttpResponseMessage response = null;

            if (isGet)
            {
                response = await client.GetAsync(url);
            }
            else
            {
                response = await client.PostAsync(url, new StringContent(content, Encoding.UTF8, "application/json"));
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            
            _logger.LogInformation($"{actionName} - response content is {responseContent}");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"{actionName} - error on response content with the following message {response.RequestMessage}, and {response.ReasonPhrase}");

                return ((int)response.StatusCode, false, response.ReasonPhrase, responseContent);
            }

            _logger.LogInformation($"{actionName} - response completed successfuly");

            return ((int)response.StatusCode, true, response.ReasonPhrase, responseContent);
        }

        public async Task<(int statusCode, bool isSuccessStatusCode, string message, string content)> MakeHttpRequestAsync(
            string content,
            string url,
            IntegrationModel auth,
            bool isGet,
            string actionName,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{actionName} - starting sending the request.");

            var client = _httpClientFactory.CreateClient();

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(auth.Scheme, auth.Token);

            //_logger.LogInformation($"{actionName} - token info is {JsonConvert.SerializeObject(auth)}");

            HttpResponseMessage response = null;

            if (isGet)
            {
                response = await client.GetAsync(url, cancellationToken);
            }
            else
            {
                response = await client.PostAsync(url, new StringContent(content, Encoding.UTF8, "application/json"), cancellationToken);
            }

            var responseContent = await response.Content.ReadAsStringAsync();

            _logger.LogInformation($"{actionName} - response content is {responseContent}");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"{actionName} - error on response content with the following message {response.RequestMessage}, and {response.ReasonPhrase}");

                return ((int)response.StatusCode, false, response.ReasonPhrase, responseContent);
            }

            _logger.LogInformation($"{actionName} - response completed successfuly");

            return ((int)response.StatusCode, true, response.ReasonPhrase, responseContent);
        }

        public async Task<(int statusCode, bool isSuccessStatusCode, string message, string content)> MakeHttpRequestAsync(
            string content,
            string url,
            IntegrationModel auth,
            bool isGet,
            string actionName,
            HttpClient client,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{actionName} - starting sending the request.");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(auth.Scheme, auth.Token);

            HttpResponseMessage response;

            if (isGet)
            {
                response = await client.GetAsync(url, cancellationToken);
            }
            else
            {
                response = await client.PostAsync(url, new StringContent(content, Encoding.UTF8, "application/json"), cancellationToken);
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogInformation($"{actionName} - response content is {responseContent}");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"{actionName} - error on response content with the following message {response.RequestMessage}, and {response.ReasonPhrase}");
                return ((int)response.StatusCode, false, response.ReasonPhrase, responseContent);
            }

            _logger.LogInformation($"{actionName} - response completed successfuly");
            return ((int)response.StatusCode, true, response.ReasonPhrase, responseContent);
        }
    }
}
