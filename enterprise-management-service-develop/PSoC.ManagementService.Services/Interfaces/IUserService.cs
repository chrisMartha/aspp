using System;
using System.Threading.Tasks;
using PSoC.ManagementService.Services.Logging;
using PSoC.ManagementService.Services.Models;

namespace PSoC.ManagementService.Services.Interfaces
{
    public interface IUserService
    {
        Task<User> GetUserAsync(Guid userId, LogRequest logRequest);
        Task<User> InsertUserAsync(User user, LogRequest logRequest);
     }
}
