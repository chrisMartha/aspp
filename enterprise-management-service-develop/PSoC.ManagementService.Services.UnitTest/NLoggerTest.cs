using System;
using System.Diagnostics;
using System.Net.Http;
using System.Web.Http.Tracing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PSoC.ManagementService.Services.Interfaces;
using PSoC.ManagementService.Services.Logging;
using TraceLevel = System.Web.Http.Tracing.TraceLevel;

namespace PSoC.ManagementService.Services.UnitTest
{
    [TestClass]
    public class NLoggerTest
    {
        private Mock<ILogger> _mockLogger;
        private Mock<ITraceWriter> _mockTraceWriter;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockLogger = new Mock<ILogger>();
            _mockTraceWriter = _mockLogger.As<ITraceWriter>();

            // Arrange: All log levels except off are active
            _mockLogger.Setup(x => x.IsEnabled(It.IsAny<LogLevel>())).Returns((LogLevel l) => l != LogLevel.Off);
        }

        [TestMethod]
        public void NLoggerTest_IsEnabled_DisabledLogLevelReturnsFalse()
        {
            // Act
            var result = _mockLogger.Object.IsEnabled(LogLevel.Off);

           // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void NLoggerTest_IsEnabled_EnabledLogLevelReturnsTrue()
        {
            // Act
            var result = _mockLogger.Object.IsEnabled(LogLevel.Info);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void NLoggerTest_Log_VerifyLogRequest()
        {
            // Arrange
            var logRequest = new LogRequest();

            // Act
            _mockLogger.Object.Log(logRequest);

            // Assert
            _mockLogger.Verify(m => m.Log(It.IsAny<LogRequest>()));
        }

        [TestMethod]
        public void NLoggerTest_Trace_VerifyRequest()
        {
            // Arrange
            var request = new HttpRequestMessage();
            const string category = "Test";
            const TraceLevel level = TraceLevel.Info;
            Action<TraceRecord> traceAction = (TraceRecord r) => Debug.WriteLine("Trace record");

            // Act
            _mockTraceWriter.Object.Trace(request, category, level, traceAction);

            // Assert
            _mockTraceWriter.Verify(m => m.Trace(It.IsAny<HttpRequestMessage>(), It.IsAny<string>(), It.IsAny<TraceLevel>(), It.IsAny<Action<TraceRecord>>()));
        }
    }
}
