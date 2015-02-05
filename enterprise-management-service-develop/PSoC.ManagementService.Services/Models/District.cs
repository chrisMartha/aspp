using System;
using System.ComponentModel.DataAnnotations;
using PSoC.ManagementService.Data.Models;

namespace PSoC.ManagementService.Services.Models
{
    /// <summary>
    /// District model
    /// </summary>
    public class District
    {
        public Guid DistrictId { get; set; }

        [StringLength(50)]
        public string CreatedBy { get; set; }

        public DateTime CreationDate { get; set; }

        [Required]
        [StringLength(50)]
        public string DistrictName { get; set; }

        public int DistrictMaxDownloadLicenses { get; set; }

        public TimeSpan? DistrictInstructionHoursStart { get; set; }

        public TimeSpan? DistrictInstructionHoursEnd { get; set; }

        public int DistrictLicenseExpirySeconds { get; set; }

        public TimeSpan? DistrictPreloadHoursStart { get; set; }

        public TimeSpan? DistrictPreloadHoursEnd { get; set; }

        [StringLength(50)]
        public string DistrictOverrideCode { get; set; }

        public int? DistrictUserPolicy { get; set; }

        [StringLength(10)]
        public string DistrictUseCacheServer { get; set; }

        [StringLength(200)]
        public string DistrictAnnotation { get; set; }

        [Required]
        [StringLength(50)]
        public string OAuthApplicationId { get; set; }

        [Required]
        [StringLength(50)]
        public string OAuthClientId { get; set; }

        [Required]
        [StringLength(200)]
        public string OAuthUrl { get; set; }

        /// <summary>
        /// Cast DistrictDto to District
        /// </summary>
        /// <param name="district"></param>
        /// <returns></returns>
        public static explicit operator District(DistrictDto district)
        {
            if (district == null) return null;

            return new District()
            {
                DistrictId = district.DistrictId,
                CreatedBy = district.CreatedBy,
                CreationDate = district.CreationDate,
                DistrictName = district.DistrictName,
                DistrictMaxDownloadLicenses = district.DistrictMaxDownloadLicenses,
                DistrictInstructionHoursStart = district.DistrictInstructionHoursStart,
                DistrictInstructionHoursEnd = district.DistrictInstructionHoursEnd,
                DistrictLicenseExpirySeconds = district.DistrictLicenseExpirySeconds,
                DistrictPreloadHoursStart = district.DistrictPreloadHoursStart,
                DistrictPreloadHoursEnd = district.DistrictPreloadHoursEnd,
                DistrictOverrideCode = district.DistrictOverrideCode,
                DistrictUserPolicy = district.DistrictUserPolicy,
                DistrictUseCacheServer = district.DistrictUseCacheServer,
                DistrictAnnotation = district.DistrictAnnotation,
                OAuthApplicationId = district.OAuthApplicationId,
                OAuthClientId = district.OAuthClientId,
                OAuthUrl = district.OAuthURL
            };
        }

        /// <summary>
        /// Cast District to DistrictDto
        /// </summary>
        /// <param name="district"></param>
        /// <returns></returns>
        public static explicit operator DistrictDto(District district)
        {
            if (district == null) return null;

            return new DistrictDto()
            {
                DistrictId = district.DistrictId,
                CreatedBy = district.CreatedBy,
                CreationDate = district.CreationDate,
                DistrictName = district.DistrictName,
                DistrictMaxDownloadLicenses = district.DistrictMaxDownloadLicenses,
                DistrictInstructionHoursStart = district.DistrictInstructionHoursStart,
                DistrictInstructionHoursEnd = district.DistrictInstructionHoursEnd,
                DistrictLicenseExpirySeconds = district.DistrictLicenseExpirySeconds,
                DistrictPreloadHoursStart = district.DistrictPreloadHoursStart,
                DistrictPreloadHoursEnd = district.DistrictPreloadHoursEnd,
                DistrictOverrideCode = district.DistrictOverrideCode,
                DistrictUserPolicy = district.DistrictUserPolicy,
                DistrictUseCacheServer = district.DistrictUseCacheServer,
                DistrictAnnotation = district.DistrictAnnotation,
                OAuthApplicationId = district.OAuthApplicationId,
                OAuthClientId = district.OAuthClientId,
                OAuthURL = district.OAuthUrl
            };
        }
    }
}