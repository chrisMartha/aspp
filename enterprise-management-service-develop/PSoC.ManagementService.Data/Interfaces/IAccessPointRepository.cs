using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using PSoC.ManagementService.Data.Models;

namespace PSoC.ManagementService.Data.Interfaces
{
    /// <summary>
    /// Interface for access point repository
    /// </summary>
    public interface IAccessPointRepository : IDataRepository<AccessPointDto, String>
    {
        Task<IList<AccessPointDto>> GetByDistrictAsync(Guid districtId);

        Task<IList<AccessPointDto>> GetBySchoolAsync(Guid schoolId);
    }
}