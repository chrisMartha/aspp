using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using Microsoft.WindowsAzure.ServiceRuntime;
using PSoC.ManagementService.Data.Interfaces;
using PSoC.ManagementService.Data.Repositories;
using PSoC.ManagementService.Services;
using PSoC.ManagementService.Services.Interfaces;
using PSoC.ManagementService.Services.Logging;

namespace PSoC.ManagementService.LicenseTimer
{
    public class WorkerRole : RoleEntryPoint
    {
        private const string LicenseCleanupIntervalConfigurationSettingName = "LicenseCleanupInterval";
        private IUnityContainer _container;
        private readonly CancellationTokenSource _cancelSource = new CancellationTokenSource();
        private ILicenseService _licenseService;

        public override bool OnStart()
        {
            PEMSEventSource.Log.TimeoutServiceStarted("Worker role starting...");
            _container = new UnityContainer();
            RegisterTypes();

            ServicePointManager.DefaultConnectionLimit = 12;

            RoleEnvironment.Changing += RoleEnvironmentChanging; 

            var enabled = PEMSEventSource.Log.ConfigureLogAll();
            PEMSEventSource.Log.TimeoutServiceConfigureLog("Worker role enabling Log All (" + enabled + ")...");
            PEMSEventSource.Log.PingLog();

            return base.OnStart();
        }

        private void RegisterTypes()
        {
            _container.RegisterInstance(_container);

            _container.RegisterType<IUnitOfWork, UnitOfWork>();
            _container.RegisterType<IAzureTableService, AzureTableService>();
            _container.RegisterType<ICustomerService, CustomerService>();
            _container.RegisterType<ILicenseService, LicenseService>();
            _container.RegisterType<ILicenseRepository, LicenseRepository>();
            _container.RegisterType<IDeviceService, DeviceService>();
            _container.RegisterType<IConfigBase, ConfigBase>();
            _container.RegisterType<IDeviceInstalledCourseRepository, DeviceInstalledCourseRepository>();

            _licenseService = _container.Resolve<LicenseService>();
        }

        public override void OnStop()
        {
            PEMSEventSource.Log.TimeoutServiceShuttingDown("Worker role shutting down...");
            _cancelSource.Cancel();

            base.OnStop();
        }

        /// <summary>
        /// Event triggered when the role environment is changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RoleEnvironmentChanging(object sender, RoleEnvironmentChangingEventArgs e)
        {
            // Force the role to go offline and restart for log setting change
            var change = e.Changes.OfType<RoleEnvironmentConfigurationSettingChange>().FirstOrDefault(c => c.ConfigurationSettingName == PEMSEventSource.LogAllSettingName);
            if (change != null)
            {
                e.Cancel = true;
            }
        }

        public override void Run()
        {
            RoleEnvironment.Changed += RoleEnvironment_Changed;

            // This must never exit or the worker role will die and restart
            RunAsync().Wait();
        }

        private async Task RunAsync()
        {
            int interval;
            if (!int.TryParse(RoleEnvironment.GetConfigurationSettingValue(LicenseCleanupIntervalConfigurationSettingName), out interval))
            {
                PEMSEventSource.Log.TimeoutServiceException("Couldn't find configuration setting LicenseCleanupInterval. Using default of 300 seconds (every 5 minutes)");
                interval = 300; // Every five minutes by default
            }

            var timeout = new LicenseEvaluator(_licenseService, interval);
            PEMSEventSource.Log.TimeoutServiceStartingEvaluator("Starting license evaluator...");
            timeout.Start();

            _cancelSource.Token.WaitHandle.WaitOne();
            PEMSEventSource.Log.TimeoutServiceStoppingEvaluator("Stopping license evaluator...");
            timeout.Stop();
        }

        void RoleEnvironment_Changed(object sender, RoleEnvironmentChangedEventArgs e)
        {
            RoleEnvironmentConfigurationSettingChange change = e.Changes.OfType<RoleEnvironmentConfigurationSettingChange>().FirstOrDefault();
            if (change != null)
            {
                // Perform an action, for example, you can initialize a client, 
                // or you can recycle the role

                if (change.ConfigurationSettingName == LicenseCleanupIntervalConfigurationSettingName)
                {
                    PEMSEventSource.Log.TimeoutServiceConfigChange("{0} setting has changed. Recycling the worker role...", LicenseCleanupIntervalConfigurationSettingName);
                    _cancelSource.Cancel();
                }
            }
        }
    }
}
