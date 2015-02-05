using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PSoC.ManagementService.Services.Models;

namespace PSoC.ManagementService.Services.Interfaces
{
    /// <summary>
    /// Interface for access point service
    /// </summary>
    public interface IAccessPointService
    {
        Task<IList<AccessPoint>> GetAsync(String username);
        Task<IList<AccessPoint>> GetByDistrictAsync(Guid districtId);
        Task<IList<AccessPoint>> GetBySchoolAsync(Guid schoolId);
        Task<AccessPoint> GetByIdAsync(String username, String key);
        Task<AccessPoint> CreateAsync(String username, AccessPoint entity);
        Task<AccessPoint> UpdateAsync(String username, AccessPoint entity);
        Task<Boolean> DeleteAsync(String username, String key);
    }
}
