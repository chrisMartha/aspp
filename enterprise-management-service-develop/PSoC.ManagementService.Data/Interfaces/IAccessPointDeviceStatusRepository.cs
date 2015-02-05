using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using PSoC.ManagementService.Core;
using PSoC.ManagementService.Data.Models;

namespace PSoC.ManagementService.Data.Interfaces
{
    /// <summary>
    /// Interface for AccessPointDeviceStatusRepository
    /// </summary>
    public interface IAccessPointDeviceStatusRepository : IDataRepository<DeviceDto, Guid>
    {
        /// <summary>
        /// Return both 1)List of DeviceDto object from database by given pageSize and start Index and 2)total rows of records before pagination
        /// </summary>
        /// <param name="type">User's admin type: GlobalAdmin, DistrictAdmin or SchoolAdmin</param>
        /// <param name="id">
        /// Global Admin: id = null; 
        /// District Admin: id = district id(not nullable); 
        /// School Admin: id = school id(not nullable)
        /// </param>
        /// <param name="pageSize">Number of status items on each page</param>
        /// <param name="startIndex">Index of the record to be displayed at the beginning (0 based)</param>
        /// <returns>
        /// List<DeviceDto>: List of DeviceDto object from database by given pageSize and start Index 
        /// int: total rows of records before pagination
        /// </returns>
        Task<Tuple<List<DeviceDto>, int>> GetByAdminTypeAsync(AdminType type, Guid? id, int pageSize, int startIndex);
    }
}
