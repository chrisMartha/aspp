﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using PSoC.ManagementService.Data.Interfaces;
using PSoC.ManagementService.Security;

namespace PSoC.ManagementService.Data.Models
{
    public class DeviceDto : IEntity
    {
        #region Constructors

        public DeviceDto()
        {
            DeviceInstalledCourses = new HashSet<DeviceInstalledCourseDto>();
            LicenseRequests = new HashSet<LicenseRequestDto>();
        }

        #endregion Constructors

        [StringLength(50)]
        public string ConfiguredGrades
        {
            get;
            set;
        }

        public int? ConfiguredUnitCount
        {
            get;
            set;
        }

        public DateTime? ContentLastUpdatedAt
        {
            get;
            set;
        }

        [Column(TypeName = "datetime2")]
        public DateTime Created
        {
            get;
            set;
        }

        [StringLength(80)]
        public string DeviceAnnotation
        {
            get;
            set;
        }

        public long? DeviceFreeSpace
        {
            get;
            set;
        }

        [Key]
        [Required]
        public Guid DeviceID
        {
            get;
            set;
        }

        public ICollection<DeviceInstalledCourseDto> DeviceInstalledCourses
        {
            get;
            set;
        }

        [StringLength(50)]
        public string DeviceName
        {
            get
            {
                if (DeviceNameEnc == null)
                    return null;

                return DeviceNameEnc.DecryptedValue;
            }
            set
            {
                DeviceNameEnc = value;
            }
        }

        [StringLength(50)]
        public string DeviceOS
        {
            get;
            set;
        }

        [StringLength(50)]
        public string DeviceOSVersion
        {
            get;
            set;
        }

        [StringLength(50)]
        public string DeviceType
        {
            get;
            set;
        }

        [StringLength(50)]
        public string GeoLocation
        {
            get;
            set;
        }

        public long? InstalledContentSize
        {
            get;
            set;
        }

        [StringLength(50)]
        public string LastUsedConfigCode
        {
            get;
            set;
        }

        public ICollection<LicenseRequestDto> LicenseRequests
        {
            get;
            set;
        }

        public string PSoCAppID
        {
            get;
            set;
        }

        [StringLength(50)]
        public string PSoCAppVersion
        {
            get;
            set;
        }

        public SchoolDto School
        {
            get;
            set;
        }

        public LicenseRequestDto LastLicenseRequest
        {
            get; set;
        }

        public bool CanRevoke
        {
            get; set;
        }

        internal EncrypedField<string> DeviceNameEnc { get; set; }
    }
}