using System;
using System.Net.Http.Formatting;
using System.Security.Claims;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

using PSoC.ManagementService.ModelBinder;
using PSoC.ManagementService.Models;
using PSoC.ManagementService.Services.Logging;

namespace PSoC.ManagementService
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            GlobalConfiguration.Configuration.Formatters.Clear();
            GlobalConfiguration.Configuration.Formatters.Add(new JsonMediaTypeFormatter());

            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.Name;

            ModelBinders.Binders.Add(typeof(DataTablePageRequestModel), new DataTablesModelBinder());

            PEMSEventSource.Log.ApplicationStart("Application starting...");
            var enabled = PEMSEventSource.Log.ConfigureLogAll();
            PEMSEventSource.Log.ApplicationConfigureLog("Application enabling Log All (" + enabled + ")...");
            PEMSEventSource.Log.PingLog();
        }

        void Application_Error(Object sender, EventArgs e)
        {
            var exception = Server.GetLastError();
            if (exception == null)
            {
                return;
            }

            var current = HttpContext.Current;
            var url = (current != null) ? current.Request.Url.ToString() : null;
            PEMSEventSource.Log.ApplicationException(exception.Message, new LogRequest { Exception = exception, Url = url });
        }
    }
}
