using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using PSoC.ManagementService.Services.Models;

namespace PSoC.ManagementService.Services.Interfaces
{
    public interface ISchoolService : IDataService<School, Guid>
    {
        Task<IList<School>> GetAsync(string username);
    }
}
