using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Web.Http.Tracing;
using NLog;
using PSoC.ManagementService.Services.Interfaces;

namespace PSoC.ManagementService.Services.Logging
{
    /// <summary>
    /// Implementation of NLog
    /// </summary>
    public class NLogger : ITraceWriter, ILogger
    {
        #region "Vars and props"
        /// <summary>
        /// Map trace level to app log level
        /// </summary>
        public readonly static Dictionary<TraceLevel, LogLevel> MapTraceLevels =
            new Dictionary<TraceLevel, LogLevel>
                {
                    {TraceLevel.Off, LogLevel.Off},
                    {TraceLevel.Debug, LogLevel.Debug},
                    {TraceLevel.Info, LogLevel.Info},
                    {TraceLevel.Warn, LogLevel.Warn},
                    {TraceLevel.Error, LogLevel.Error},
                    {TraceLevel.Fatal, LogLevel.Fatal}
                };

        /// <summary>
        /// Map app log level to NLog log level
        /// </summary>
        public readonly static Dictionary<LogLevel, NLog.LogLevel> MapLogLevels =
            new Dictionary<LogLevel, NLog.LogLevel>
                {
                    {LogLevel.Off, NLog.LogLevel.Off},
                    {LogLevel.Debug, NLog.LogLevel.Debug},
                    {LogLevel.Trace, NLog.LogLevel.Trace},
                    {LogLevel.Info, NLog.LogLevel.Info},
                    {LogLevel.Warn, NLog.LogLevel.Warn},
                    {LogLevel.Error, NLog.LogLevel.Error},
                    {LogLevel.Fatal, NLog.LogLevel.Fatal},
                };

        private readonly Logger _logger;
        #endregion

        /// <summary>
        /// Constructor with log name
        /// </summary>
        /// <param name="name"></param>
        public NLogger(string name)
        {
            _logger = LogManager.GetLogger(name);
        }

        /// <summary>
        /// Get a new logger
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ILogger GetLogger(string name)
        {
            return new NLogger(name);
        }

        /// <summary>
        /// Determine if a log level is enabled.
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public bool IsEnabled(LogLevel level)
        {
            return _logger.IsEnabled(MapLogLevels[level]);
        }

        /// <summary>
        /// Log a request
        /// </summary>
        /// <param name="request"></param>
        public void Log(LogRequest request)
        {
            var logEventInfo = new LogEventInfo
            {
                LoggerName = request.Logger,
                TimeStamp = (request.Timestamp.HasValue) ? request.Timestamp.Value : DateTime.UtcNow,
                Level = MapLogLevels[request.Level],
                Message = (!string.IsNullOrEmpty(request.Message)) ? request.Message.Substring(0, Math.Min(request.Message.Length, 4000)) : null,
                Exception = request.Exception
            };
            logEventInfo.Properties.Add("UserId", request.UserId);
            logEventInfo.Properties.Add("RequestLength", request.RequestLength);
            logEventInfo.Properties.Add("ResponseLength", request.ResponseLength);
            logEventInfo.Properties.Add("Duration", request.Duration);
            logEventInfo.Properties.Add("IpAddress", request.IpAddress);
            logEventInfo.Properties.Add("UserAgent", request.UserAgent);
            logEventInfo.Properties.Add("EventId", request.EventId);
            logEventInfo.Properties.Add("Keywords", request.Keywords);
            logEventInfo.Properties.Add("Task", request.Task);
            logEventInfo.Properties.Add("HttpMethod", request.HttpMethod);
            logEventInfo.Properties.Add("Url", request.Url);
            logEventInfo.Properties.Add("HttpStatusCode", (Int16)request.HttpStatusCode);
            logEventInfo.Properties.Add("EventSource", request.EventSource);
            logEventInfo.Properties.Add("EventDestination", request.EventDestination);
            logEventInfo.Properties.Add("Event", request.Event);
            logEventInfo.Properties.Add("EventDescription", request.EventDescription);
            logEventInfo.Properties.Add("DistrictId", request.DistrictId);
            logEventInfo.Properties.Add("SchoolId", request.SchoolId);
            logEventInfo.Properties.Add("ClassroomId", request.ClassroomId);
            logEventInfo.Properties.Add("AccessPointId", request.AccessPointId);
            logEventInfo.Properties.Add("DeviceId", request.DeviceId);
            logEventInfo.Properties.Add("AppId", request.AppId);
            logEventInfo.Properties.Add("LicenseRequestId", request.LicenseRequestId);
            logEventInfo.Properties.Add("ConfigCode", request.ConfigCode);
            logEventInfo.Properties.Add("DownloadRequested", request.DownloadRequested);
            logEventInfo.Properties.Add("ItemsQueued", request.ItemsQueued);
            logEventInfo.Properties.Add("GrantDenyDecision", request.GrantDenyDecision);
            logEventInfo.Properties.Add("CountByAccessPoint", request.CountByAccessPoint);
            logEventInfo.Properties.Add("CountBySchool", request.CountBySchool);
            logEventInfo.Properties.Add("CountByDistrict", request.CountByDistrict);
            _logger.Log(logEventInfo);
        }

        /// <summary>
        /// Trace a request message
        /// </summary>
        /// <param name="request"></param>
        /// <param name="category"></param>
        /// <param name="level"></param>
        /// <param name="traceAction"></param>
        public void Trace(HttpRequestMessage request, string category, TraceLevel level, Action<TraceRecord> traceAction)
        {
            if (IsEnabled(LogLevel.Trace))
            {
                var record = new TraceRecord(request, category, level);
                traceAction(record);
                ProcessTrace(record);
            }
        }

        /// <summary>
        /// Process a trace record
        /// </summary>
        /// <param name="record"></param>
        private void ProcessTrace(TraceRecord record)
        {
            var sb = new StringBuilder();

            var request = record.Request;
            if (request != null)
            {
                var requestMethod = request.Method;
                if (requestMethod != null)
                    sb.Append(requestMethod);

                var requestUri = request.RequestUri;
                if (requestUri != null)
                    sb.Append(" ").Append(requestUri);
            }

            var category = record.Category;
            if (!string.IsNullOrWhiteSpace(category))
                sb.Append(" ").Append(category);

            var op = record.Operator;
            if (!string.IsNullOrWhiteSpace(op))
                sb.Append(" ").Append(op).Append(" ").Append(record.Operation);

            var message = record.Message;
            if (!string.IsNullOrWhiteSpace(message))
                sb.Append(" ").Append(message);

            var exception = record.Exception;
            if (exception != null)
            {
                var baseExceptionMessage = exception.GetBaseException().Message;
                if (!string.IsNullOrWhiteSpace(baseExceptionMessage))
                    sb.Append(" ").Append(baseExceptionMessage);
            }

            _logger.Log(MapLogLevels[MapTraceLevels[record.Level]], sb);
        }
    }
}
