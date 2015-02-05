using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using PSoC.ManagementService.Core;
using PSoC.ManagementService.Data.Interfaces;

namespace PSoC.ManagementService.Data.Models
{
    public class LicenseRequestDto : IEntity
    {
        public AccessPointDto AccessPoint
        {
            get; set;
        }

        [Required]
        [StringLength(50)]
        public string ConfigCode
        {
            get; set;
        }

        [Column(TypeName = "datetime2")]
        public DateTime Created
        {
            get; set;
        }

        public DeviceDto Device
        {
            get; set;
        }

        public int? LearningContentQueued
        {
            get; set;
        }

        public LicenseDto License
        {
            get; set;
        }

        [Key]
        [Required]
        public Guid LicenseRequestID
        {
            get; set;
        }

        public LicenseRequestType LicenseRequestType
        {
            get; set;
        }

        [StringLength(50)]
        public string LocationId
        {
            get; set;
        }

        [StringLength(50)]
        public string LocationName
        {
            get; set;
        }

        public DateTime RequestDateTime
        {
            get; set;
        }

        [StringLength(50)]
        public string Response
        {
            get; set;
        }

        public DateTime? ResponseDateTime
        {
            get; set;
        }

        public Guid? PrevLicenseRequestID
        {
            get;
            set;
        }

        public SchoolDto School
        {
            get; set;
        }

        public UserDto User
        {
            get; set;
        }
    }
}