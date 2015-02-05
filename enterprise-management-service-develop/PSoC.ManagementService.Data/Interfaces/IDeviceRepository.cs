using System;

using PSoC.ManagementService.Data.Models;

namespace PSoC.ManagementService.Data.Interfaces
{
    /// <summary>
    /// An interface for Device repository
    /// </summary>
    public interface IDeviceRepository : IDataRepository<DeviceDto, Guid>
    {
    }
}