using System;

using PSoC.ManagementService.Core;
using PSoC.ManagementService.Data.Models;

namespace PSoC.ManagementService.Services.Models
{
    /// <summary>
    /// Model to represent a LicenseRequest
    /// </summary>
    public class LicenseRequest
    {
        public Guid LicenseRequestId { get; set; }
        public LicenseRequestType RequestType { get; set; }
        public Guid UserId { get; set; }
        public Guid DeviceId { get; set; }
        public string LocationId { get; set; }
        public string LocationName { get; set; }
        public int? LearningContentQueued { get; set; }
        public DateTime Created { get; set; }
        public DateTime RequestDateTime { get; set; }
        public DateTime ResponseDateTime { get; set; }
        public string Response { get; set; }

        public static explicit operator LicenseRequest(LicenseRequestDto licenseRequest)
        {
            if (licenseRequest == null) return null;

            return new LicenseRequest()
            {
                LicenseRequestId = licenseRequest.LicenseRequestID,
                RequestType = licenseRequest.LicenseRequestType,
                UserId = licenseRequest.User.UserID,
                DeviceId = licenseRequest.Device.DeviceID,
                LocationId = licenseRequest.LocationId,
                LocationName = licenseRequest.LocationName,
                LearningContentQueued = licenseRequest.LearningContentQueued,
                Created = licenseRequest.Created,
                RequestDateTime = licenseRequest.RequestDateTime,
                Response = licenseRequest.Response,
                ResponseDateTime = licenseRequest.RequestDateTime
            };
        }
    }
}
