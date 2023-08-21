using DTO.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Integration
{
    public interface IIntegrationService
    {
        IntegrationModel GetIntegrationInfo(string key, string secret);
        IntegrationModel GetBearerIntegrationInfo(string token);
    }
}
