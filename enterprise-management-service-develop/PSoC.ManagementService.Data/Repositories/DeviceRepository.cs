using System;
using PSoC.ManagementService.Data.DataMapper;
using PSoC.ManagementService.Data.Interfaces;
using PSoC.ManagementService.Data.Models;
using PSoC.ManagementService.Data.QueryFactory;

namespace PSoC.ManagementService.Data.Repositories
{
    public class DeviceRepository : Repository<DeviceDto, DeviceQuery, DeviceDataMapper, Guid>, IDeviceRepository
    {
    }
}