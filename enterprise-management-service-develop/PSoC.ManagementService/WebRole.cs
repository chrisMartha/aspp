using System;
using System.Linq;
using Microsoft.WindowsAzure.ServiceRuntime;
using PSoC.ManagementService.Services.Logging;

namespace PSoC.ManagementService
{
    public class WebRole : RoleEntryPoint
    {
        public override Boolean OnStart()
        {
            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.
            RoleEnvironment.Changing += RoleEnvironmentChanging; 
            return base.OnStart();
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
    }
}