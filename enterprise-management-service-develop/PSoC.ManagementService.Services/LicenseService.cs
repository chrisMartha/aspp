using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PSoC.ManagementService.Data.Models;
using PSoC.ManagementService.Data.Interfaces;
using PSoC.ManagementService.Services.Interfaces;
using PSoC.ManagementService.Services.Logging;
using PSoC.ManagementService.Services.Models;

namespace PSoC.ManagementService.Services
{
    /// <summary>
    /// License Service - Implements interface to provide LicenseRequest and License specific operations for a device
    /// </summary>
    public class LicenseService : ILicenseService
    {
        private readonly ILicenseRepository _licenseRepository;
        private readonly IDeviceInstalledCourseRepository _deviceInstalledCourseRepository;

        public LicenseService(ILicenseRepository licenseRepository,
            IDeviceInstalledCourseRepository deviceInstalledCourseRepository)
        {
            _licenseRepository = licenseRepository;
            _deviceInstalledCourseRepository = deviceInstalledCourseRepository;
        }

        public async Task<int> DeleteExpiredLicensesAsync()
        {
            try
            {
                var expiredLicenses = await _licenseRepository.GetExpiredLicensesAsync().ConfigureAwait(false);
                var expiredLicensesCount = expiredLicenses.Count;

                if (expiredLicensesCount > 0)
                {
                    var toDelete = expiredLicenses.Select(x => x.LicenseRequest.LicenseRequestID).ToArray();
                    if ((await _licenseRepository.DeleteAsync(toDelete).ConfigureAwait(false)) == false)
                    {
                        expiredLicensesCount = 0;
                    }
                }

                return expiredLicensesCount;
            }
            catch (Exception ex)
            {
                PEMSEventSource.Log.LicenseServiceException(ex.Message, new LogRequest { Exception = ex });
                throw;
            }
        }

        public async Task<License> GetLicenseForDeviceAsync(Guid deviceId, LogRequest logRequest)
        {
            try
            {
                if (deviceId == Guid.Empty)
                {
                    var ex = new ArgumentException("Value for device Id is invalid", "deviceId");
                    throw ex;
                }

                return (License)await _licenseRepository.GetLicenseForDeviceAsync(deviceId).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logRequest = logRequest ?? new LogRequest();
                logRequest.Exception = ex;
                logRequest.DeviceId = deviceId.ToString();
                PEMSEventSource.Log.LicenseServiceException(ex.Message, logRequest);
                throw;
            }
        }

        public async Task RevokeLicenseForDeviceAsync(Guid licenseRequestId, Guid userId, DateTime requestedDateTime, LogRequest logRequest, bool isAdmin)
        {
            try
            {
                if (licenseRequestId == Guid.Empty)
                {
                    var ex = new ArgumentException("Value for License Request Id is invalid", "licenseRequestId");
                    throw ex;
                }

                if (userId == Guid.Empty)
                {
                    var ex = new ArgumentException("Value for User Id is invalid", "userId");
                    throw ex;
                }

                var result = await _licenseRepository.RevokeLicenseForDeviceAsync(licenseRequestId, userId, requestedDateTime, isAdmin).ConfigureAwait(false);

                if (!result)
                {
                    throw new Exception(String.Format("Device License revocation error for license request id {0}: No New Record add to deviceInstalledCourse table.", licenseRequestId));
                }
            }
            catch (Exception ex)
            {
                logRequest = logRequest ?? new LogRequest();
                logRequest.Exception = ex;
                logRequest.LicenseRequestId = licenseRequestId.ToString();
                logRequest.UserId = userId.ToString();
                PEMSEventSource.Log.LicenseServiceException(ex.Message, logRequest);
                throw;
            }
        }

        public async Task<bool> RequestLicenseForDeviceAsync(DeviceLicenseRequest licenseRequest, LogRequest logRequest)
        {
            try
            {
                if (licenseRequest == null || string.IsNullOrEmpty(licenseRequest.DeviceId) ||
                    string.IsNullOrEmpty(licenseRequest.WifiBSSID) || string.IsNullOrEmpty(licenseRequest.UserId))
                {
                    var ex = new ArgumentException("Value for License Request is invalid or missing params", "licenseRequest");
                    throw ex;
                }

                return await _licenseRepository.GrantLicenseForDeviceAsync((LicenseRequestDto)licenseRequest).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logRequest = logRequest ?? new LogRequest();
                logRequest.Exception = ex;
                PEMSEventSource.Log.Append(licenseRequest, logRequest);
                var deviceId = (licenseRequest != null) ? licenseRequest.DeviceId : null;
                PEMSEventSource.Log.LicenseServiceException(String.Format("Failed to respond to Request License for device {0}.", deviceId), logRequest);
                throw;
            }
        }

        public async Task<bool> SaveDownloadStatusAsync(Guid deviceId, List<Course> courses, LogRequest logRequest)
        {
            try
            {
                var dicDtos = new List<DeviceInstalledCourseDto>();
                if (courses != null)
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
                }
                return await _deviceInstalledCourseRepository.ImportDataAsync(deviceId, dicDtos).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logRequest = logRequest ?? new LogRequest();
                logRequest.Exception = ex;
                logRequest.DeviceId = deviceId.ToString();
                PEMSEventSource.Log.LicenseServiceException(ex.Message, logRequest);
                throw;
            }
        }
    }
}
