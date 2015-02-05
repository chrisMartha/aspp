using System;
using Microsoft.Practices.Unity;
using Locator = Microsoft.Practices.ServiceLocation;
using PSoC.ManagementService.Services;
using PSoC.ManagementService.Services.Interfaces;
using PSoC.ManagementService.Data.Interfaces;
using PSoC.ManagementService.Data.Repositories;

namespace PSoC.ManagementService
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public class UnityConfig
    {
        #region Unity Container
        private static Lazy<IUnityContainer> container = new Lazy<IUnityContainer>(() =>
        {
            var container = new UnityContainer();
            RegisterTypes(container);
            return container;
        });

        /// <summary>
        /// Gets the configured Unity container.
        /// </summary>
        public static IUnityContainer GetConfiguredContainer()
        {
            return container.Value;
        }
        #endregion

        /// <summary>Registers the type mappings with the Unity container.</summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>There is no need to register concrete types such as controllers or API controllers (unless you want to 
        /// change the defaults), as Unity allows resolving a concrete type even if it was not previously registered.</remarks>
        public static void RegisterTypes(IUnityContainer container)
        {
            // NOTE: To load from web.config uncomment the line below. Make sure to add a Microsoft.Practices.Unity.Configuration to the using statements.
            // container.LoadConfiguration();

            // TODO: Register your types here
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
            container.RegisterType<IAzureTableService, AzureTableService>();
            container.RegisterType<ICustomerService, CustomerService>();
            container.RegisterType<IDeviceService, DeviceService>();
            container.RegisterType<IDistrictService, DistrictService>();
            container.RegisterType<ISchoolService, SchoolService>();
            container.RegisterType<ILicenseService, LicenseService>();
            container.RegisterType<ISchoolnetService, SchoolnetService>();
            container.RegisterType<IUserService, UserService>();
            container.RegisterType<IAccessPointDeviceStatusRepository, AccessPointDeviceStatusRepository>();

            var locator = new UnityServiceLocator(container);
            Locator.ServiceLocator.SetLocatorProvider(() => locator);
            
        }
    }
}
