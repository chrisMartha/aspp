using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PSoC.ManagementService.Services.Models;

namespace PSoC.ManagementService.Services.Interfaces
{
    /// <summary>
    /// Interface for user service
    /// </summary>
    public interface IAdminService
    {
        /// <summary>
        /// Gets user information by username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        Task<Admin> GetByUsernameAsync(string username);
        Task UpdateLastLoginDateTime(Guid userId, DateTime loginDateTime);

        Task<IEnumerable<Admin>> GetAsync();
        Task<Admin> GetByIdAsync(Guid userId);
        Task<bool> InsertAsync(Admin admin);
        Task<bool> UpdateAsync(Admin admin);
        Task<bool> DeleteAsync(Guid userId);
    }
}
