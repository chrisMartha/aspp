using System;
using PSoC.ManagementService.Services.Logging;
using PSoC.ManagementService.Services.Models;

namespace PSoC.ManagementService.Services.Interfaces
{
    /// <summary>
    /// PEMSEventSource interface
    /// </summary>
    public interface IPEMSEventSource
    {
        ILogger Logger { get; set; }

        /// <summary>
        /// Write a log
        /// </summary>
        /// <param name="request"></param>
        void WriteLog(LogRequest request);
        void PingLog();
        Boolean ConfigureLogAll();
        void ApplicationStart(string message);
        void ApplicationConfigureLog(string message);
        void ApplicationFailure(string message, LogRequest logRequest = null);
        void ApplicationException(string message, LogRequest logRequest = null);
        void AccountLoginFailed(string message, LogRequest logRequest = null);
        void SchoolnetAuthorizationRequested(string restUrl, LogRequest logRequest = null);
        void SchoolnetAuthorizationSucceeded(LogRequest logRequest = null);
        void SchoolnetAuthorizationFailure(string message, string data, LogRequest logRequest = null);
        void AzureTableServiceInvalidTable(string tableEndpoint, string tableName);
        void AzureTableServiceException(string message);
        void AzureTableServiceInsertException(string message);
        void AzureTableServiceDeleteException(string message);
        void AzureTableServiceGetException(string message);
        void DeviceApiGetFailure(string message, LogRequest logRequest = null);
        void DeviceApiGetFailure(string message, string deviceId, LogRequest logRequest = null, bool writeLog = false);
        void DeviceApiPutFailure(string message, string deviceId, LogRequest logRequest = null, bool writeLog = false);
        void DeviceApiDeleteBadRequest(string message, string deviceId, LogRequest logRequest = null, bool writeLog = false);
        void DeviceApiDeleteItemNotFound(string message, string deviceId, LogRequest logRequest = null, bool writeLog = false);
        void DeviceApiLicenseRequested(string message, string deviceId, LogRequest logRequest = null, bool writeLog = false);
        void DeviceApiLicenseReturned(string message, string deviceId, LogRequest logRequest = null, bool writeLog = false);
        void DeviceApiLicenseRevoked(string message, string deviceId, LogRequest logRequest = null, bool writeLog = false);
        void DeviceApiLicenseFailed(string message, string deviceId, LogRequest logRequest = null, bool writeLog = false);
        void DeviceApiLicenseSucceeded(string message, string deviceId, LogRequest logRequest = null, bool writeLog = false);
        void DeviceApiReportDownloadStatusFailure(string message, string deviceId, LogRequest logRequest = null, bool writeLog = false);
        void DeviceServiceException(string message, LogRequest logRequest = null);
        void DeviceServiceGetDevicesRequested(string message, LogRequest logRequest = null);
        void DeviceServiceInsertUpdate(string message, string deviceId, LogRequest logRequest = null);
        void DeviceServiceDelete(string message, string deviceId, LogRequest logRequest = null);
        void DeviceServiceSaveDownloadStatusException(string message, string deviceId, LogRequest logRequest = null);
        void DeviceServiceGetAccessPointDeviceStatusException(string message, LogRequest logRequest = null);
        void TimeoutServiceStarted(string message);
        void TimeoutServiceShuttingDown(string message);
        void TimeoutServiceException(string message);
        void TimeoutServiceStartingEvaluator(string message);
        void TimeoutServiceStoppingEvaluator(string message);
        void TimeoutServiceConfigChange(string message, string configurationSettingName);
        void TimeoutServiceConfigureLog(string message);
        void TimeoutServiceLicenses(string message, int numberOfLicenses);
        void LicenseServiceException(string message, LogRequest logRequest = null);
        void LicenseServiceMaxExceeded(string message, int customerMax, int current, LogRequest logRequest = null);
        void WhiteListServiceException(string message, LogRequest logRequest = null);
        void WhiteListServiceRequested(string message, string environmentId, string userId, LogRequest logRequest = null);
        void DistrictServiceException(string message, string districtId = null, string userId = null, LogRequest logRequest = null);
        void AccessPointServiceException(string message, string accessPointId = null, string userId = null, LogRequest logRequest = null);
        void AdminServiceException(string message, LogRequest logRequest = null);
        void SchoolServiceException(string message, string districtId = null, string schoolId = null, string userId = null, LogRequest logRequest = null);
        void UserServiceException(string message, LogRequest logRequest = null);
        LogRequest Append(DeviceLicenseRequest req, LogRequest logRequest);
    }
}
