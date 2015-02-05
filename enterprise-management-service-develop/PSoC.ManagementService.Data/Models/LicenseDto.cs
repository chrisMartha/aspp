using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using PSoC.ManagementService.Data.Interfaces;

namespace PSoC.ManagementService.Data.Models
{
    public class LicenseDto : IEntity
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

        public DateTime LicenseExpiryDateTime
        {
            get; set;
        }

        public DateTime LicenseIssueDateTime
        {
            get; set;
        }

        [Key]
        [Required]
        public LicenseRequestDto LicenseRequest
        {
            get; set;
        }

        public SchoolDto School
        {
            get; set;
        }

        [Required]
        [StringLength(17)]
        public string WifiBSSID
        {
            get; set;
        }
    }
}
