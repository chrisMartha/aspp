using Locator = Microsoft.Practices.ServiceLocation;
using PSoC.ManagementService.Services.Interfaces;

namespace PSoC.ManagementService.Services
{
    public static class ServiceLocator
    {

        public static IDistrictService GetDistrictService()
        {
            var service = Locator.ServiceLocator.Current.GetInstance<IDistrictService>();

            return service;
        }

        public static ISchoolService GetSchoolService()
        {
            var service = Locator.ServiceLocator.Current.GetInstance<ISchoolService>();
            return service;
        }
    }
}
