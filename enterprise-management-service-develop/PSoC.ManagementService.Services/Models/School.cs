using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using PSoC.ManagementService.Data.Models;

namespace PSoC.ManagementService.Services.Models
{
    /// <summary>
    /// Service level model for school
    /// </summary>
    public class School
    {
        public School() { }

        public School(SchoolDto school)
        {
            SchoolId = school.SchoolID;
            District = school.District == null ? null : (District) school.District;
            SchoolName = school.SchoolName;
            SchoolMaxDownloadLicenses = school.SchoolMaxDownloadLicenses;
            SchoolLicenseExpirySeconds = school.SchoolLicenseExpirySeconds;
            SchoolAnnotation = school.SchoolAnnotation;
        }

        public Guid SchoolId { get; set; }

        public District District { get; set; }

        [Required]
        [StringLength(50)]
        public string SchoolName { get; set; }

        public int SchoolMaxDownloadLicenses { get; set; }

        public int? SchoolLicenseExpirySeconds { get; set; }

        public string SchoolAnnotation { get; set; }

        /// <summary>
        /// Cast SchoolDto to School
        /// </summary>
        /// <param name="school"></param>
        /// <returns></returns>
        public static explicit operator School(SchoolDto school)
        {
            if (school == null) return null;

            return new School(school);
        }

        /// <summary>
        /// Cast School to SchoolDto
        /// </summary>
        /// <param name="school"></param>
        /// <returns></returns>
        public static explicit operator SchoolDto(School school)
        {
            if (school == null) return null;

            return new SchoolDto
            {
                SchoolID = school.SchoolId,
                District = school.District == null ? null : (DistrictDto)school.District,
                SchoolName = school.SchoolName,
                SchoolMaxDownloadLicenses = school.SchoolMaxDownloadLicenses,
                SchoolLicenseExpirySeconds = school.SchoolLicenseExpirySeconds,
                SchoolAnnotation = school.SchoolAnnotation,
            };
        }       
    }
}
