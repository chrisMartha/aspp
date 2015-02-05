using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using PSoC.ManagementService.Services.Models;

namespace PSoC.ManagementService.Models
{
    /// <summary>
    /// District setting view model
    /// </summary>
    public class DistrictSettingsViewModel
    {
        public IList<District> Districts { get; set; }

        public District SelectedDistrict { get; set; }

        [Required]
        public Guid? DistrictId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please input a value greater than 0")]
        [Display(Name = "Max Downloads for District")]
        public Int32 DistrictMaxDownloadLicenses { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please input a value greater than 0")]
        [Display(Name = "Download License Timeout (Seconds)")]
        public Int32 DistrictLicenseExpirySeconds { get; set; }

        public String Message { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public DistrictSettingsViewModel()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="districts"></param>
        /// <param name="districtId"></param>
        public DistrictSettingsViewModel(IList<District> districts, Guid? districtId)
        {
            Districts = districts;
            SelectedDistrict = districtId.HasValue ? districts.FirstOrDefault(d => d.DistrictId == districtId) : districts.FirstOrDefault();
        }
    }
}