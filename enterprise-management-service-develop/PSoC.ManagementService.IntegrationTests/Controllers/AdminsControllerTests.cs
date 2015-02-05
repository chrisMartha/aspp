using System;
using System.Threading.Tasks;
using System.Web.Mvc;

using Locator = Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using PSoC.ManagementService.Controllers;
using PSoC.ManagementService.Core;
using PSoC.ManagementService.Data.Interfaces;
using PSoC.ManagementService.Data.Repositories;
using PSoC.ManagementService.Models;
using PSoC.ManagementService.Services;
using PSoC.ManagementService.Services.Interfaces;
using PSoC.ManagementService.Services.Models;

namespace PSoC.ManagementService.IntegrationTests.Controllers
{
    [TestClass]
    public class AdminsControllerTests
    {
        private AdminsController _sut;
        private AdminService _adminService;

        [TestInitialize]
        public void TestInitialize()
        {
            var container = new UnityContainer();
            container.RegisterType<IConfigBase, ConfigBase>();
            container.RegisterType<IHttpClientFactory, HttpClientFactory>();
            // Data
            container.RegisterType<IAccessPointRepository, AccessPointRepository>();
            container.RegisterType<IAdminRepository, AdminRepository>();
            container.RegisterType<IDeviceRepository, DeviceRepository>();
            container.RegisterType<IDeviceInstalledCourseRepository, DeviceInstalledCourseRepository>();
            container.RegisterType<IDistrictRepository, DistrictRepository>();
            container.RegisterType<ISchoolRepository, SchoolRepository>();
            container.RegisterType<ILicenseRepository, LicenseRepository>();
            container.RegisterType<IUserRepository, UserRepository>();
            container.RegisterType<IUnitOfWork, UnitOfWork>();
            // Service
            container.RegisterType<IAccessPointService, AccessPointService>();
            container.RegisterType<IAdminService, AdminService>();
            container.RegisterType<ICustomerService, CustomerService>();
            container.RegisterType<IDeviceService, DeviceService>();
            container.RegisterType<IDistrictService, DistrictService>();
            container.RegisterType<ISchoolService, SchoolService>();
            container.RegisterType<ILicenseService, LicenseService>();
            container.RegisterType<ISchoolnetService, SchoolnetService>();
            container.RegisterType<IUserService, UserService>();

            var locator = new UnityServiceLocator(container);
            Locator.ServiceLocator.SetLocatorProvider(() => locator);

            _adminService = container.Resolve<AdminService>();
            _sut = new AdminsController(_adminService);
        }

        [TestMethod]
        public async Task AdminController_Edit()
        {
            //Arrange
            var userId = Guid.Parse("c89ccc4e-edb6-43c9-9dbe-e46cdb845fae");

            //Act
            var result = await _sut.Edit(userId).ConfigureAwait(false) as ViewResult;

            //Assert
            Assert.IsNotNull(result);
        }
    }
}
