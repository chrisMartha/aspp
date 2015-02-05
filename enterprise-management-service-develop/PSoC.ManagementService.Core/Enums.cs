using System.ComponentModel;

namespace PSoC.ManagementService.Core
{
    #region Enumerations

    /// <summary>
    /// Denotes user types for the website (all website users are some sort of administrators)
    /// Description corresponds to UserType from Users table
    /// </summary>
    public enum AdminType
    {
        [Description("Global Admin")] 
        GlobalAdmin = 1,
        [Description("District Admin")]
        DistrictAdmin,
        [Description("School Admin")]
        SchoolAdmin
    }

    public enum LicenseRequestType
    {
        [Description("Device is requesting a new license or reporting it has a license and continuing download that results in slide of expiry time")]
        RequestLicense = 1,
        [Description("Device is returning the license")]
        ReturnLicense,
        [Description("Administrator has requested to revoke license already granted for a specific device")]
        RevokeLicense,
        [Description("This may come up from push (to distinguish pull request from push request)")]
        ServerGrant
    }

    #endregion Enumerations
}
