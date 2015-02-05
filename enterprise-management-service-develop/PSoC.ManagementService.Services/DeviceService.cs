using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PSoC.ManagementService.Core;
using PSoC.ManagementService.Data.Models;
using PSoC.ManagementService.Data.Interfaces;
using PSoC.ManagementService.Services.Interfaces;
using PSoC.ManagementService.Services.Logging;
using PSoC.ManagementService.Services.Models;

namespace PSoC.ManagementService.Services
{
    public class DeviceService : /*DataService<DeviceDto, Guid>,*/ IDeviceService
    {
        private readonly IAzureTableService _azureTableService;
        private readonly IConfigBase _configBase;
        private readonly string _deviceTable;
        private readonly IDeviceInstalledCourseRepository _deviceInstalledCourseRepository;
        private readonly IAccessPointDeviceStatusRepository _accessPointDeviceStatusRepository;

        //TODO: Add this back in once services is switched to using the new datalayer
        //public DeviceService(IUnitOfWork unitOfWork)
        //    : base(unitOfWork)
        //{
        //}

        //TODO:  This will be deleted in another story
        public DeviceService(IConfigBase configBase,
                             IAzureTableService azureTableService,
                             IUnitOfWork unitOfWork, 
                             IDeviceInstalledCourseRepository deviceInstalledCourseRepository,
                             IAccessPointDeviceStatusRepository accessPointDeviceStatusRepository)
            //: base(unitOfWork)
        {
            _azureTableService = azureTableService;
            _configBase = configBase;
            _deviceTable = _configBase.GetApplicationSetting<string>("StorageTableName");
            _deviceInstalledCourseRepository = deviceInstalledCourseRepository;
            _accessPointDeviceStatusRepository = accessPointDeviceStatusRepository;
        }
        
        public async Task<Device> GetDeviceAsync(string environmentId, string deviceId)
        {
            try
            {
                if (string.IsNullOrEmpty(environmentId) || string.IsNullOrEmpty(deviceId))
                {
                    var ex = new ArgumentNullException("environmentId or deviceId");
                    throw ex;
                }

                PEMSEventSource.Log.DeviceServiceGetDevicesRequested("Get requested by partition row key");
                var res = await _azureTableService.GetEntityByPartitionRowKeyAsync<Device>(_deviceTable, environmentId, deviceId);
                return res;
            }
            catch (Exception ex)
            {
                PEMSEventSource.Log.DeviceServiceException(ex.Message, new LogRequest { Exception = ex, ConfigCode = environmentId, DeviceId = deviceId });
                throw;
            }
        }

        public async Task<IQueryable<Device>> GetDevices(List<string> environmentIds)
        {
            try
            {
                if (environmentIds == null)
                {
                    var ex = new ArgumentNullException("environmentIds");
                    throw ex;
                }

                PEMSEventSource.Log.DeviceServiceGetDevicesRequested("Get requested by environment Ids");
                var devices = await _azureTableService.GetEntitiesByPartitionKeys<Device>(_deviceTable, environmentIds);
                return devices;
            }
            catch (Exception ex)
            {
                PEMSEventSource.Log.DeviceServiceException(ex.Message, new LogRequest { Exception = ex });
                throw;
            }
        }

        public async Task<IQueryable<Device>> GetDevices(string deviceId)
        {
            try
            {
                if (string.IsNullOrEmpty(deviceId))
                {
                    var ex = new ArgumentNullException("deviceId");
                    throw ex;
                }

                PEMSEventSource.Log.DeviceServiceGetDevicesRequested(string.Format("Get requested for deviceId {0}", deviceId));
                var devices = await _azureTableService.GetEntitiesByRowKey<Device>(_deviceTable, deviceId);
                return devices;
            }
            catch (Exception ex)
            {
                PEMSEventSource.Log.DeviceServiceException(ex.Message, new LogRequest { Exception = ex, DeviceId = deviceId });
                throw;
            }
        }

        public async Task<bool> InsertOrUpdateDeviceAsync(Device device)
        {
            try
            {
                if (device == null)
                {
                    var ex = new ArgumentNullException("device");
                    throw ex;
                }

                var res = await _azureTableService.InsertOrReplaceEntityAsync<Device>(_deviceTable, device);
                PEMSEventSource.Log.DeviceServiceInsertUpdate(string.Format("Insert/update {0} for device", res), device.DeviceId);
                return res;
            }
            catch (Exception ex)
            {
                var deviceId = (device != null) ? device.DeviceId : null;
                PEMSEventSource.Log.DeviceServiceException(ex.Message, new LogRequest { Exception = ex, DeviceId = deviceId });
                throw;
            }
        }

        public async Task<bool> DeleteDeviceAsync(Device device)
        {
            try
            {
                if (device == null)
                {
                    var ex = new ArgumentNullException("device");
                    throw ex;
                }

                var res = await _azureTableService.DeleteEntityAsync<Device>(_deviceTable, device);
                PEMSEventSource.Log.DeviceServiceDelete(string.Format("Delete {0} for device", res), device.DeviceId);
                return res;
            }
            catch (Exception ex)
            {
                var deviceId = (device != null) ? device.DeviceId : null;
                PEMSEventSource.Log.DeviceServiceException(ex.Message, new LogRequest { Exception = ex, DeviceId = deviceId });
                throw;
            }
        }

        public async Task SaveDownloadStatusAsync(Guid deviceId, List<Course> courses, LogRequest logRequest = null)
        {
            var dicDtos = new List<DeviceInstalledCourseDto>();
            try
            {
                foreach (var course in courses)
                {
                    var dicDto = (DeviceInstalledCourseDto) course;
                    dicDto.Device = new DeviceDto
                    {
                        DeviceID = deviceId
                    };
                    dicDtos.Add(dicDto);
                }

                var result = await _deviceInstalledCourseRepository.ImportDataAsync(deviceId, dicDtos).ConfigureAwait(false);
                if (!result)
                {
                    throw new Exception(String.Format("Device service database error with device id {0}: No New Record add to deviceInstalledCourse table.", deviceId));
                }     
            }
            catch (Exception ex)
            {
                logRequest = logRequest ?? new LogRequest();
                logRequest.Exception = ex;
                PEMSEventSource.Log.DeviceServiceSaveDownloadStatusException(ex.Message, deviceId.ToString(), logRequest);
                throw;
            }           
        }

        public async Task<Tuple<List<AccessPointDeviceStatus>, int>> GetAccessPointDeviceStatusAsync(
                                                                            AdminType type, 
                                                                            Guid? id,
                                                                            int pageSize,
                                                                            int startIndex,
                                                                            LogRequest logRequest = null)
        {
            try
            {
                if ((type == AdminType.DistrictAdmin || type == AdminType.SchoolAdmin) && id == null)
                {
                    throw new Exception(String.Format("GetAccessPointDeviceStatus Failed: Invalid id for {0}.",
                                    type == AdminType.DistrictAdmin ? "District Admin" :
                                    type == AdminType.SchoolAdmin ? "School Admin" : type.ToString()));
                }

                var result = await _accessPointDeviceStatusRepository.GetByAdminTypeAsync(type, id, pageSize, startIndex).ConfigureAwait(false);
                return new Tuple<List<AccessPointDeviceStatus>, int>(
                    result.Item1.Select(dto => new AccessPointDeviceStatus(dto)).ToList(),
                    result.Item2
                );
            }
            catch (Exception ex)
            {
                logRequest = logRequest ?? new LogRequest();
                logRequest.Exception = ex;
                logRequest.DistrictId = (type == AdminType.DistrictAdmin) ? ((id.HasValue) ? id.ToString() : logRequest.DistrictId) : logRequest.DistrictId;
                logRequest.SchoolId = (type == AdminType.SchoolAdmin) ? ((id.HasValue) ? id.ToString() : logRequest.SchoolId) : logRequest.SchoolId;
                PEMSEventSource.Log.DeviceServiceGetAccessPointDeviceStatusException(String.Format("GetAccessPointDeviceStatus Failed: {0}.", ex.Message), logRequest);
                throw;
            }
        }
    }
}
