using Common.Localization;
using Common.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Net;

namespace Services
{
    public class RazorRenderer : IRazorRenderer
    {
        private readonly IRazorViewEngine _viewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<RazorRenderer> _logger;
        private readonly IAppResourceService _resourceService;

        public RazorRenderer(
            IRazorViewEngine viewEngine,
            ITempDataProvider tempDataProvider,
            IServiceProvider serviceProvider, 
            ILogger<RazorRenderer> logger,
            IAppResourceService resourceService)
        {
            _viewEngine = viewEngine;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;
            _logger = logger;
            _resourceService = resourceService;
        }

        public string RenderPartialToString<TModel>(string partialName, TModel model)
        {
            try
            {
                var actionContext = GetActionContext();
                _logger.LogInformation($"********** RenderPartialToString, actionContext : {actionContext} ******");
                var partial = FindView(actionContext, partialName);
                _logger.LogInformation($"********** RenderPartialToString, partial : {partial} ******");
                _logger.LogInformation($"********** RenderPartialToString, model & partialName : {model}*****{partialName} ******");
                using (var output = new StringWriter())
                {
                    var viewContext = new ViewContext(
                        actionContext,
                        partial,
                        new ViewDataDictionary<TModel>(
                            metadataProvider: new EmptyModelMetadataProvider(),
                            modelState: new ModelStateDictionary())
                        {
                            Model = model
                        },
                        new TempDataDictionary(
                            actionContext.HttpContext,
                            _tempDataProvider),
                        output,
                        new HtmlHelperOptions()
                    );

                    partial.RenderAsync(viewContext).ConfigureAwait(false);
                    return output.ToString();
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Error while RenderPartialToString , Exception Message : {e.Message} ");
                throw new CustomHttpException(HttpStatusCode.BadRequest, _resourceService.GetResource("Error_Generate_Pdf"));
            }
           
        }

        private IView FindView(ActionContext actionContext, string partialName)
        {
            var getPartialResult = _viewEngine.GetView(null, partialName, false);
            if (getPartialResult.Success)
            {
                return getPartialResult.View;
            }

            var findPartialResult = _viewEngine.FindView(actionContext, partialName, false);
            if (findPartialResult.Success)
            {
                return findPartialResult.View;
            }

            var searchedLocations = getPartialResult.SearchedLocations.Concat(findPartialResult.SearchedLocations);
            var errorMessage = string.Join(
                Environment.NewLine,
                new[] { $"Unable to find partial '{partialName}'. The following locations were searched:" }.Concat(searchedLocations)); ;
            throw new InvalidOperationException(errorMessage);
        }

        private ActionContext GetActionContext()
        {
            var httpContext = new DefaultHttpContext
            {
                RequestServices = _serviceProvider
            };
            return new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        }
    }
}
