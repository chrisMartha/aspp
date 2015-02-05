using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;
using PSoC.ManagementService.Models;
using PSoC.ManagementService.Services.Interfaces;
using PSoC.ManagementService.Services.Models;
using PSoC.ManagementService.Services.Logging;

namespace PSoC.ManagementService.Controllers
{
    [Authorize(Roles = "GlobalAdmin, DistrictAdmin, SchoolAdmin, Customer")]
    public class HomeController : Controller
    {
        private readonly ICustomerService _customerService;

        public HomeController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        public ActionResult Index()
        {
            ViewBag.Title = "Managed Devices";
            ViewBag.BodyClass = "ManagedDevices";
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> Settings()
        {
            var customer = await ExtractCustomerFromClaim().ConfigureAwait(false);

            var vm = new SettingsViewModel 
            {
                DownloadLicenseTimeout = customer.DownloadLicenseTimeout,
                MaxDownloadsPerClass = customer.MaxDownloadsPerClass,
                MaxDownloadsPerCustomer = customer.MaxDownloadsPerCustomer
            };

            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Settings(SettingsViewModel model)
        {
            if (ModelState.IsValid)
            {
                var customer = await ExtractCustomerFromClaim().ConfigureAwait(false);

                customer.DownloadLicenseTimeout = model.DownloadLicenseTimeout;
                customer.MaxDownloadsPerClass = model.MaxDownloadsPerClass;
                customer.MaxDownloadsPerCustomer = model.MaxDownloadsPerCustomer;

                await _customerService.UpdateCustomerAsync(customer).ConfigureAwait(false);

                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }

            
        private async Task<Customer> ExtractCustomerFromClaim()
        {
            var claimsId = (ClaimsIdentity)User.Identity;

            var primaryEnvironmentId = claimsId.Claims.SingleOrDefault(x => x.Type == "PrimaryEnvironmentId");
            if (primaryEnvironmentId == null)
            {
                    var ex = new Exception("Primary Enviornment Id Claim not found");
                    PEMSEventSource.Log.ApplicationException(ex.Message);
                    throw ex;
            }

            var district = claimsId.Claims.SingleOrDefault(x => x.Type == "District");
            if (district == null)
            {
                    var ex = new Exception("District Claim not found");
                    PEMSEventSource.Log.ApplicationException(ex.Message);
                    throw ex;
            }

            var customer = await _customerService.GetCustomerByPrimaryCustomerIdAsync(primaryEnvironmentId.Value, district.Value).ConfigureAwait(false);
            if (customer == null)
            {
                    var ex = new Exception(string.Format("Could not retrieve customer for district {0} and id {1}", district.Value, primaryEnvironmentId.Value));
                    PEMSEventSource.Log.ApplicationException(ex.Message);
                    throw ex;
            }
            return customer;
        }
    }
}
