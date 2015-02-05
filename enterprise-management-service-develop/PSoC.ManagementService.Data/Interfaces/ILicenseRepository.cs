using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using PSoC.ManagementService.Data.Models;

namespace PSoC.ManagementService.Data.Interfaces
{
    public interface ILicenseRepository : IDataRepository<LicenseDto, Guid>
    {
        /// <summary>
        /// Retrieve from the database and return licenses previously granted to applications that are past expiration date
        /// </summary>
        /// <returns>Licenses previously granted to applications that are past expiration date</returns>
        Task<IList<LicenseDto>> GetExpiredLicensesAsync();

        /// <summary>
        /// Retrieves license for a device
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns>LicenseDto object if a valid license is present for the device; otherwise null</returns>
        Task<LicenseDto> GetLicenseForDeviceAsync(Guid deviceId);

        /// <summary>
        /// Revokes a license for the device
        /// </summary>
        /// <param name="licenseRequestId">License identified by LicenseRequestId to be revoked</param>
        /// <param name="userId">Indicates user who initiated the Return/Revoke License</param>
        /// <param name="requestedDateTime">DateTime in UTC format when Return/Revoke was requested</param>
        /// <param name="isAdmin">True for Revoke License if requested by an Admin; False to denote being returned by a Device</param>
        /// <returns>True if # of rows affected by command is greater than 0; otherwise false</returns>
        Task<bool> RevokeLicenseForDeviceAsync(Guid licenseRequestId, Guid userId, DateTime requestedDateTime, bool isAdmin = true);

        /// <summary>
        /// Grants a license for device
        /// </summary>
        /// <param name="licenseRequest">Indicates LicenseRequestDto Object</param>
        /// <returns>True if # of rows affected by command is greater than 0; otherwise false</returns>
        Task<bool> GrantLicenseForDeviceAsync(LicenseRequestDto licenseRequest);
    }
}
