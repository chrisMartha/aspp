using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PSoC.ManagementService.Services.Interfaces;
using PSoC.ManagementService.Services.Logging;
using PSoC.ManagementService.Services.Models;

namespace PSoC.ManagementService.Services.UnitTest
{
    [TestClass]
    public class PEMSEventSourceTest
    {
        private IPEMSEventSource eventSource;
        private Mock<ILogger> _mockLogger;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockLogger = new Mock<ILogger>();
            eventSource = PEMSEventSource.Log;

            // Arrange: Inject logger to event source
            eventSource.Logger = _mockLogger.Object; 
        }

        [TestMethod]
        public void PEMSEventSourceTest_WriteLog_VerifyLogRequest()
        {
            // Arrange
            var logRequest = new LogRequest {Logger = "Unit Test", Level = LogLevel.Info};
            _mockLogger.Setup(x => x.IsEnabled(It.IsAny<LogLevel>())).Returns((LogLevel l) => l != LogLevel.Off);
            _mockLogger.Setup(x => x.Log(It.IsAny<LogRequest>()));

            // Act
            eventSource.WriteLog(logRequest);

            // Assert
            _mockLogger.VerifyAll();
        }

        [TestMethod]
        public void PEMSEventSourceTest_PingLog_VerifyLogRequest()
        {
            // Arrange
            var numLevels = Enum.GetNames(typeof(LogLevel)).Length-1; // All available log levels except Off
            _mockLogger.Setup(x => x.IsEnabled(It.IsAny<LogLevel>())).Returns((LogLevel l) => l != LogLevel.Off);
            _mockLogger.Setup(x => x.Log(It.IsAny<LogRequest>()));

            // Act
            eventSource.PingLog();

            // Assert
            _mockLogger.VerifyAll();
            _mockLogger.Verify(x => x.Log(It.IsAny<LogRequest>()), Times.Exactly(numLevels)); 
        }


        [TestMethod]
        public void PEMSEventSourceTest_AppendDeviceLicenseRequest_ExpectNull()
        {
            // Act
            var actual = eventSource.Append(null, null);

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void PEMSEventSourceTest_AppendDeviceLicenseRequest_ExpectNotNull()
        {
            // Arrange
            var deviceId = Guid.NewGuid().ToString();
            var deviceReq = new DeviceLicenseRequest { DeviceId = deviceId };

            // Act
            var actual = eventSource.Append(deviceReq, null);

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(deviceId, actual.DeviceId);
        }

        [TestMethod]
        public void PEMSEventSourceTest_AppendDeviceLicenseRequest_ExpectUpdatedLogRequest()
        {
            // Arrange
            var deviceId = Guid.NewGuid().ToString();
            var deviceReq = new DeviceLicenseRequest { DeviceId = deviceId };
            var logRequest = new LogRequest();

            // Act
            var actual = eventSource.Append(deviceReq, logRequest);

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(deviceId, actual.DeviceId);
        }
    }
}
