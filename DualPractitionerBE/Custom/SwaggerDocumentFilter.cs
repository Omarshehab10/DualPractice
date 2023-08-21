using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;


namespace DualPractitionerBE.Custom
{
    public class SwaggerDocumentFilter : IDocumentFilter
    {
        private readonly string _swaggerDocHost;
        public SwaggerDocumentFilter(IHttpContextAccessor httpContextAccessor)
        {
            var host = httpContextAccessor.HttpContext.Request.Host.Value;

            var scheme = httpContextAccessor.HttpContext.Request.Scheme;

            _swaggerDocHost = $"https://{host}";
        }


        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            swaggerDoc.Servers.Add(new OpenApiServer { Url = _swaggerDocHost });
        }
    }
}
