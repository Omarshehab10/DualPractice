using Common.Types;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;

namespace DualPractitionerBE.Controllers
{
    /// <summary>
    /// ErrorController
    /// </summary>
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorController : ControllerBase
    {
        private readonly ILogger<ErrorController> _logger;
        private readonly IWebHostEnvironment _webHost;

        /// <summary>
        /// ErrorController
        /// </summary>
        public ErrorController(ILogger<ErrorController> logger, IWebHostEnvironment webHost)
        {
            _logger = logger;
            _webHost = webHost;
        }

        /// <summary>
        /// Error
        /// </summary>
        [Route("error")]
        public ResultOfAction<object> Error()
        {
            _logger.LogError("Start Logging The Error");
            _logger.LogError("------------------------");

            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var exception = context.Error;

            _logger.LogError($"The Actual Error Details Are {exception.Message}");

            string defaultMessage = exception.Message;

            //if (_webHost.IsProduction())
            //{
            //    defaultMessage = "There is an issue with request, please check the logs";
            //}

            var result = new ResultOfAction<object>((int)HttpStatusCode.InternalServerError, defaultMessage, null);

            if (exception is CustomHttpException)
            {
                var customHttpException = (CustomHttpException)exception;

                result.StatusCode = customHttpException.StatusCode;
                result.Message = customHttpException.Message;
            }

            Response.StatusCode = result.StatusCode;

            _logger.LogError("------------------------");
            _logger.LogError("End Logging The Error");

            return result;
        }
    }
}
