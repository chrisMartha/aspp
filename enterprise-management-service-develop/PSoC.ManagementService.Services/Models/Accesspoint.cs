using System;
using System.ComponentModel.DataAnnotations;
using PSoC.ManagementService.Data.Models;

namespace PSoC.ManagementService.Services.Models
{
    /// <summary>
    /// Access point model
    /// </summary>
    public class AccessPoint
    {
        [StringLength(17)]
        public string WifiBSSId { get; set; }

        [Required]
        [StringLength(32)]
        public string WifiSSId { get; set; }

        public Guid? DistrictId { get; set; }

        public Guid? SchoolId { get; set; }

        public Guid? ClassroomId { get; set; }

        public int AccessPointMaxDownloadLicenses { get; set; }

        public int? AccessPointExpiryTimeSeconds { get; set; }

        [StringLength(200)]
        public string AccessPointAnnotation { get; set; }

        [StringLength(50)]
        public string AccessPointModel { get; set; }

        public DateTime Created { get; set; }

        /// <summary>
        /// Cast AccessPointDto to AccessPoint
        /// </summary>
        /// <param name="accesspoint"></param>
        /// <returns></returns>
        public static explicit operator AccessPoint(AccessPointDto accesspoint)
        {
            if (accesspoint == null) return null;

            return new AccessPoint()
            {
                WifiBSSId = accesspoint.WifiBSSID,
                WifiSSId = accesspoint.WifiSSID,
                DistrictId = (accesspoint.District != null) ? (Guid?)accesspoint.District.DistrictId : null,
                SchoolId = (accesspoint.School != null) ? (Guid?)accesspoint.School.SchoolID : null,
                ClassroomId = (accesspoint.Classroom != null) ? (Guid?)accesspoint.Classroom.ClassroomID : null,
                AccessPointMaxDownloadLicenses = accesspoint.AccessPointMaxDownloadLicenses,
                AccessPointExpiryTimeSeconds = accesspoint.AccessPointExpiryTimeSeconds,
                AccessPointAnnotation = accesspoint.AccessPointAnnotation,
                AccessPointModel = accesspoint.AccessPointModel,
                Created = accesspoint.Created
            };
        }

        /// <summary>
        /// Cast AccessPoint to AccessPointDto
        /// </summary>
        /// <param name="accesspoint"></param>
        /// <returns></returns>
        public static explicit operator AccessPointDto(AccessPoint accesspoint)
        {
            if (accesspoint == null) return null;

            return new AccessPointDto()
            {
                WifiBSSID = accesspoint.WifiBSSId,
                WifiSSID = accesspoint.WifiSSId,
                District = (accesspoint.DistrictId.HasValue) ? new DistrictDto { DistrictId = accesspoint.DistrictId.Value } : null,
                School = (accesspoint.SchoolId.HasValue) ? new SchoolDto { SchoolID = accesspoint.SchoolId.Value } : null,
                Classroom = (accesspoint.ClassroomId.HasValue) ? new ClassroomDto { ClassroomID = accesspoint.ClassroomId.Value } : null,
                AccessPointMaxDownloadLicenses = accesspoint.AccessPointMaxDownloadLicenses,
                AccessPointExpiryTimeSeconds = accesspoint.AccessPointExpiryTimeSeconds,
                AccessPointAnnotation = accesspoint.AccessPointAnnotation,
                AccessPointModel = accesspoint.AccessPointModel,
                Created = accesspoint.Created
            };
        }
    }
}