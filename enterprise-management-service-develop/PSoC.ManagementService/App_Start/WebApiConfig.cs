using PSoC.ManagementService.Models;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Tracing;
using PSoC.ManagementService.Services.Logging;

namespace PSoC.ManagementService
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
            config.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new UnixDateTimeConverter());
    
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/v1/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // Add trace writer
            GlobalConfiguration.Configuration.Services.Replace(typeof(ITraceWriter), new NLogger("TraceWriter"));
        }
    }
}
