using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PSoC.ManagementService.Data.Interfaces;

namespace PSoC.ManagementService.Data.Models
{
    public class DistrictDto : IEntity
    {
        #region Constructors

        public DistrictDto()
        {
            AccessPoints = new HashSet<AccessPointDto>();
            Classrooms = new HashSet<ClassroomDto>();
            Licenses = new HashSet<LicenseDto>();
            LicenseRequests = new HashSet<LicenseRequestDto>();
            Schools = new HashSet<SchoolDto>();
        }

        #endregion Constructors

        public ICollection<AccessPointDto> AccessPoints
        {
            get; set;
        }

        public ICollection<ClassroomDto> Classrooms
        {
            get; set;
        }

        [StringLength(50)]
        public string CreatedBy
        {
            get; set;
        }

        [Column(TypeName = "datetime2")]
        public DateTime CreationDate
        {
            get; set;
        }

        [StringLength(200)]
        public string DistrictAnnotation
        {
            get; set;
        }

        [Key]
        [Required]
        public Guid DistrictId
        {
            get; set;
        }

        public TimeSpan? DistrictInstructionHoursEnd
        {
            get; set;
        }

        public TimeSpan? DistrictInstructionHoursStart
        {
            get; set;
        }

        public int DistrictLicenseExpirySeconds
        {
            get; set;
        }

        public int DistrictMaxDownloadLicenses
        {
            get; set;
        }

        [Required]
        [StringLength(50)]
        public string DistrictName
        {
            get; set;
        }

        [StringLength(50)]
        public string DistrictOverrideCode
        {
            get; set;
        }

        public TimeSpan? DistrictPreloadHoursEnd
        {
            get; set;
        }

        public TimeSpan? DistrictPreloadHoursStart
        {
            get; set;
        }

        [StringLength(10)]
        public string DistrictUseCacheServer
        {
            get; set;
        }

        public int? DistrictUserPolicy
        {
            get; set;
        }

        public ICollection<LicenseRequestDto> LicenseRequests
        {
            get; set;
        }

        public ICollection<LicenseDto> Licenses
        {
            get; set;
        }

        [Required]
        [StringLength(50)]
        public string OAuthApplicationId
        {
            get; set;
        }

        [Required]
        [StringLength(50)]
        public string OAuthClientId
        {
            get; set;
        }

        [Required]
        [StringLength(200)]
        public string OAuthURL
        {
            get; set;
        }

        public ICollection<SchoolDto> Schools
        {
            get; set;
        }
    }
}