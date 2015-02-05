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
    public class CourseDto : IEntity
    {
        #region Constructors

        public CourseDto()
        {
            DeviceInstalledCourses = new HashSet<DeviceInstalledCourseDto>();
        }

        #endregion Constructors

        [StringLength(80)]
        public string CourseAnnotation
        {
            get; set;
        }

        [Key]
        [Required]
        public Guid CourseLearningResourceID
        {
            get; set;
        }

        [StringLength(50)]
        public string CourseName
        {
            get; set;
        }

        [Column(TypeName = "datetime2")]
        public DateTime Created
        {
            get; set;
        }

        public ICollection<DeviceInstalledCourseDto> DeviceInstalledCourses
        {
            get; set;
        }

        [StringLength(2)]
        public string Grade
        {
            get; set;
        }

        [StringLength(20)]
        public string Subject
        {
            get; set;
        }
    }
}