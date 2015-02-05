using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using PSoC.ManagementService.Services.Models;

namespace PSoC.ManagementService.Models
{
    /// <summary>
    /// Access point setting view model
    /// </summary>
    public class AccessPointSettingsViewModel
    {
        public IList<AccessPoint> AccessPoints { get; set; }

        public AccessPoint SelectedAccessPoint { get; set; }

        [Required]
        public String WifiBSSId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please input a value greater than 0")]
        [Display(Name = "Max Downloads for Access Point")]
        public Int32 AccessPointMaxDownloadLicenses { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please input a value greater than 0")]
        [Display(Name = "Download License Timeout (Seconds)")]
        public Int32 AccessPointExpiryTimeSeconds { get; set; }

        public String Message { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public AccessPointSettingsViewModel()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="accessPoints"></param>
        /// <param name="wifiBSSId"></param>
        public AccessPointSettingsViewModel(IList<AccessPoint> accessPoints, String wifiBSSId)
        {
            AccessPoints = accessPoints;
            SelectedAccessPoint = (!String.IsNullOrEmpty(wifiBSSId)) ? accessPoints.FirstOrDefault(d => d.WifiBSSId == wifiBSSId) : accessPoints.FirstOrDefault();
        }
    }
}