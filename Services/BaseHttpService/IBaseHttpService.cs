using DTO.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Services.BaseHttpService
{
    public interface IBaseHttpService
    {
        Task<(int statusCode, bool isSuccessStatusCode, string message, string content)> MakeHttpRequestAsync(string content, string url, IntegrationModel auth, bool isGet, string actionName);
    }
}
