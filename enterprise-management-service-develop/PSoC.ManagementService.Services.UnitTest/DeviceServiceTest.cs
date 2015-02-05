using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PSoC.ManagementService.Core;
using PSoC.ManagementService.Data.Interfaces;
using PSoC.ManagementService.Data.Models;
using PSoC.ManagementService.Data.Repositories;
using PSoC.ManagementService.Services.Interfaces;
using PSoC.ManagementService.Services.Models;

namespace PSoC.ManagementService.Services.UnitTest
{
    [TestClass]
    public class DeviceServiceTest
    {
        private Mock<IAzureTableService> _azureTableService;
        private Mock<IConfigBase> _configBase;
        private Mock<IDeviceInstalledCourseRepository> _deviceInstalledCourseRepositoryMock;
        private Mock<IAccessPointDeviceStatusRepository> _accessPointDeviceStatusRepositoryMock;
        private DeviceService _sut; 

        [TestInitialize]
        public void TestInit()
        {
            _azureTableService = new Mock<IAzureTableService>();
            _configBase = new Mock<IConfigBase>();
            _deviceInstalledCourseRepositoryMock = new Mock<IDeviceInstalledCourseRepository>();
            _accessPointDeviceStatusRepositoryMock = new Mock<IAccessPointDeviceStatusRepository>();
            _deviceInstalledCourseRepositoryMock
                .Setup(x => x.ImportDataAsync(It.IsAny<Guid>(), It.IsAny<List<DeviceInstalledCourseDto>>()))
                .ReturnsAsync(true);
            _accessPointDeviceStatusRepositoryMock
                .Setup(x => x.GetByAdminTypeAsync(It.IsAny<AdminType>(), It.IsAny<Guid?>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new Tuple<List<DeviceDto>, int>(new List<DeviceDto>(), 0));
            _sut = new DeviceService(_configBase.Object, _azureTableService.Object, new UnitOfWork(), _deviceInstalledCourseRepositoryMock.Object, _accessPointDeviceStatusRepositoryMock.Object);
        }

        [TestMethod]
        public async Task TestSaveDownloadStatusAsync_valid()
        {  
            // Arrange
            var deviceId = Guid.NewGuid();
            var courseId1 = Guid.NewGuid();
            var courseId2 = Guid.NewGuid();
            var courses = new List<Course>
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
            // Act
            await _sut.SaveDownloadStatusAsync(deviceId, courses);
            // Assert
            _deviceInstalledCourseRepositoryMock.Verify(x => x.ImportDataAsync(It.IsAny<Guid>(), It.IsAny<List<DeviceInstalledCourseDto>>()), Times.Once);
        }

        [TestMethod]
        public async Task TestGetAccessPointDeviceStatusAsync_GlobalAdmin()
        {
            // Arrange
            const AdminType globalAdminType = AdminType.GlobalAdmin;

            // Act
            var list = await _sut.GetAccessPointDeviceStatusAsync(globalAdminType, null, 10, 0);
            // Assert 
            _accessPointDeviceStatusRepositoryMock.Verify(x => x.GetByAdminTypeAsync(globalAdminType, null, 10, 0), Times.Once);
        }

        [TestMethod]
        public async Task TestGetAccessPointDeviceStatusAsync_DistrictAdmin()
        {
            // Arrange
            const AdminType districtAdminType = AdminType.DistrictAdmin;

            // Act
            var list = await _sut.GetAccessPointDeviceStatusAsync(districtAdminType, Guid.NewGuid(), 10, 0);

            // Assert 
            _accessPointDeviceStatusRepositoryMock.Verify(x => x.GetByAdminTypeAsync(districtAdminType, It.IsAny<Guid>(), 10, 0), Times.Once);
        }

        [TestMethod]
        public async Task TestGetAccessPointDeviceStatusAsync_SchoolAdmin()
        {
            // Arrange
            const AdminType schoolAdminType = AdminType.SchoolAdmin;

            // Act
            var list = await _sut.GetAccessPointDeviceStatusAsync(schoolAdminType, Guid.NewGuid(), 10, 0);

            // Assert 
            _accessPointDeviceStatusRepositoryMock.Verify(x => x.GetByAdminTypeAsync(schoolAdminType, It.IsAny<Guid>(), 10, 0), Times.Once);
        }      
    }
}
