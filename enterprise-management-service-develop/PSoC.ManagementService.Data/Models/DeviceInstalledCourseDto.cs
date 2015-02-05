using System;
using System.ComponentModel.DataAnnotations;

using PSoC.ManagementService.Data.Interfaces;

namespace PSoC.ManagementService.Data.Models
{
    public class DeviceInstalledCourseDto : IEntity
    {
        public CourseDto Course
        {
            get;
            set;
        }

        [Key]
        [Required]
        public DeviceDto Device
        {
            get;
            set;
        }

        public DateTime? LastUpdated
        {
            get;
            set;
        }

        public decimal? PercentDownloaded
        {
            get;
            set;
        }
    }
}