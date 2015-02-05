using System;
using System.Net;
using System.Net.Http;

namespace PSoC.ManagementService.Services.Logging
{
    /// <summary>
    /// Log request
    /// </summary>
    public class LogRequest
    {
        public String Logger { get; set; }
        public DateTime? Timestamp { get; set; }
        public LogLevel Level { get; set; }
        public String UserId { get; set; }
        public String Message { get; set; }
        public Exception Exception { get; set; }
        public Int32? ThreadId { get; set; }
        public Int64? RequestLength { get; set; }
        public Int64? ResponseLength { get; set; }
        public Int64? Duration { get; set; }
        public String IpAddress { get; set; }
        public String UserAgent { get; set; }
        public LogUser User
        {
            set
            {
                UserId = value.Id;
                IpAddress = value.IpAddress;
                UserAgent = value.UserAgent;
            }
        }
        // Additional native fields
        public Int32? EventId { get; set; }
        public String Keywords { get; set; }
        public String Task { get; set; }
        public String InstanceName { get; set; }
        public Int32? ProcessId { get; set; }
        public HttpMethod HttpMethod { get; set; }
        public String Url { get; set; }
        public HttpStatusCode HttpStatusCode { get; set; }
        public String EventSource { get; set; }
        public String EventDestination { get; set; }
        public String Event { get; set; }
        public String EventDescription { get; set; }
        // Custom fields
        public String DistrictId { get; set; }
        public String SchoolId { get; set; }
        public String ClassroomId { get; set; }
        public String AccessPointId { get; set; }
        public String DeviceId { get; set; }
        public String AppId { get; set; }
        public String LicenseRequestId { get; set; }
        public String ConfigCode { get; set; }
        public Int32? DownloadRequested { get; set; }
        public Int32? ItemsQueued { get; set; }
        public String GrantDenyDecision { get; set; }
        public Int32? CountByAccessPoint { get; set; }
        public Int32? CountBySchool { get; set; }
        public Int32? CountByDistrict { get; set; }
    }
}
