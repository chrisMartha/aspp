using System;

using PSoC.ManagementService.Data.Models;

namespace PSoC.ManagementService.Services.Models
{
    /// <summary>
    /// Model to represent valid Device License
    /// </summary>
    public class License
    {
        public LicenseRequest LicenseRequest { get; set; }
        public string ConfigCode { get; set; }
        public AccessPoint Accesspoint { get; set; }
        public District District { get; set; }
        public School School { get; set; }
        public DateTime LicenseIssueDateTime { get; set; }
        public DateTime LicenseExpiryDateTime { get; set; }

        public bool DynamicAccessPoint
        {
            get { return (District == null || School == null); }
        }

        public static explicit operator License(LicenseDto license)
        {
            if (license == null) return null;

            return new License()
            {
                LicenseRequest = (LicenseRequest)license.LicenseRequest,
               ConfigCode = license.ConfigCode,
               LicenseExpiryDateTime = license.LicenseExpiryDateTime,
               LicenseIssueDateTime = license.LicenseIssueDateTime,
               School = license.School == null ? null : new School(license.School),
               Accesspoint = license.AccessPoint == null ? null : (AccessPoint)license.AccessPoint,
               District = (license.AccessPoint == null || license.AccessPoint.District == null)  ? null : (District)license.AccessPoint.District,
            };
        }
    }
}
