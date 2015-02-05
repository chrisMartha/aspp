using System;
using System.Diagnostics.Tracing;
using System.Linq;
using Microsoft.WindowsAzure;
using NLog;
using NLog.Config;
using PSoC.ManagementService.Services.Interfaces;
using PSoC.ManagementService.Services.Models;

namespace PSoC.ManagementService.Services.Logging
{
    /// <summary>
    /// A singleton object for logging events in PEMS
    /// </summary>
    [EventSource(Name = "PEMS")]
    public class PEMSEventSource : EventSource, IPEMSEventSource
    {
        public const String LogAllSettingName = "LogAll";
        public const String LogTargetName = "database";
        private static readonly Lazy<IPEMSEventSource> Instance = new Lazy<IPEMSEventSource>(() => new PEMSEventSource());

        public ILogger Logger { get; set; }

        private PEMSEventSource() { }

        public static IPEMSEventSource Log { get { return Instance.Value; } }

        /// <summary>
        /// Add custom keywords by incrementing the enum by 2
        /// </summary>
        [Flags]
        public enum Keywords
        {
            License = 1,
            WebAPI = 2,
            Login = 4,
            Schoolnet = 8,
            Exception = 16,
            DeviceService = 32,
            TimeoutService = 64,
            LicenseService = 128,
            WhiteListService = 256,
            DistrictService = 512,
            AccessPointService = 1024,
            AdminService = 2048,
            UserService = 4096,
            SchoolService = 8192
        }

        /// <summary>
        /// Add custom tasks
        /// </summary>
        public enum Tasks
        {
            WebAPIGet = 1,
            WebAPIPost = 2,
            WebAPIPut = 3,
            WebAPIDelete = 4
        }

        /// <summary>
        /// Write a log
        /// </summary>
        /// <param name="request"></param>
        public void WriteLog(LogRequest request)
        {
            var logger = Logger ?? NLogger.GetLogger(request.Logger);
            if (logger.IsEnabled(request.Level))
                logger.Log(request);
        }

        /// <summary>
        /// Ping log by creating sample logs for all levels
        /// </summary>
        public void PingLog()
        {
            foreach (var vp in NLogger.MapLogLevels)
            {
                if (vp.Key != LogLevel.Off)
                {
                    WriteLog(new LogRequest
                    {
                        Logger = LogAllSettingName,
                        Level = vp.Key,
                        Message = vp.Key.ToString()
                    });
                }
            }
        }

        /// <summary>
        /// Configure log all for debugging
        /// </summary>
        /// <returns></returns>
        public Boolean ConfigureLogAll()
        {
            // Add/remove LogAll rule
            Boolean enabled = false;
            var changedValue = CloudConfigurationManager.GetSetting(LogAllSettingName);
            var config = LogManager.Configuration;
            Func<LoggingRule, Boolean> logAllRule = r => (r.LoggerNamePattern == "*") && (r.Final);

            if (String.Equals(changedValue, Boolean.TrueString, StringComparison.OrdinalIgnoreCase))
            {
                // "true": Add catch-all trace rule if not exist
                var isFound = config.LoggingRules.Any(logAllRule);
                if (!isFound)
                {
                    var target = config.FindTargetByName(LogTargetName);
                    config.LoggingRules.Insert(0, new LoggingRule("*", NLog.LogLevel.Trace, target) { Final = true });
                    config.Reload();
                    enabled = true;
                }
            }
            else
            {
                // "false": Remove catch-all trace rule if exist
                var loggingRule = config.LoggingRules.FirstOrDefault(logAllRule);
                if (loggingRule != null)
                {
                    config.LoggingRules.Remove(loggingRule);
                    config.Reload();
                }
            }

            return enabled;
        }

        #region Application
        public void ApplicationStart(string message)
        {
            var req = new LogRequest
            {
                Logger = Keywords.WebAPI.ToString(),
                Level = LogLevel.Warn,
                Message = message,
                EventId = 103,
                Keywords = Keywords.WebAPI.ToString()
            };
            WriteLog(req);
        }

        public void ApplicationConfigureLog(string message)
        {
            var req = new LogRequest
            {
                Logger = Keywords.WebAPI.ToString(),
                Level = LogLevel.Warn,
                Message = message,
                EventId = 104,
                Keywords = Keywords.WebAPI.ToString()
            };
            WriteLog(req);
        }

        public void ApplicationFailure(string message, LogRequest logRequest = null)
        {
            var req = logRequest ?? new LogRequest();
            req.Logger = Keywords.Exception.ToString();
            req.Level = LogLevel.Fatal;
            req.Message = string.Format("Application Failure: {0}", message);
            req.EventId = 100;
            req.Keywords = Keywords.Exception.ToString();
            WriteLog(req);
        }

        public void ApplicationException(string message, LogRequest logRequest = null)
        {
            var req = logRequest ?? new LogRequest();
            req.Logger = Keywords.Exception.ToString();
            req.Level = LogLevel.Error;
            req.Message = string.Format("Exception Occured: {0}", message);
            req.EventId = 101;
            req.Keywords = Keywords.Exception.ToString();
            WriteLog(req);
        }

        public void AccountLoginFailed(string message, LogRequest logRequest = null)
        {
            var req = logRequest ?? new LogRequest();
            req.Logger = Keywords.Login.ToString();
            req.Level = LogLevel.Info;
            req.Message = string.Format("Exception Occured: {0}", message);
            req.EventId = 102;
            req.Keywords = Keywords.Login.ToString();
            WriteLog(req);
        }

        #endregion

        #region Schoolnet
        public void SchoolnetAuthorizationRequested(string restUrl, LogRequest logRequest = null)
        {
            var req = logRequest ?? new LogRequest();
            req.Logger = Keywords.Schoolnet.ToString();
            req.Level = LogLevel.Info;
            req.EventId = 200;
            req.Keywords = Keywords.Schoolnet.ToString();
            req.Url = restUrl;
            WriteLog(req);
        }

        public void SchoolnetAuthorizationSucceeded(LogRequest logRequest = null)
        {
            var req = logRequest ?? new LogRequest();
            req.Logger = Keywords.Schoolnet.ToString();
            req.Level = LogLevel.Info;
            req.EventId = 201;
            req.Keywords = Keywords.Schoolnet.ToString();
            WriteLog(req);
        }

        public void SchoolnetAuthorizationFailure(string message, string data, LogRequest logRequest = null)
        {
            var req = logRequest ?? new LogRequest();
            req.Logger = Keywords.Schoolnet.ToString();
            req.Level = LogLevel.Error;
            req.Message = string.Format("{0}\n[Data] {1}", message, data);
            req.EventId = 202;
            req.Keywords = (Keywords.Schoolnet | Keywords.Exception).ToString();
            WriteLog(req);
        }
        #endregion

        #region AzureTableService
        public void AzureTableServiceInvalidTable(string tableEndpoint, string tableName)
        {
            throw new NotImplementedException("Azure table is no longer supported.");
        }

        public void AzureTableServiceException(string message)
        {
            throw new NotImplementedException("Azure table is no longer supported.");
        }

        public void AzureTableServiceInsertException(string message)
        {
            throw new NotImplementedException("Azure table is no longer supported.");
        }

        public void AzureTableServiceDeleteException(string message)
        {
            throw new NotImplementedException("Azure table is no longer supported.");
        }

        public void AzureTableServiceGetException(string message)
        {
            throw new NotImplementedException("Azure table is no longer supported.");
        }
        #endregion

        #region DeviceApi

        public void DeviceApiGetFailure(string message, LogRequest logRequest = null)
        {
            DeviceApiGetFailure(message, null, logRequest);
        }

        public void DeviceApiGetFailure(string message, string deviceId, LogRequest logRequest = null, bool writeLog = false)
        {
            var req = logRequest ?? new LogRequest();
            req.Logger = Keywords.WebAPI.ToString();
            req.Level = LogLevel.Error;
            req.Message = message;
            req.EventId = 400;
            req.Task = Tasks.WebAPIGet.ToString();
            req.Keywords = (Keywords.WebAPI | Keywords.Exception).ToString();
            req.DeviceId = deviceId;
            if (writeLog)
                WriteLog(req);
        }

        public void DeviceApiPutFailure(string message, string deviceId, LogRequest logRequest = null, bool writeLog = false)
        {
            var req = logRequest ?? new LogRequest();
            req.Logger = Keywords.WebAPI.ToString();
            req.Level = LogLevel.Error;
            req.Message = message;
            req.EventId = 401;
            req.Task = Tasks.WebAPIPut.ToString();
            req.Keywords = (Keywords.WebAPI | Keywords.Exception).ToString();
            req.DeviceId = deviceId;
            if (writeLog)
                WriteLog(req);
        }

        public void DeviceApiDeleteBadRequest(string message, string deviceId, LogRequest logRequest = null, bool writeLog = false)
        {
            var req = logRequest ?? new LogRequest();
            req.Logger = Keywords.WebAPI.ToString();
            req.Level = LogLevel.Error;
            req.Message = message;
            req.EventId = 402;
            req.Task = Tasks.WebAPIDelete.ToString();
            req.Keywords = (Keywords.WebAPI | Keywords.Exception).ToString();
            req.DeviceId = deviceId;
            if (writeLog)
                WriteLog(req);
        }

        public void DeviceApiDeleteItemNotFound(string message, string deviceId, LogRequest logRequest = null, bool writeLog = false)
        {
            var req = logRequest ?? new LogRequest();
            req.Logger = Keywords.WebAPI.ToString();
            req.Level = LogLevel.Error;
            req.Message = message;
            req.EventId = 403;
            req.Task = Tasks.WebAPIDelete.ToString();
            req.Keywords = (Keywords.WebAPI | Keywords.Exception).ToString();
            req.DeviceId = deviceId;
            if (writeLog)
                WriteLog(req);
        }

        public void DeviceApiLicenseRequested(string message, string deviceId, LogRequest logRequest = null, bool writeLog = false)
        {
            var req = logRequest ?? new LogRequest();
            req.Logger = Keywords.WebAPI.ToString();
            req.Level = LogLevel.Info;
            req.Message = message;
            req.EventId = 404;
            req.Task = Tasks.WebAPIPut.ToString();
            req.Keywords = (Keywords.WebAPI | Keywords.License).ToString();
            req.DeviceId = deviceId;
            if (writeLog)
                WriteLog(req);
        }

        public void DeviceApiLicenseReturned(string message, string deviceId, LogRequest logRequest = null, bool writeLog = false)
        {
            var req = logRequest ?? new LogRequest();
            req.Logger = Keywords.WebAPI.ToString();
            req.Level = LogLevel.Info;
            req.Message = message;
            req.EventId = 408;
            req.Task = Tasks.WebAPIPut.ToString();
            req.Keywords = (Keywords.WebAPI | Keywords.License).ToString();
            req.DeviceId = deviceId;
            if (writeLog)
                WriteLog(req);
        }

        public void DeviceApiLicenseRevoked(string message, string deviceId, LogRequest logRequest = null, bool writeLog = false)
        {
            var req = logRequest ?? new LogRequest();
            req.Logger = Keywords.WebAPI.ToString();
            req.Level = LogLevel.Info;
            req.Message = message;
            req.EventId = 409;
            req.Task = Tasks.WebAPIPut.ToString();
            req.Keywords = (Keywords.WebAPI | Keywords.License).ToString();
            req.DeviceId = deviceId;
            if (writeLog)
                WriteLog(req);
        }

        public void DeviceApiLicenseFailed(string message, string deviceId, LogRequest logRequest = null, bool writeLog = false)
        {
            var req = logRequest ?? new LogRequest();
            req.Logger = Keywords.WebAPI.ToString();
            req.Level = LogLevel.Error;
            req.Message = message;
            req.EventId = 405;
            req.Task = Tasks.WebAPIPut.ToString();
            req.Keywords = (Keywords.WebAPI | Keywords.License).ToString();
            req.DeviceId = deviceId;
            if (writeLog)
                WriteLog(req);
        }

        public void DeviceApiLicenseSucceeded(string message, string deviceId, LogRequest logRequest = null, bool writeLog = false)
        {
            var req = logRequest ?? new LogRequest();
            req.Logger = Keywords.WebAPI.ToString();
            req.Level = LogLevel.Info;
            req.Message = message;
            req.EventId = 406;
            req.Task = Tasks.WebAPIPut.ToString();
            req.Keywords = (Keywords.WebAPI | Keywords.License).ToString();
            req.DeviceId = deviceId;
            if (writeLog)
                WriteLog(req);
        }

        public void DeviceApiReportDownloadStatusFailure(string message, string deviceId, LogRequest logRequest = null, bool writeLog = false)
        {
            var req = logRequest ?? new LogRequest();
            req.Logger = Keywords.WebAPI.ToString();
            req.Level = LogLevel.Error;
            req.Message = message;
            req.EventId = 407;
            req.Task = Tasks.WebAPIPost.ToString();
            req.Keywords = (Keywords.WebAPI | Keywords.Exception).ToString();
            req.DeviceId = deviceId;
            if (writeLog)
                WriteLog(req);
        }

        #endregion

        #region DeviceService

        public void DeviceServiceException(string message, LogRequest logRequest = null)
        {
            var req = logRequest ?? new LogRequest();
            req.Logger = Keywords.DeviceService.ToString();
            req.Level = LogLevel.Error;
            req.Message = message;
            req.EventId = 500;
            req.Keywords = (Keywords.DeviceService | Keywords.Exception).ToString();
            WriteLog(req);
        }

        public void DeviceServiceGetDevicesRequested(string message, LogRequest logRequest = null)
        {
            var req = logRequest ?? new LogRequest();
            req.Logger = Keywords.DeviceService.ToString();
            req.Level = LogLevel.Info;
            req.Message = message;
            req.EventId = 501;
            req.Keywords = Keywords.DeviceService.ToString();
            WriteLog(req);
        }

        public void DeviceServiceInsertUpdate(string message, string deviceId, LogRequest logRequest = null)
        {
            var req = logRequest ?? new LogRequest();
            req.Logger = Keywords.DeviceService.ToString();
            req.Level = LogLevel.Info;
            req.Message = message;
            req.EventId = 502;
            req.Keywords = Keywords.DeviceService.ToString();
            WriteLog(req);
        }

        public void DeviceServiceDelete(string message, string deviceId, LogRequest logRequest = null)
        {
            var req = logRequest ?? new LogRequest();
            req.Logger = Keywords.DeviceService.ToString();
            req.Level = LogLevel.Info;
            req.Message = message;
            req.EventId = 503;
            req.Keywords = Keywords.DeviceService.ToString();
            WriteLog(req);
        }

        public void DeviceServiceSaveDownloadStatusException(string message, string deviceId, LogRequest logRequest = null)
        {
            var req = logRequest ?? new LogRequest();
            req.Logger = Keywords.DeviceService.ToString();
            req.DeviceId = deviceId;
            req.Level = LogLevel.Error;
            req.Message = message;
            req.EventId = 504;
            req.Keywords = Keywords.DeviceService.ToString();
            WriteLog(req);
        }

        public void DeviceServiceGetAccessPointDeviceStatusException(string message, LogRequest logRequest = null)
        {
            var req = logRequest ?? new LogRequest();
            req.Logger = Keywords.DeviceService.ToString();
            req.Level = LogLevel.Error;
            req.Message = message;
            req.EventId = 505;
            req.Keywords = Keywords.DeviceService.ToString();
            WriteLog(req);
        }

        #endregion

        #region TimeoutService

        public void TimeoutServiceStarted(string message)
        {
            var req = new LogRequest
            {
                Logger = Keywords.TimeoutService.ToString(),
                Level = LogLevel.Warn,
                Message = message,
                EventId = 600,
                Keywords = Keywords.TimeoutService.ToString()
            };
            WriteLog(req);
        }

        public void TimeoutServiceShuttingDown(string message)
        {
            var req = new LogRequest
            {
                Logger = Keywords.TimeoutService.ToString(),
                Level = LogLevel.Warn,
                Message = message,
                EventId = 601,
                Keywords = Keywords.TimeoutService.ToString()
            };
            WriteLog(req);
        }

        public void TimeoutServiceException(string message)
        {
            var req = new LogRequest
            {
                Logger = Keywords.TimeoutService.ToString(),
                Level = LogLevel.Error,
                Message = message,
                EventId = 602,
                Keywords = (Keywords.TimeoutService | Keywords.Exception).ToString()
            };
            WriteLog(req);
        }

        public void TimeoutServiceStartingEvaluator(string message)
        {
            var req = new LogRequest
            {
                Logger = Keywords.TimeoutService.ToString(),
                Level = LogLevel.Warn,
                Message = message,
                EventId = 603,
                Keywords = Keywords.TimeoutService.ToString()
            };
            WriteLog(req);
        }

        public void TimeoutServiceStoppingEvaluator(string message)
        {
            var req = new LogRequest
            {
                Logger = Keywords.TimeoutService.ToString(),
                Level = LogLevel.Warn,
                Message = message,
                EventId = 604,
                Keywords = Keywords.TimeoutService.ToString()
            };
            WriteLog(req);
        }

        public void TimeoutServiceConfigChange(string message, string configurationSettingName)
        {
            var req = new LogRequest
            {
                Logger = Keywords.TimeoutService.ToString(),
                Level = LogLevel.Warn,
                Message = string.Format(message, configurationSettingName),
                EventId = 605,
                Keywords = Keywords.TimeoutService.ToString()
            };
            WriteLog(req);
        }

        public void TimeoutServiceLicenses(string message, int numberOfLicenses)
        {
            var req = new LogRequest
            {
                Logger = Keywords.TimeoutService.ToString(),
                Level = LogLevel.Info,
                Message = string.Format(message, numberOfLicenses),
                EventId = 606,
                Keywords = Keywords.TimeoutService.ToString(),
                DownloadRequested = numberOfLicenses
            };
            WriteLog(req);
        }

        public void TimeoutServiceConfigureLog(string message)
        {
            var req = new LogRequest
            {
                Logger = Keywords.TimeoutService.ToString(),
                Level = LogLevel.Warn,
                Message = string.Format(message),
                EventId = 607,
                Keywords = Keywords.TimeoutService.ToString()
            };
            WriteLog(req);
        }
        #endregion
		
		#region LicenseService
        public void LicenseServiceException(string message, LogRequest logRequest = null)
        {
            var req = logRequest ?? new LogRequest();
            req.Logger = Keywords.LicenseService.ToString();
            req.Level = LogLevel.Error;
            req.Message = message;
            req.EventId = 700;
            req.Keywords = (Keywords.LicenseService | Keywords.Exception).ToString();
            WriteLog(req);
        }

        public void LicenseServiceMaxExceeded(string message, int customerMax, int current, LogRequest logRequest = null)
        {
            var req = logRequest ?? new LogRequest();
            req.Logger = Keywords.LicenseService.ToString();
            req.Level = LogLevel.Info;
            req.Message = message;
            req.EventId = 701;
            req.Keywords = Keywords.LicenseService.ToString();
            WriteLog(req);
        }

        #endregion

        #region WhiteListService

        public void WhiteListServiceException(string message, LogRequest logRequest = null)
        {
            var req = logRequest ?? new LogRequest();
            req.Logger = Keywords.WhiteListService.ToString();
            req.Level = LogLevel.Error;
            req.Message = message;
            req.EventId = 800;
            req.Keywords = Keywords.WhiteListService.ToString();
            WriteLog(req);
        }

        public void WhiteListServiceRequested(string message, string environmentId, string userId, LogRequest logRequest = null)
        {
            var req = logRequest ?? new LogRequest();
            req.Logger = Keywords.WhiteListService.ToString();
            req.Level = LogLevel.Info;
            req.UserId = userId;
            req.Message = message;
            req.EventId = 801;
            req.Keywords = Keywords.WhiteListService.ToString();
            req.ConfigCode = environmentId;
            WriteLog(req);
        }

        #endregion

        #region DistrictService
        public void DistrictServiceException(string message, string districtId = null, string userId = null, LogRequest logRequest = null)
        {
            var req = logRequest ?? new LogRequest();
            req.Logger = Keywords.DistrictService.ToString();
            req.Level = LogLevel.Error;
            req.UserId = userId;
            req.Message = message;
            req.EventId = 900;
            req.Keywords = (Keywords.DistrictService | Keywords.Exception).ToString();
            req.DistrictId = districtId;
            WriteLog(req);
        }

        public void DistrictServiceException(string message, LogRequest logRequest = null)
        {
            var req = logRequest ?? new LogRequest();
            req.Logger = Keywords.DistrictService.ToString();
            req.Level = LogLevel.Error;
            req.Message = message;
            req.EventId = 901;
            req.Keywords = (Keywords.DistrictService | Keywords.Exception).ToString();
            WriteLog(req);
        }
        #endregion

        #region AccessPointService
        public void AccessPointServiceException(string message, string accessPointId = null, string userId = null, LogRequest logRequest = null)
        {
            var req = logRequest ?? new LogRequest();
            req.Logger = Keywords.AccessPointService.ToString();
            req.Level = LogLevel.Error;
            req.UserId = userId;
            req.Message = message;
            req.EventId = 1000;
            req.Keywords = (Keywords.AccessPointService | Keywords.Exception).ToString();
            req.AccessPointId = accessPointId;
            WriteLog(req);
        }
        #endregion

        #region AdminService
        public void AdminServiceException(string message, LogRequest logRequest = null)
        {
            var req = logRequest ?? new LogRequest();
            req.Logger = Keywords.AdminService.ToString();
            req.Level = LogLevel.Error;
            req.Message = message;
            req.EventId = 1100;
            req.Keywords = (Keywords.AdminService | Keywords.Exception).ToString();
            WriteLog(req);
        }
        #endregion

        #region SchoolService
        public void SchoolServiceException(string message, string districtId = null, string schoolId = null, string userId = null, LogRequest logRequest = null)
        {
            var req = logRequest ?? new LogRequest();
            req.Logger = Keywords.SchoolService.ToString();
            req.Level = LogLevel.Error;
            req.UserId = userId;
            req.Message = message;
            req.EventId = 1300;
            req.Keywords = (Keywords.SchoolService | Keywords.Exception).ToString();
            req.DistrictId = districtId;
            req.SchoolId = schoolId;
            WriteLog(req);
        }
        #endregion

        #region UserService
        public void UserServiceException(string message, LogRequest logRequest = null)
        {
            var req = logRequest ?? new LogRequest();
            req.Logger = Keywords.UserService.ToString();
            req.Level = LogLevel.Error;
            req.Message = message;
            req.EventId = 1200;
            req.Keywords = (Keywords.UserService | Keywords.Exception).ToString();
            WriteLog(req);
        }
        #endregion

        #region Models

        /// <summary>
        /// Append device license request to a log request
        /// </summary>
        /// <param name="req"></param>
        /// <param name="logRequest"></param>
        /// <returns></returns>
        public LogRequest Append(DeviceLicenseRequest req, LogRequest logRequest)
        {
            if (req == null) return logRequest;
            logRequest = logRequest ?? new LogRequest();

            logRequest.UserId = req.UserId;
            logRequest.AccessPointId = req.WifiBSSID;
            logRequest.DeviceId = req.DeviceId;
            logRequest.ConfigCode = req.LastUsedConfigCode;
            logRequest.AppId = String.Format("{0}/{1}", req.PSoCAppId, req.PSoCAppVersion);
            logRequest.DownloadRequested = (req.DownloadLicenseRequested) ? 1 : (int?) null;
            logRequest.ItemsQueued = req.LearningContentQueued;

            return logRequest;
        }
        #endregion
    }
}
