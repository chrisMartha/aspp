using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;
using PSoC.ManagementService.Models;
using PSoC.ManagementService.Services.Interfaces;
using PSoC.ManagementService.Services.Logging;
using PSoC.ManagementService.Services.Models;

namespace PSoC.ManagementService.Controllers
{
    [Authorize(Roles = "GlobalAdmin, DistrictAdmin, SchoolAdmin")]
    public class SettingsController : Controller
    {
        private Admin _admin;
        private readonly IAccessPointService _accessPointService;
        private readonly IDistrictService _districtService;

        public SettingsController(IAccessPointService accessPointService, IDistrictService districtService)
        {
            _accessPointService = accessPointService;
            _districtService = districtService;
        }


        /// <summary>
        /// Get access point settings
        /// </summary>
        /// <param name="wifiBSSId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> AccessPoint(String wifiBSSId)
        {
            var vm = await CreateViewModel(wifiBSSId).ConfigureAwait(false);

            if (vm.SelectedAccessPoint != null)
            {
                vm.WifiBSSId = vm.SelectedAccessPoint.WifiBSSId;
                vm.AccessPointMaxDownloadLicenses = vm.SelectedAccessPoint.AccessPointMaxDownloadLicenses;
                vm.AccessPointExpiryTimeSeconds = vm.SelectedAccessPoint.AccessPointExpiryTimeSeconds.GetValueOrDefault();
            }
            return View(vm);
        }

        /// <summary>
        /// Update access point settings
        /// </summary>
        /// <param name="accessPointVm"></param>
        /// <returns></returns>
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> AccessPoint(AccessPointSettingsViewModel accessPointVm)
        {
            var vm = await CreateViewModel(accessPointVm.WifiBSSId).ConfigureAwait(false);

            if (vm.SelectedAccessPoint == null)
            {
                ModelState.AddModelError(string.Empty, "No matching access point is available.");
                return View(vm);
            }

            if (ModelState.IsValid)
            {
                vm.SelectedAccessPoint.WifiBSSId = accessPointVm.WifiBSSId;
                vm.SelectedAccessPoint.AccessPointMaxDownloadLicenses = accessPointVm.AccessPointMaxDownloadLicenses;
                vm.SelectedAccessPoint.AccessPointExpiryTimeSeconds = accessPointVm.AccessPointExpiryTimeSeconds;
                await _accessPointService.UpdateAsync(_admin.Username, vm.SelectedAccessPoint).ConfigureAwait(false);
                vm.Message = "Access point settings updated successfully.";
            }

            return View(vm);
        }

        /// <summary>
        /// Get district settings
        /// </summary>
        /// <param name="districtId"></param>
        /// <returns></returns>
        [HttpGet, Authorize(Roles = "GlobalAdmin, DistrictAdmin")]
        public async Task<ActionResult> District(Guid? districtId)
        {
            var vm = await CreateViewModel(districtId).ConfigureAwait(false);

            if (vm.SelectedDistrict != null)
            {
                vm.DistrictId = vm.SelectedDistrict.DistrictId;
                vm.DistrictMaxDownloadLicenses = vm.SelectedDistrict.DistrictMaxDownloadLicenses;
                vm.DistrictLicenseExpirySeconds = vm.SelectedDistrict.DistrictLicenseExpirySeconds;
            }
            return View(vm);
        }

        /// <summary>
        /// Update district settings
        /// </summary>
        /// <param name="districtVm"></param>
        /// <returns></returns>
        [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "GlobalAdmin, DistrictAdmin")]
        public async Task<ActionResult> District(DistrictSettingsViewModel districtVm)
        {
            var vm = await CreateViewModel(districtVm.DistrictId).ConfigureAwait(false);

            if (vm.SelectedDistrict == null)
            {
                ModelState.AddModelError(string.Empty, "No matching district is available.");
                return View(vm);
            }

            if (ModelState.IsValid)
            {
                vm.SelectedDistrict.DistrictId = districtVm.DistrictId.GetValueOrDefault();
                vm.SelectedDistrict.DistrictMaxDownloadLicenses = districtVm.DistrictMaxDownloadLicenses;
                vm.SelectedDistrict.DistrictLicenseExpirySeconds = districtVm.DistrictLicenseExpirySeconds;
                await _districtService.UpdateAsync(_admin.Username, vm.SelectedDistrict).ConfigureAwait(false);
                vm.Message = "District settings updated successfully.";
            }

            return View(vm);
        }

        /// <summary>
        /// Extract claim for user information
        /// </summary>
        /// <returns></returns>
        private void ExtractClaim()
        {
            var claimsId = (ClaimsIdentity)User.Identity;
            var claims = claimsId.Claims.ToList();
            _admin = new Admin { Username = claimsId.Name };

            // Check required claim for district admins
            if (User.IsInRole("DistrictAdmin"))
            {
                Guid districtId;
                var district = claims.SingleOrDefault(x => x.Type == "District");
                if ((district == null) || (!Guid.TryParse(district.Value, out districtId)))
                {
                    var ex = new Exception("District Claim not found");
                    PEMSEventSource.Log.ApplicationException(ex.Message);
                    throw ex;
                }
                _admin.DistrictId = districtId;
            }

            // Check required claim for school admins
            if (User.IsInRole("SchoolAdmin"))
            {
                Guid schoolId;
                var district = claims.SingleOrDefault(x => x.Type == "School");
                if ((district == null) || (!Guid.TryParse(district.Value, out schoolId)))
                {
                    var ex = new Exception("School Claim not found");
                    PEMSEventSource.Log.ApplicationException(ex.Message);
                    throw ex;
                }
                _admin.SchoolId = schoolId;
            }
        }

        /// <summary>
        /// Create an access point view model
        /// </summary>
        /// <param name="wifiBSSId"></param>
        /// <returns></returns>
        private async Task<AccessPointSettingsViewModel> CreateViewModel(String wifiBSSId)
        {
            ExtractClaim();

            var accessPoints = (await _accessPointService.GetAsync(_admin.Username).ConfigureAwait(false)).OrderBy(ap => ap.WifiBSSId).ToList();
            return new AccessPointSettingsViewModel(accessPoints, wifiBSSId);
        }

        /// <summary>
        /// Create a district setting view model
        /// </summary>
        /// <param name="districtId"></param>
        /// <returns></returns>
        private async Task<DistrictSettingsViewModel> CreateViewModel(Guid? districtId)
        {
            ExtractClaim();

            var districts = (await _districtService.GetAsync(_admin.Username).ConfigureAwait(false)).OrderBy(d => d.DistrictName).ToList();
            return new DistrictSettingsViewModel(districts, districtId);
        }
    }
}