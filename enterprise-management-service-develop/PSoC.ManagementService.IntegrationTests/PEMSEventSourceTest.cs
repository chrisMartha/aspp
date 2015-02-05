using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PSoC.ManagementService.Data.Helpers;
using PSoC.ManagementService.Services.Interfaces;
using PSoC.ManagementService.Services.Logging;

namespace PSoC.ManagementService.IntegrationTests
{
    [TestClass]
    public class PEMSEventSourceTest
    {
        private IPEMSEventSource _eventSource;
        private const string LoggerName = "PEMSEventSourceTest";
        private const string ConnectionStringName = "Log";

        [TestInitialize]
        public void TestInitialize()
        {
            _eventSource = PEMSEventSource.Log;
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            var rowsDeleted = DeleteTestLogs().Result;
        }

        [TestMethod]
        public async Task PEMSEventSourceTest_InsertedLogExists()
        {
            // Arrange
            const LogLevel logLevel = LogLevel.Info;
            var userId = Guid.NewGuid();
            var districtId = Guid.NewGuid();
            var schoolId = Guid.NewGuid();
            var classroomId = Guid.NewGuid();
            var accessPointId = Guid.NewGuid();
            var deviceId = Guid.NewGuid();
            var appId = Guid.NewGuid();
            var licenseRequestId = Guid.NewGuid();

            var logRequest = new LogRequest
            {
                Logger = LoggerName,
                Level = logLevel, 
                UserId = userId.ToString(),
                Message = "Test message",
                Exception = new Exception("Test exception"),
                RequestLength = 1234,
                ResponseLength = 5678,
                Duration = 1234,
                IpAddress = "123.255.255.255",
                UserAgent = "Test User Agent 1.0",
                EventId = 123,
                Keywords = "Test keywords",
                Task = "Test task",
                HttpMethod = HttpMethod.Get,
                Url = "http://www.pearson.com/api/v1/test",
                HttpStatusCode = HttpStatusCode.InternalServerError,
                EventSource = "Test event source",
                EventDestination = "Test destination",
                Event = "Test event",
                EventDescription = "Test description",
                DistrictId = districtId.ToString(),
                SchoolId = schoolId.ToString(),
                ClassroomId = classroomId.ToString(),
                AccessPointId = accessPointId.ToString(),
                DeviceId = deviceId.ToString(),
                AppId = appId.ToString(),
                LicenseRequestId = licenseRequestId.ToString(),
                ConfigCode = "CCSOCDCT",
                DownloadRequested = 123,
                ItemsQueued = 2345,
                GrantDenyDecision = "Grant",
                CountByAccessPoint = 123,
                CountBySchool = 234,
                CountByDistrict = 456
            };

            // Act
            _eventSource.WriteLog(logRequest);
            // Search the inserted log by key indices
            var query = string.Format("SELECT 1 FROM [dbo].[Log] WHERE Logger = '{0}' AND DeviceId = '{1}'", LoggerName, deviceId);
            var result = (int)await DataAccessHelper.ExecuteScalarAsync(query, database: ConnectionStringName);

            // Assert
            Assert.AreEqual(1, result);
        }

        /// <summary>
        /// Delete test logs
        /// </summary>
        /// <returns></returns>
        private static async Task<int> DeleteTestLogs()
        {
            var query = string.Format("DELETE FROM [dbo].[Log] WHERE Logger = '{0}'", LoggerName);
            return await DataAccessHelper.ExecuteAsync(query, database: ConnectionStringName);
        }
    }
}
