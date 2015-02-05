using System;
using System.Collections.Generic;
using Newtonsoft.Json;

using PSoC.ManagementService.Core;
using PSoC.ManagementService.Core.Extensions;
using PSoC.ManagementService.Data.Models;

namespace PSoC.ManagementService.Services.Models
{
    /// <summary>
    /// Model to wrap License Request from a Device/Site. In use by License Grant/Revoke endpoint to serialize/deserialize payload
    /// </summary>
    public class DeviceLicenseRequest
    {
        public DeviceLicenseRequest()
        {
            //Default Initialization
            DownloadLicenseRequested = true;
            RequestType = LicenseRequestType.RequestLicense; //Default to requesting a new license from device app
            LearningContentQueued = 0;  //Default to 0 - this will implicitly indicate revoke license since there is nothing to download
            RequestTime = DateTime.UtcNow;
            CanDownloadLearningContent = false;
        }

        public LicenseRequestType RequestType { get; set; }
        public bool ShouldSerializeRequestType()
        {
            return false;
        }

        public bool DownloadLicenseRequested { get; set; }
        public bool CanDownloadLearningContent { get; set; }
        
        public DateTime RequestTime { get; internal set; }
        public bool ShouldSerializeRequestTime()
        {
            return false;
        }

        public string EnvironmentId { get; set; }   //Represents Config Code
        public int LearningContentQueued { get; set; }

        public string DeviceId { get; set; }
        public string DeviceName { get; set; }
        public string DeviceType { get; set; }
        public string DeviceOS { get; set; }
        public string DeviceOSVersion { get; set; }

        [JsonProperty("AppId")]
        public string PSoCAppId { get; set; }

        [JsonProperty("AppVersion")]
        public string PSoCAppVersion { get; set; }

        [JsonProperty("freeSpaceSize")]
        public long? DeviceFreeSpace { get; set; }

        [JsonIgnore]
        public string LastUsedConfigCode
        {
            get { return EnvironmentId; }
        }

        public string DeviceAnnotation { get; set; }

        public int? ConfiguredUnitCount { get; set; }
        public DateTime? ContentLastUpdatedAt { get; set; } //Assumption that device will be sending this data in UTC format
        public long? InstalledContentSize { get; set; }
        public string GeoLocation { get; set; }

        public string UserId { get; set; }
        public string Username { get; set; }
        public string UserType { get; set; }    //Possible values are student, teacher from device app. Otherwise Admin through site.
        
        public string WifiBSSID { get; set; }
        public string WifiSSID { get; set; }
        
        public string LocationId { get; set; }
        public string LocationName { get; set; }

        [JsonIgnore]
        public List<int> ConfiguredGrades { get; set; }

        public List<Dictionary<string, object>> CoursesInstalled { get; set; }

        [JsonIgnore]
        public bool IsAdminRequest
        {
            get { return (RequestType == LicenseRequestType.RevokeLicense); }
        }

        public static explicit operator List<Course>(DeviceLicenseRequest request)
        {
            if (request == null || !request.CoursesInstalled.HasElements()) return null;
            var listCourses = new List<Course>();

            foreach (var installedCourse in request.CoursesInstalled)
            {
                // Make sure learning resource id is specified or ignore this content as learning resource id is required in the database. Sorry, K1 app! :(
                Guid learningResourceId;
                if (installedCourse.ContainsKey("LearningResourceId") && installedCourse["LearningResourceId"] != null && 
                    Guid.TryParse(installedCourse["LearningResourceId"].ToString(), out learningResourceId))
                {
                    decimal? nullablePercent;
                    decimal percent;
                    if (installedCourse.ContainsKey("Percent") && installedCourse["Percent"] != null && 
                        decimal.TryParse(installedCourse["Percent"].ToString(), out percent))
                    {
                        nullablePercent = percent;
                    }
                    else
                    {
                        nullablePercent = null;
                    }

                    var course = new Course
                    {
                        Subject = installedCourse.ContainsKey("Subject") ? installedCourse["Subject"].ToString() : null,
                        Grade = installedCourse.ContainsKey("Grade") ? installedCourse["Grade"].ToString() : null,
                        Percent = nullablePercent,
                        LearningResourceId = learningResourceId
                    };

                    listCourses.Add(course);
                }
            }

            return listCourses;
        }

        public static explicit operator LicenseRequestDto(DeviceLicenseRequest request)
        {
            if (request == null) return null;

            return new LicenseRequestDto()
            {
                User = string.IsNullOrEmpty(request.UserId) ? null : new UserDto()
                {
                    UserID = new Guid(request.UserId),
                    UserType = request.UserType,
                    Username = request.Username
                },
                AccessPoint = string.IsNullOrEmpty(request.WifiBSSID) ? null : 
                    new AccessPointDto()
                    {
                     WifiBSSID = request.WifiBSSID,
                     WifiSSID = request.WifiSSID
                    },
                Device = string.IsNullOrEmpty(request.DeviceId) ? null :
                new DeviceDto()
                {
                    DeviceID = new Guid(request.DeviceId),
                    LastUsedConfigCode = request.LastUsedConfigCode,
                    DeviceName = request.DeviceName,
                    DeviceType = request.DeviceType,
                    DeviceOS = request.DeviceOS,
                    DeviceOSVersion = request.DeviceOSVersion,
                    PSoCAppID = request.PSoCAppId,
                    PSoCAppVersion = request.PSoCAppVersion,
                    DeviceFreeSpace = request.DeviceFreeSpace,
                    DeviceAnnotation = request.DeviceAnnotation,
                    ConfiguredGrades = request.ConfiguredGrades != null ? string.Join(",", request.ConfiguredGrades) : null,
                    ConfiguredUnitCount = request.ConfiguredUnitCount,
                    GeoLocation = request.GeoLocation,
                    InstalledContentSize = request.InstalledContentSize,
                },
                LearningContentQueued = request.LearningContentQueued,
                LocationId = request.LocationId,
                LocationName = request.LocationName,
                RequestDateTime = request.RequestTime
            };
        }
    
    }
}