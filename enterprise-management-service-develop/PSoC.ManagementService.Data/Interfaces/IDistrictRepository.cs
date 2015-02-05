using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PSoC.ManagementService.Data.Models;

namespace PSoC.ManagementService.Data.Interfaces
{
    /// <summary>
    /// An interface for District repository
    /// </summary>
    public interface IDistrictRepository : IDataRepository<DistrictDto, Guid>
    {
    }
}