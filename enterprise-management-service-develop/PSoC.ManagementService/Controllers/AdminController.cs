using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;

using PSoC.ManagementService.Core;
using PSoC.ManagementService.Core.Extensions;
using PSoC.ManagementService.Models;
using PSoC.ManagementService.Responses;
using PSoC.ManagementService.Services.Interfaces;
using PSoC.ManagementService.Services.Logging;
using PSoC.ManagementService.Services.Models;

namespace PSoC.ManagementService.Controllers
{
    [Authorize(Roles = "GlobalAdmin, DistrictAdmin, SchoolAdmin")]
    public class AdminController : Controller
    {
        private readonly IDeviceService _deviceService;

        #region UserData

        private bool IsAuthenticated
        {
            get { return (User != null && User.Identity != null && User.Identity.IsAuthenticated); }
        }

        private ClaimsIdentity UserIdentity
        {
            get
            {
                if (IsAuthenticated) return (ClaimsIdentity) User.Identity;
                var ex = new Exception("User not authenticated or failed to retrieve identity");
                PEMSEventSource.Log.ApplicationException(ex.Message);
                throw ex;
            }
        }

        private AdminType UserType
        {
            get
            {
                var role = UserIdentity.FindFirst(ClaimTypes.Role);
                if (role != null && !string.IsNullOrEmpty(role.Value)) return Enum<AdminType>.Parse(role.Value);
                var ex = new Exception("User Role not found");
                PEMSEventSource.Log.ApplicationException(ex.Message);
                throw ex;
            }
        }

        private string Username
        {
            get
            {
                var username = UserIdentity.FindFirst(ClaimTypes.Name);
                if (username != null && !string.IsNullOrEmpty(username.Value)) return username.Value;
                var ex = new Exception("Username not found");
                PEMSEventSource.Log.ApplicationException(ex.Message);
                throw ex;
            }
        }

        private Guid DistrictId
        {
            get
            {
                Guid districtId;
                var district = UserIdentity.FindFirst(x => x.Type == "District");
                if ((district == null) || (!Guid.TryParse(district.Value, out districtId)))
                {
                    var ex = new Exception("District Id not found");
                    PEMSEventSource.Log.ApplicationException(ex.Message);
                    throw ex;
                }
                return districtId;
            }
        }

        private Guid SchoolId
        {
            get
            {
                Guid schoolId;
                var school = UserIdentity.FindFirst(x => x.Type == "School");
                if ((school == null) || (!Guid.TryParse(school.Value, out schoolId)))
                {
                    var ex = new Exception("School Id not found");
                    PEMSEventSource.Log.ApplicationException(ex.Message);
                    throw ex;
                }
                return schoolId;
            }
        }

        #endregion

        public AdminController(IDeviceService deviceService)
        {
            _deviceService = deviceService;
        }

        public ActionResult Index()
        {
            ViewBag.IsAuthenticated = IsAuthenticated;
            ViewBag.Username = Username;
            ViewBag.UserType = UserType;

            return View();
        }

        /// <summary>
        /// Invoked as AJAX call
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> Dashboard(DataTablePageRequestModel jQueryDataTablesModel)
        {
           if (jQueryDataTablesModel == null || jQueryDataTablesModel.DisplayLength <= 0)
            {
                jQueryDataTablesModel = new DataTablePageRequestModel()
                {
                    DisplayStart = 0,   // Index (based on list before filtering) of the first Record on the page
                    DisplayLength = 10, // Page Size (# of Records Per Page)
                    Echo = 1,
                };
            }

            Guid? institutionEntityId = null;
            switch (UserType)
            {
                case AdminType.DistrictAdmin:
                    institutionEntityId = DistrictId;
                    break;
                case AdminType.SchoolAdmin:
                    institutionEntityId = SchoolId;
                    break;
            }
                            
            var result = await _deviceService.GetAccessPointDeviceStatusAsync(
                                                                            UserType,
                                                                            institutionEntityId,
                                                                            jQueryDataTablesModel.DisplayLength,
                                                                            jQueryDataTablesModel.DisplayStart,
                                                                            null).ConfigureAwait(false);

            return Json(new DataTablesResponse<AccessPointDeviceStatus>(result.Item1,
                                                                        result.Item2,
                                                                        result.Item2,
                                                                        jQueryDataTablesModel.Echo), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> AddEditCustomer(string environmentId)
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> AddEditCustomer()
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("Index", "Admin"); 
            }

            return View();
        }

        public async Task<ActionResult> DeleteCustomer(string environmentId)
        {
            return RedirectToAction("Index", "Admin"); 
        }

    }
}