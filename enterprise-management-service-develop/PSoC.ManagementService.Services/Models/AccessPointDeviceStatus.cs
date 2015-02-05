using System;

using PSoC.ManagementService.Core;
using PSoC.ManagementService.Data.Models;

namespace PSoC.ManagementService.Services.Models
{
    /// <summary>
    /// Model to wrap AccessPoint, DeviceStatus, District and School info for Admin Dashboard Website.
    /// </summary>
    [Serializable]
    public class AccessPointDeviceStatus
    {
        public DateTime     Created             { get; set; }
        public DateTime?    LastContentUpdate   { get; set; }
        public Guid         DeviceId            { get; set; }
        public string       DeviceName          { get; set; }
        public string       DeviceType          { get; set; }
        public string       DeviceOSVersion     { get; set; }
        public string       Username            { get; set; }
        public string       UserType            { get; set; }
        public string       ConfiguredGrade     { get; set; }
        public string       LocationName        { get; set; }
        public string       WifiBSSID           { get; set; }
        public string       WifiSSID            { get; set; }
        public string       DownloadRequestType { get; set; }
        public int          DownloadRequestCount { get; set; }
        public bool         CanRevoke           { get; set; }
        public string       DistrictName        { get; set; }
        public string       SchoolName          { get; set; }

        public AccessPointDeviceStatus(DeviceDto dto)
        {
            var lastLicenseRequest = dto.LastLicenseRequest;

            LastContentUpdate = dto.ContentLastUpdatedAt;
            DeviceId = dto.DeviceID;
            DeviceName = dto.DeviceName;
            DeviceType = dto.DeviceType;
            DeviceOSVersion = dto.DeviceOSVersion;
            ConfiguredGrade = dto.ConfiguredGrades;

            if (lastLicenseRequest != null)
            {
                if (lastLicenseRequest.User != null)
                {
                    Username = dto.LastLicenseRequest.User.Username;
                    UserType = dto.LastLicenseRequest.User.UserType;
                }

                if (lastLicenseRequest.AccessPoint != null)
                {
                    WifiBSSID = dto.LastLicenseRequest.AccessPoint.WifiBSSID;
                    WifiSSID = dto.LastLicenseRequest.AccessPoint.WifiSSID;

                    if (lastLicenseRequest.AccessPoint.District != null)
                    {
                        DistrictName = dto.LastLicenseRequest.AccessPoint.District.DistrictName;
                    }

                    if (lastLicenseRequest.AccessPoint.School != null)
                    {
                        SchoolName = dto.LastLicenseRequest.AccessPoint.School.SchoolName;
                    }
                }

                LocationName = dto.LastLicenseRequest.LocationName;
                DownloadRequestType = GetLicenseRequestType(dto.LastLicenseRequest.LicenseRequestType);
                DownloadRequestCount = dto.LastLicenseRequest.LearningContentQueued ?? 0;
                Created = dto.LastLicenseRequest.Created;
            }

            CanRevoke = dto.CanRevoke;
        }

        private static string GetLicenseRequestType(LicenseRequestType requestType)
        {
            switch (requestType)
            {
                case LicenseRequestType.RequestLicense:
                    return "Requested License";
                case LicenseRequestType.ReturnLicense:
                    return "Returned License";
                case LicenseRequestType.RevokeLicense:
                    return "Revoked License";
                case LicenseRequestType.ServerGrant:
                    return "Server Granted";
                default:
                    return string.Empty;
            }
        }

    }
}
