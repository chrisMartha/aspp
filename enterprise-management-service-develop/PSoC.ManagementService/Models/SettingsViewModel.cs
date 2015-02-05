using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PSoC.ManagementService.Models
{
    public class SettingsViewModel
    {
        [Range(3600, int.MaxValue, ErrorMessage = "Value must be between 3600 and 2147483647")]
        public int DownloadLicenseTimeout { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Value must be between 0 and 2147483647")]
        public int MaxDownloadsPerClass { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Value must be between 0 and 2147483647")]
        public int MaxDownloadsPerCustomer { get; set; }
    }
}