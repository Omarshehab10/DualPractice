using DTO.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Integration
{
    public class IntegrationService : IIntegrationService
    {
        public IntegrationModel GetIntegrationInfo(string key, string secret)
        {
            var bytes = Encoding.UTF8.GetBytes(key + ":" + secret);

            return new IntegrationModel() { Token = Convert.ToBase64String(bytes), Scheme = "Basic" };
        }

        public IntegrationModel GetBearerIntegrationInfo(string token)
        {
            return new IntegrationModel() { Token = token, Scheme = "Bearer" };
        }
    }
}
