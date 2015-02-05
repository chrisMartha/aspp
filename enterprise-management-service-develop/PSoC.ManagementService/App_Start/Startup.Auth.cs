using System;
using System.Configuration;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;

namespace PSoC.ManagementService
{
    public partial class Startup
    {
        /// <summary>
        /// Default Authentication Timeout/Session Expiration In Minutes
        /// </summary>
        public static Double AuthenticationTimeOut
        {
            get
            {
                double sessionTimeOut;
                return (Double.TryParse(ConfigurationManager.AppSettings["DefaultAuthenticationTimeOutInMinutes"],
                    out sessionTimeOut) && sessionTimeOut > 0)
                    ? sessionTimeOut
                    : 30;
            }
        }

        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                ExpireTimeSpan = TimeSpan.FromMinutes(AuthenticationTimeOut),
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                LogoutPath = new PathString("/Account/Logout")
            });
            
        }
    }
}