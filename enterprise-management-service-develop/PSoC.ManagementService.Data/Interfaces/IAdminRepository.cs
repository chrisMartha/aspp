using System;
using System.Threading.Tasks;

using PSoC.ManagementService.Data.Models;

namespace PSoC.ManagementService.Data.Interfaces
{
    /// <summary>
    /// An interface for Admin Repository
    /// </summary>
    public interface IAdminRepository : IDataRepository<AdminDto, Guid>
    {
        Task<AdminDto> GetByUsernameAsync(string username);

        Task UpdateLastLoginDateTimeAsync(Guid userId, DateTime loginDateTime);
    }
}
