using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Microsoft.WindowsAzure;

using PSoC.ManagementService.Core;
using PSoC.ManagementService.Models;
using PSoC.ManagementService.Services.Interfaces;
using PSoC.ManagementService.Services.Logging;
using PSoC.ManagementService.Services.Models;

namespace PSoC.ManagementService.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly IAdminService _adminService;
        private readonly IDistrictService _districtService;
        private readonly ISchoolnetService _schoolnetService;

        public AccountController(IAdminService adminService, ISchoolnetService schoolnetService,
            IDistrictService districtService)
        {
            _adminService = adminService;
            _districtService = districtService;
            _schoolnetService = schoolnetService;
        }

        IAuthenticationManager Authentication
        {
            get { return HttpContext.GetOwinContext().Authentication; }
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Login()
        {
            var loginModel = new LoginModel();
            return View("Login", loginModel);
        }

        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                // Is the user super-user, i.e. someone who can add administrators without authenticating through SchoolNet?
                if (model.Username == "FrenchToastMafia" && model.Password == "Pe@rs0n!")
                {
                    string authenticationBypass = CloudConfigurationManager.GetSetting("AuthenticationBypassEnabled");
                    bool authenticationBypassEnabled;
                    if (bool.TryParse(authenticationBypass, out authenticationBypassEnabled))
                    {
                        // Is authentication bypass for superuser enabled?
                        if (authenticationBypassEnabled)
                        {
                            Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

                            Claim[] claims = {
                                new Claim(ClaimTypes.Name, model.Username),
                                new Claim(ClaimTypes.Sid, Guid.NewGuid().ToString()),
                                new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", model.Username),
                                new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", model.Username),
                                new Claim(ClaimTypes.Role, AdminType.GlobalAdmin.ToString())
                            };
                            ClaimsIdentity identity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie, ClaimTypes.Name, ClaimTypes.Role);

                            AuthenticationProperties authenticationProperties = new AuthenticationProperties { IsPersistent = true };
                            Authentication.SignIn(authenticationProperties, identity);

                            return RedirectToAction("Create", "Admins");
                        }
                    }
                }
                
                bool isAuthorized = false;
                var admin = await _adminService.GetByUsernameAsync(model.Username).ConfigureAwait(false);

                if (admin == null || !admin.Active)
                {
                    PEMSEventSource.Log.AccountLoginFailed(admin != null
                        ? string.Format("Not an active user {0}", admin.UserId)
                        : "Not a whitelisted user"); // Can't save sensitive data in model.Username
                    ModelState.AddModelError("LogOnError", "Unauthorized to access this site.");
                    return View("Login", model);
                }

                var userId = admin.UserId;
                bool isGlobalAdmin = (admin.AdminType == AdminType.GlobalAdmin);
                bool isSchoolAdmin = (admin.AdminType == AdminType.SchoolAdmin);

                /*  TODO - School Admin should make use of SchoolService implementation. DistrictService has an authorization check
                    that will only work for DistrictAdmin and above, so the following is a Quick HACK for now. 
                    This allows school admins to be able to login. To be removed eventually*/

                if (isGlobalAdmin || isSchoolAdmin)
                {
                    isAuthorized = await _schoolnetService.IsAuthorizedAsync(
                        GlobalAppSettings.GetValue("OAuthUrl"),
                        GlobalAppSettings.GetValue("OAuthClientId"),
                        GlobalAppSettings.GetValue("OAuthApplicationId"),
                        model.Username,
                        model.Password).ConfigureAwait(false);
                }
                else
                {
                    if (admin.DistrictId.HasValue)
                    {
                        District district = await _districtService.GetByIdAsync(model.Username, admin.DistrictId.Value).ConfigureAwait(false);
                        if (district == null)
                        {
                            PEMSEventSource.Log.AccountLoginFailed(
                                string.Format("Unable to retrieve district information. for user: {0}", userId));
                            ModelState.AddModelError("ApplicationError",
                                "An unexpected critical error has occured. Please contact the site administrator.");
                            return View("Login", model);
                        }

                        isAuthorized =
                            await _schoolnetService.IsAuthorizedAsync(
                                district.OAuthUrl,
                                district.OAuthClientId,
                                district.OAuthApplicationId,
                                model.Username,
                                model.Password).ConfigureAwait(false);
                    }
                }

                if (isAuthorized)
                {
                    Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                    await _adminService.UpdateLastLoginDateTime(userId, DateTime.UtcNow).ConfigureAwait(false);
                    ClaimsIdentity identity;

                    if (isGlobalAdmin)
                    {
                        identity = new ClaimsIdentity(new[]
                        {
                            new Claim(ClaimTypes.Name, model.Username),
                            new Claim(ClaimTypes.Sid, userId.ToString()),
                            new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier",
                                model.Username),
                            new Claim(
                                "http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider",
                                model.Username),
                            new Claim(ClaimTypes.Role, admin.AdminType.ToString())
                        },
                            DefaultAuthenticationTypes.ApplicationCookie,
                            ClaimTypes.Name, ClaimTypes.Role);
                    }
                    else
                    {
                        identity = new ClaimsIdentity(new[]
                        {
                            new Claim(ClaimTypes.Name, model.Username),
                            new Claim(ClaimTypes.Sid, userId.ToString()), 
                            new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier",
                                model.Username),
                            new Claim(
                                "http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider",
                                model.Username),
                            new Claim(ClaimTypes.Role, admin.AdminType.ToString()),
                            new Claim("District",
                                admin.DistrictId.HasValue ? admin.DistrictId.Value.ToString() : string.Empty),
                            new Claim("School", admin.SchoolId.HasValue ? admin.SchoolId.Value.ToString() : string.Empty),
                            new Claim(ClaimTypes.Role, admin.AdminType.ToString())
                        },
                            DefaultAuthenticationTypes.ApplicationCookie,
                            ClaimTypes.Name, ClaimTypes.Role);
                    }

                    Authentication.SignIn(new AuthenticationProperties
                    {
                        IsPersistent = true
                    }, identity);

                    return RedirectToAction("Index", "Admin");
                }
                ModelState.AddModelError("LogOnError", "Failed to authorize with Schoolnet.");
                PEMSEventSource.Log.AccountLoginFailed(string.Format("Failed to authorize with Schoolnet for user {0}", userId));
                return View("Login", model);
            }
            else
            {
                ModelState.AddModelError("LogOnError", "The user name or password provided is incorrect.");
                return View("Login", model);
            }
        }

        [HttpGet]
        public ActionResult Logout()
        {
            Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Login");
        }

    }
}