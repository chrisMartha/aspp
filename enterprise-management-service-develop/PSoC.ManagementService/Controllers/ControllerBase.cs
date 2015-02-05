using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using PSoC.ManagementService.Services.Logging;

namespace PSoC.ManagementService.Controllers
{
    public abstract class ControllerBase : ApiController
    {
        protected LogUser CurrentUser { get; set; }
        protected LogRequest LogRequest { get; set; }
        private readonly DateTime _startTime = DateTime.UtcNow;
        private String _ipAddress;
        private String _userAgent;

        /// <summary>
        /// Execute a single HTTP operation
        /// </summary>
        /// <param name="controllerContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override async Task<HttpResponseMessage> ExecuteAsync(HttpControllerContext controllerContext,
            CancellationToken cancellationToken)
        {
            var req = controllerContext.Request;

            // Execute request
            var pageStartTime = DateTime.UtcNow;
            Int64 requestlength = 0;
            Int64 responseLength = 0;
            var content = req.Content;

            // Get client info
            _ipAddress = GetIpAddressFromRequest(req);
            _userAgent = GetUserAgent(req);
            CurrentUser = new LogUser
            {
                IpAddress = _ipAddress,
                UserAgent = _userAgent,
                StartTime = pageStartTime,
                Id = GetUserId(controllerContext)
            };

            // Get request length
            if ((content != null) && (content.Headers != null))
            {
                requestlength = content.Headers.ContentLength.GetValueOrDefault();
                if (requestlength == 0)
                    requestlength = (await content.ReadAsStringAsync().ConfigureAwait(false)).Length;
            }

            // Create the log request
            LogRequest = new LogRequest
            {
                Logger = GetType().FullName,
                Timestamp = DateTime.UtcNow,
                UserId = CurrentUser.Id,
                Level = LogLevel.Trace,
                ThreadId = Thread.CurrentThread.ManagedThreadId,
                RequestLength = requestlength,
                IpAddress = _ipAddress,
                UserAgent = _userAgent,
                HttpMethod = req.Method,
                Url = req.RequestUri.ToString()
            };

            try
            {
                var response = await base.ExecuteAsync(controllerContext, cancellationToken).ConfigureAwait(false);

                // Get response length
                if ((response.Content != null) && (response.Content.Headers != null))
                {
                    responseLength = response.Content.Headers.ContentLength.GetValueOrDefault();
                    if (responseLength == 0)
                        responseLength = (await response.Content.ReadAsStringAsync().ConfigureAwait(false)).Length;
                }

                // Get time
                var totalDuration = new TimeSpan(DateTime.UtcNow.Ticks - _startTime.Ticks).TotalMilliseconds;

                // Add response info to log request
                LogRequest.ResponseLength = responseLength;
                LogRequest.Duration = (long)totalDuration;
                LogRequest.HttpStatusCode = response.StatusCode;

                return response;
            }
            finally
            {
                // Write the log request
                PEMSEventSource.Log.WriteLog(LogRequest);
            }
        }

        #region Client Info
        /// <summary>
        /// Get client's user agent
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        protected static String GetUserAgent(HttpRequestMessage request)
        {
            if ((request == null) || (request.Headers == null))
            {
                return null;
            }

            var userAgent = request.Headers.UserAgent;
            if (userAgent != null)
            {
                var userAgentString = userAgent.ToString();
                if (!String.IsNullOrWhiteSpace(userAgentString))
                {
                    // Limit length for security purpose
                    return (userAgentString.Length > 4000)
                        ? String.Format("{0}...", userAgentString.Substring(0, 3995))
                        : userAgentString;
                }
            }

            return null;
        }

        /// <summary>
        /// Parses the Authorization header and creates user credentials
        /// </summary>
        /// <param name="actionContext"></param>
        protected static String GetUserId(HttpControllerContext actionContext)
        {
            // TODO: Implement if user ID can be exposed.
            return null;
        }

        /// <summary>
        /// Get client's IP Address from request message
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        protected static String GetIpAddressFromRequest(HttpRequestMessage request)
        {
            if (request == null)
            {
                return null;
            }

            Object value;

            // Web-hosting
            const String WebHostingContext = "MS_HttpContext";
            if (request.Properties.TryGetValue(WebHostingContext, out value))
            {
                if (value != null)
                {
                    var ctx = (HttpContextBase)value;
                    return ctx.Request.UserHostAddress;
                }
            }

            return null;
        }
        #endregion
    }
}