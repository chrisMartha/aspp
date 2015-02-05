using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PSoC.ManagementService.Services.Models;

namespace PSoC.ManagementService.Services.Interfaces
{
    /// <summary>
    /// Interface for district service
    /// </summary>
    public interface IDistrictService : IDataService<District, Guid>
    {
        Task<IList<District>> GetAsync(String username);
        Task<District> GetByIdAsync(String username, Guid key);
        Task<District> CreateAsync(String username, District entity);
        Task<District> UpdateAsync(String username, District entity);
        Task<Boolean> DeleteAsync(String username, Guid key);
    }
}
