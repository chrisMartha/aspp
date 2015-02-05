using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using PSoC.ManagementService.Data.Interfaces;

namespace PSoC.ManagementService.Data.Models
{
    public class SchoolDto : IEntity
    {
        public SchoolDto()
        {
            AccessPoints = new HashSet<AccessPointDto>();
            Classrooms = new HashSet<ClassroomDto>();
            Licenses = new HashSet<LicenseDto>();
            LicenseRequests = new HashSet<LicenseRequestDto>();
        }

        [Key]
        [Required]
        public Guid SchoolID { get; set; }

        [StringLength(50)]
        public string SchoolName { get; set; }

        [StringLength(80)]
        public string SchoolAddress1 { get; set; }

        [StringLength(80)]
        public string SchoolAddress2 { get; set; }

        [StringLength(80)]
        public string SchoolTown { get; set; }

        [StringLength(80)]
        public string SchoolState { get; set; }

        [StringLength(10)]
        public string SchoolZipCode { get; set; }

        [StringLength(50)]
        public string SchoolGrades { get; set; }

        public int SchoolMaxDownloadLicenses { get; set; }

        public int? SchoolLicenseExpirySeconds { get; set; }

        public TimeSpan? GradeInstructionHoursBegin { get; set; }

        public TimeSpan? GradeInstructionHoursEnd { get; set; }

        [StringLength(50)]
        public string SchoolOverrideCode { get; set; }

        public TimeSpan? GradePreloadHoursBegin { get; set; }

        public TimeSpan? GradePreloadHoursEnd { get; set; }

        [StringLength(50)]
        public string SchoolUseCacheServer { get; set; }

        public int? SchoolUserPolicy { get; set; }

        [StringLength(200)]
        public string SchoolAnnotation { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime Created { get; set; }

        public ICollection<AccessPointDto> AccessPoints { get; set; }

        public ICollection<ClassroomDto> Classrooms { get; set; }

        public DistrictDto District { get; set; }

        public ICollection<LicenseDto> Licenses { get; set; }

        public ICollection<LicenseRequestDto> LicenseRequests { get; set; }
    }
}
