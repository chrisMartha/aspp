using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using PSoC.ManagementService.Data.Models;

namespace PSoC.ManagementService.Data.Interfaces
{
    public interface IDeviceInstalledCourseRepository : IDataRepository<DeviceInstalledCourseDto, Tuple<Guid, Guid>>
    {
        Task<bool> ImportDataAsync(Guid deviceId, List<DeviceInstalledCourseDto> dicDtos);
    }
}