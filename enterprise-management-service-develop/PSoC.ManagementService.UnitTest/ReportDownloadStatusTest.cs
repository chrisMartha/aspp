using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PSoC.ManagementService.Controllers;
using PSoC.ManagementService.Services.Interfaces;
using PSoC.ManagementService.Services.Logging;
using PSoC.ManagementService.Services.Models;

namespace PSoC.ManagementService.UnitTest
{
    [TestClass]
    public class ReportDownloadStatusTest
    {
        private Mock<IDeviceService> _mockDeviceService;
        private Mock<ILicenseService> _mockLicenseService;
        private Mock<IUserService> _mockUserService;
        private DevicesController _sut;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockDeviceService = new Mock<IDeviceService>();
            _mockLicenseService = new Mock<ILicenseService>();
            _mockUserService = new Mock<IUserService>();
            _sut = new DevicesController(_mockDeviceService.Object, _mockLicenseService.Object, _mockUserService.Object);
        }

        [TestMethod]
        public async Task TestDownloadStatusReport()
        {
            _mockDeviceService.Setup(x => x.SaveDownloadStatusAsync(It.IsAny<Guid>(), It.IsAny<List<Course>>(), It.IsAny<LogRequest>()));
            var deviceId = Guid.NewGuid();
            var courseId1 = Guid.NewGuid();
            var courseId2 = Guid.NewGuid();
            var courses = new List<Course>()
            {
                new Course
                {
                    LearningResourceId = courseId1,
                    Subject = "ELA",
                    Grade = "2",
                    Percent = 1.5m
                },
                new Course
                {
                    LearningResourceId = courseId2,
                    Subject = "MATH",
                    Grade = "3",
                    Percent = 15m
                }
            };

            await _sut.ReportDownloadStatus(deviceId.ToString(), courses);

            _mockDeviceService.Verify(x => x.SaveDownloadStatusAsync(It.IsAny<Guid>(), It.IsAny<List<Course>>(), It.IsAny<LogRequest>()), Times.Once);          
        }

        [TestMethod]
        public async Task TestDownloadStatusReport_EmptyCourse()
        {
            var deviceId = Guid.NewGuid();
            var courses = new List<Course>();

            await _sut.ReportDownloadStatus(deviceId.ToString(), courses);
            _mockDeviceService.Verify(x => x.SaveDownloadStatusAsync(It.IsAny<Guid>(), It.IsAny<List<Course>>(), It.IsAny<LogRequest>()), Times.Once); 
        }

        [TestMethod]
        public async Task TestDownloadStatusReport_EmptyDeviceId()
        {
            var courses = new List<Course>();

            await _sut.ReportDownloadStatus(string.Empty, courses);
            _mockDeviceService.Verify(x => x.SaveDownloadStatusAsync(It.IsAny<Guid>(), It.IsAny<List<Course>>(), It.IsAny<LogRequest>()), Times.Never); 
        }

        [TestMethod]
        public async Task TestDownloadStatusReport_InvalidDeviceId()
        {
            var courses = new List<Course>();

            await _sut.ReportDownloadStatus("INVALID_ID", courses);
            _mockDeviceService.Verify(x => x.SaveDownloadStatusAsync(It.IsAny<Guid>(), It.IsAny<List<Course>>(), It.IsAny<LogRequest>()), Times.Never); 
        }

        [TestMethod]
        public async Task TestDownloadStatusReport_InvalidCourse()
        {
            var deviceId = Guid.NewGuid();

            await _sut.ReportDownloadStatus(deviceId.ToString(), null);
            _mockDeviceService.Verify(x => x.SaveDownloadStatusAsync(It.IsAny<Guid>(), It.IsAny<List<Course>>(), It.IsAny<LogRequest>()), Times.Never); 
        }

        [TestMethod]
        public async Task TestDownloadStatusReport_FailedService()
        {
            var deviceId = Guid.NewGuid();
            var courseId1 = Guid.NewGuid();
            var courseId2 = Guid.NewGuid();
            var courses = new List<Course>()
            {
                new Course
                {
                    LearningResourceId = courseId1,
                    Subject = "ELA",
                    Grade = "2",
                    Percent = 1.5m
                },
                new Course
                {
                    LearningResourceId = courseId2,
                    Subject = "MATH",
                    Grade = "3",
                    Percent = 15m
                }
            };
            _mockDeviceService.Setup(x => x.SaveDownloadStatusAsync(It.IsAny<Guid>(), It.IsAny<List<Course>>(), It.IsAny<LogRequest>()))
                .Throws(new Exception("TEST"));
            await _sut.ReportDownloadStatus(deviceId.ToString(), courses);
            _mockDeviceService.Verify(x => x.SaveDownloadStatusAsync(It.IsAny<Guid>(), It.IsAny<List<Course>>(), It.IsAny<LogRequest>()), Times.Once); 
        }
    }
}
