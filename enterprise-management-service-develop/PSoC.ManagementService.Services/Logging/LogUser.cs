using System;

namespace PSoC.ManagementService.Services.Logging
{
    /// <summary>
    /// Log user
    /// </summary>
    public class LogUser
    {
        public String Id { get; set; }
        public String IpAddress { get; set; }
        public DateTime StartTime { get; set; }
        public String UserAgent { get; set; }
    }
}
