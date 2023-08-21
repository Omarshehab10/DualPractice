using Lean.Framework.Entities;
using Lean.Framework.Entities.Provider;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Services.IdentityServices
{
    [DependencyService(ServiceLifetimeEnum.Scoped)]
    public abstract class DualPracticeServiceAbstract<TEntity> : IDualPracticeService
    {
        protected IServiceProvider ServiceProvider { get; }
        protected ILogger Logger { get; }
        public DualPracticeServiceAbstract(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            Logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger(GetType());

        }

        public int CurrentUserId
        {
            get
            {

                var sehaIntegrationService = ServiceProvider.GetRequiredService<ISehaIntegrationService>();
                var userData = sehaIntegrationService.GetUserData();
                if (userData != null)
                {
                    return userData.UserId;
                }

                Logger.LogError($"userData is NULL - Get CurrentUserId Exception");
                throw new Exception("userData is NULL - Get CurrentUserId Exception");

            }
        }

        public int UserOrganizationId
        {
            get
            {

                var sehaIntegrationService = ServiceProvider.GetRequiredService<ISehaIntegrationService>();
                var userData = sehaIntegrationService.GetUserData();
                if (userData != null)
                {
                    return userData.RequestInfoId;
                }

                Logger.LogError($"userData is NULL - Get UserOrganizationId Exception");
                throw new Exception("userData is NULL - Get UserOrganizationId Exception");

            }
        }
    }
}
