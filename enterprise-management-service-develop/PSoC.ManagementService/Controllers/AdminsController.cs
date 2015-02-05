using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;

using PSoC.ManagementService.Services.Interfaces;
using PSoC.ManagementService.Services.Models;

namespace PSoC.ManagementService.Controllers
{
    [Authorize(Roles = "GlobalAdmin")]
    public class AdminsController : Controller
    {
        private readonly IAdminService _adminService;

        public AdminsController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        // GET: Admins
        public async Task<ActionResult> Index()
        {
            IEnumerable<Admin> admins = await _adminService.GetAsync().ConfigureAwait(false);
            return View(admins);
        }

        // GET: Admins/Details/5
        public async Task<ActionResult> Details(Guid? userId)
        {
            if (userId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Admin admin = await _adminService.GetByIdAsync(userId.Value).ConfigureAwait(false);
            if (admin == null)
            {
                return HttpNotFound();
            }
            return View(admin);
        }

        // GET: Admins/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admins/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "AdminType,UserId,Username,UserType,Active,LastLoginDateTime,DistrictId,SchoolId,AdminEmail")] Admin admin)
        {
            await ValidateModel(admin, false).ConfigureAwait(false);
            if (ModelState.IsValid)
            {
                admin = PopulateUserType(admin);
                if (await _adminService.InsertAsync(admin).ConfigureAwait(false))
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Could not create Admin");
                }
            }

            return View("Create", admin);
        }

        // GET: Admins/Edit/5
        public async Task<ActionResult> Edit(Guid? userId)
        {
            if (userId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Admin admin = await _adminService.GetByIdAsync(userId.Value).ConfigureAwait(false);
            if (admin == null)
            {
                return HttpNotFound();
            }
            return View(admin);
        }

        // POST: Admins/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "AdminType,UserId,Username,UserType,Active,LastLoginDateTime,DistrictId,SchoolId,AdminEmail")] Admin admin)
        {
            await ValidateModel(admin, true).ConfigureAwait(false);
            if (ModelState.IsValid)
            {
                admin = PopulateUserType(admin);
                if (await _adminService.UpdateAsync(admin).ConfigureAwait(false))
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Could not update Admin");
                }
            }

            return View("Edit", admin);
        }

        // POST: Admins/Delete/ea4efd7f-34b4-44d4-a3ef-0e90dd9798d3
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(Guid? userId)
        {
            if (userId.HasValue && await _adminService.DeleteAsync(userId.Value).ConfigureAwait(false))
            {
                return Content(Boolean.TrueString);
            }
            else
            {
                return Content(Boolean.FalseString);
            }
        }

        public Admin PopulateUserType(Admin model)
        {
            // Not a business requirement, however this is what the current base load uses for data values.
            switch(model.AdminType)
            {
                case Core.AdminType.DistrictAdmin:
                    model.UserType = "District Admin";
                    break;
                case Core.AdminType.GlobalAdmin:
                    model.UserType = "Global Admin";
                    break;
                case Core.AdminType.SchoolAdmin:
                    model.UserType = "School Admin";
                    break;
            }

            return model;
        }

        public async Task ValidateModel(Admin model, bool isEdit)
        {
            if((int)model.AdminType ==0)
            {
                ModelState.AddModelError("AdminType", "Admin type is a required field");
            }
            else if(model.AdminType == Core.AdminType.DistrictAdmin && model.DistrictId == null)
            {
                ModelState.AddModelError("DistrictId", "District is required for District Admin");
            }
            else if (model.AdminType == Core.AdminType.SchoolAdmin && model.SchoolId == null)
            {
                ModelState.AddModelError("SchoolId", "School is required for School Admin");
            }

            if (!string.IsNullOrEmpty(model.Username))
            {
                if (isEdit) // Edit admin screen
                {
                    Admin adminByName = await _adminService.GetByUsernameAsync(model.Username).ConfigureAwait(false);
                    if (adminByName != null && adminByName.UserId != model.UserId)
                    {
                        ModelState.AddModelError("Username", string.Format("Existing user with id {0} already has username {1}", adminByName.UserId, adminByName.Username));
                    }
                }
                else // Create admin screen
                {
                    Admin adminById = await _adminService.GetByIdAsync(model.UserId).ConfigureAwait(false);
                    if (adminById != null)
                    {
                        ModelState.AddModelError("UserId", string.Format("User with id {0} already exists with username {1}", adminById.UserId, adminById.Username));
                    }
                    Admin adminByName = await _adminService.GetByUsernameAsync(model.Username).ConfigureAwait(false);
                    if (adminByName != null)
                    {
                        ModelState.AddModelError("Username", string.Format("Existing user with id {0} already has username {1}", adminByName.UserId, adminByName.Username));
                    }
                }
            }
        }
    }
}
