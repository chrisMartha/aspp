using System;

using PSoC.ManagementService.Data.Models;

namespace PSoC.ManagementService.Data.Interfaces
{
    public interface IUserRepository : IDataRepository<UserDto, Guid>
    {
    }
}