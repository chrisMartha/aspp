using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PSoC.ManagementService.Services.Logging;
using PSoC.ManagementService.Services.Models;

namespace PSoC.ManagementService.Services.Interfaces
{
    /// <summary>
    /// Interface for License Service
    /// </summary>
    public interface ILicenseService
    {
        /// <summary>
        /// Gets License for a device
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="logRequest"></param>
        /// <returns>DeviceLicense if a valid license is present; otherwise returns null</returns>
        Task<License> GetLicenseForDeviceAsync(Guid deviceId, LogRequest logRequest);

        /// <summary>
        /// Invoked by Background Job (LicenseTimer->LicenseEvaluator running as azure worker role) to delete expired licenses
        /// </summary>
        /// <returns>an integer value to denote deleted licenses</returns>
        Task<int> DeleteExpiredLicensesAsync();

        /// <summary>
        /// Removes a license for a device identified by licenseRequestId. 
        /// Default to revoke by Admin (isAdmin=true). isAdmin=false denotes license being returned by the device. 
        /// </summary>
        /// <param name="licenseRequestId"></param>
        /// <param name="userId"></param>
        /// <param name="requestedDateTime"></param>
        /// <param name="logRequest"></param>
        /// <param name="isAdmin"></param>
        /// <returns>void</returns>
        Task RevokeLicenseForDeviceAsync(Guid licenseRequestId, Guid userId, DateTime requestedDateTime,
            LogRequest logRequest,
            bool isAdmin = true);

        /// <summary>
        /// Request to grant a license from device
        /// </summary>
        /// <param name="licenseRequest"></param>
        /// <param name="logRequest"></param>
        /// <returns>True if # of rows affected is greater than 0; otherwise false</returns>
        Task<bool> RequestLicenseForDeviceAsync(DeviceLicenseRequest licenseRequest, LogRequest logRequest);
        
        /// <summary>
        /// Save the list of courses currently installed on the device
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="courses"></param>
        /// <param name="logRequest"></param>
        /// <returns>True if # of rows affected is greater than 0; otherwise false</returns>
        Task<bool> SaveDownloadStatusAsync(Guid deviceId, List<Course> courses, LogRequest logRequest);
    }
}
