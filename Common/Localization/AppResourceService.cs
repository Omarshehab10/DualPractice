using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Localization
{
    public class AppResourceService : IAppResourceService
    {
        private readonly IStringLocalizer<SharedResources> _localizer;

        public AppResourceService(IStringLocalizer<SharedResources> localizer)
        {
            _localizer = localizer;
        }

        public string GetResource(string key)
        {
            return _localizer[key];
        }
    }
}
