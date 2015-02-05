using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using PSoC.ManagementService.Data.Interfaces;

namespace PSoC.ManagementService.Data.Models
{
    /// <summary>
    /// Access point DTO
    /// </summary>
    public class AccessPointDto : IEntity
    {        
        [StringLength(200)]
        public string AccessPointAnnotation
        {
            get; set;
        }

        public int? AccessPointExpiryTimeSeconds
        {
            get; set;
        }
     
        public int AccessPointMaxDownloadLicenses { get; set; }

        [StringLength(50)]
        public string AccessPointModel
        {
            get; set;
        }

        public ClassroomDto Classroom
        {
            get; set;
        }

        [Column(TypeName = "datetime2")]
        public DateTime Created
        {
            get; set;
        }

        public DistrictDto District
        {
            get; set;
        }

        public SchoolDto School
        {
            get; set;
        }

        [Key]
        [Required]
        [StringLength(17)]
        public string WifiBSSID
        {
            get; set;
        }

        [Required]
        [StringLength(32)]
        public string WifiSSID
        {
            get; set;
        }
    }
}
