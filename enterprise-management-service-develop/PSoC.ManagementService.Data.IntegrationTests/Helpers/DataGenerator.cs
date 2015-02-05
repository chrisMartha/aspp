using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PSoC.ManagementService.Core;
using PSoC.ManagementService.Data.Models;
using PSoC.ManagementService.Data.Repositories;

namespace PSoC.ManagementService.Data.IntegrationTests.Helpers
{
    internal class DataGenerator
    {
        private readonly AccessPointRepository _accessPointRepository = new AccessPointRepository();
        private readonly DeviceRepository _deviceRepository = new DeviceRepository();
        private readonly LicenseRepository _licenseRepository = new LicenseRepository();
        private readonly LicenseRequestRepository _licenseRequestRepository = new LicenseRequestRepository();
        private readonly UserRepository _userRepository = new UserRepository();

        #region AccessPoint
        internal async Task<AccessPointDto> CreateNewAccessPointAsync()
        {
            string wifiBssid = GetRandomMacAddress();
            var accessPoint = new AccessPointDto
            {
                WifiBSSID = wifiBssid,
                WifiSSID = string.Format("SSID for {0}", wifiBssid)
            };
            accessPoint = await _accessPointRepository.InsertAsync(accessPoint).ConfigureAwait(false);
            Assert.IsNotNull(accessPoint);
            return accessPoint;
        }

        internal async Task DeleteAccessPointAsync(AccessPointDto accessPoint)
        {
            bool isDeleted = await _accessPointRepository.DeleteAsync(accessPoint.WifiBSSID).ConfigureAwait(false);
            Assert.IsTrue(isDeleted);
        }
        #endregion

        #region Device
        internal async Task<DeviceDto> CreateNewDeviceAsync()
        {
            Guid deviceId = Guid.NewGuid();
            var device = new DeviceDto { DeviceID = deviceId };
            device = await _deviceRepository.InsertAsync(device).ConfigureAwait(false);
            Assert.IsNotNull(device);
            return device;
        }

        internal async Task DeleteDeviceAsync(DeviceDto device)
        {
            bool isDeleted = await _deviceRepository.DeleteAsync(device.DeviceID).ConfigureAwait(false);
            Assert.IsTrue(isDeleted);
        }
        #endregion

        #region License
        internal async Task<LicenseDto> CreateNewLicense(LicenseRequestDto licenseRequest, bool expired = false)
        {
            var license = new LicenseDto
            {
                LicenseRequest = licenseRequest,
                ConfigCode = licenseRequest.ConfigCode,
                WifiBSSID = licenseRequest.AccessPoint.WifiBSSID,
                LicenseIssueDateTime = licenseRequest.RequestDateTime,
                LicenseExpiryDateTime = licenseRequest.RequestDateTime.Add(new TimeSpan(0, (expired ? -1 : 1) * 30, 0))
            };
            license = await _licenseRepository.InsertAsync(license).ConfigureAwait(false);
            Assert.IsNotNull(license);
            return license;
        }

        internal async Task DeleteLicenseAsync(LicenseDto license)
        {
            bool isDeleted = await _licenseRepository.DeleteAsync(license.LicenseRequest.LicenseRequestID).ConfigureAwait(false);
            Assert.IsTrue(isDeleted);
        }
        #endregion

        #region LicenseRequest
        internal async Task<LicenseRequestDto> CreateNewLicenseRequest(AccessPointDto accessPoint, DeviceDto device, UserDto user)
        {
            Guid licenseRequestId = Guid.NewGuid();
            const string configCode = "test config";
            DateTime now = DateTime.UtcNow;
            var licenseRequest = new LicenseRequestDto
            {
                LicenseRequestID = licenseRequestId,
                ConfigCode = configCode,
                LicenseRequestType = LicenseRequestType.RequestLicense,
                Device = device,
                AccessPoint = accessPoint,
                User = user,
                RequestDateTime = now
            };
            licenseRequest = await _licenseRequestRepository.InsertAsync(licenseRequest).ConfigureAwait(false);
            Assert.IsNotNull(licenseRequest);
            return licenseRequest;
        }

        internal async Task DeleteLicenseRequestAsync(LicenseRequestDto licenseRequest)
        {
            bool isDeleted = await _licenseRequestRepository.DeleteAsync(licenseRequest.LicenseRequestID).ConfigureAwait(false);
            Assert.IsTrue(isDeleted);
        }
        #endregion

        #region User
        internal async Task<UserDto> CreateNewUserAsync()
        {
            Guid userId = Guid.NewGuid();
            var user = new UserDto { UserID = userId };
            user = await _userRepository.InsertAsync(user).ConfigureAwait(false);
            Assert.IsNotNull(user);
            return user;
        }

        internal async Task DeleteUserAsync(UserDto user)
        {
            bool isDeleted = await _userRepository.DeleteAsync(user.UserID).ConfigureAwait(false);
            Assert.IsTrue(isDeleted);
        }
        #endregion

        internal static string GetRandomMacAddress()
        {
            var random = new Random();
            var buffer = new byte[6];
            random.NextBytes(buffer);
            var result = String.Concat(buffer.Select(x => string.Format("{0}:", x.ToString("X2"))).ToArray());
            return result.TrimEnd(':');
        }
    }
}