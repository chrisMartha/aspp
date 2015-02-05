//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Moq;

//using PSoC.ManagementService.Data.Interfaces;
//using PSoC.ManagementService.Data.Models;
//using PSoC.ManagementService.Services;

//namespace PSoC.ManagementService.Services.UnitTest
//{
//    [TestClass]
//    public class DataServiceTest
//    {
//        private Mock<IDeviceRepository> _deviceRepositoryMock;
//        private Mock<IUnitOfWork> _unitOfWorkMock;
//        private DataService<DeviceDto, Guid> _sut;
//        private Guid testId = Guid.Parse("E6D60709-C9B3-4083-9D30-1A6F63A5B77C");

//        [TestInitialize]
//        public void Initialize()
//        {
//            _deviceRepositoryMock = new Mock<IDeviceRepository>();
//            _unitOfWorkMock = new Mock<IUnitOfWork>();

//            // Pre-arrange: unit of work returns device repository
//            _unitOfWorkMock.Setup(x => x.GetDataRepository<DeviceDto, Guid>()).Returns(_deviceRepositoryMock.Object);

//            // Subject under test
//            _sut = new DeviceService(_unitOfWorkMock.Object);
//        }

//        #region GetAsync unit tests
//        [TestMethod]
//        public async Task DataService_GetAsync_ReturnsNone()
//        {
//            // Arrange: empty device list
//            var devices = new List<DeviceDto>();

//            // Arrange: device repository returns empty device data list
//            _deviceRepositoryMock.Setup(x => x.GetAsync()).ReturnsAsync(devices);

//            // Act
//            DeviceDto[] result = await _sut.GetAsync();

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(result.Length, 0);
//        }

//        [TestMethod]
//        public async Task DataService_GetAsync_ReturnsOne()
//        {
//            // Arrange: device
//            var device = new DeviceDto();

//            // Arrange: single device list
//            var devices = new List<DeviceDto>
//            {
//                device
//            };

//            // Arrange: device repository returns single device data list
//            _deviceRepositoryMock.Setup(x => x.GetAsync()).ReturnsAsync(devices);

//            // Act
//            DeviceDto[] result = await _sut.GetAsync();

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(result.Length, 1);
//            Assert.AreEqual(result[0], device);
//        }

//        [TestMethod]
//        public async Task DataService_GetAsync_ReturnsMany()
//        {
//            // Arrange: device
//            var device1 = new DeviceDto();
//            var device2 = new DeviceDto();

//            // Arrange: single device list
//            var devices = new List<DeviceDto>
//            {
//                device1,
//                device2
//            };

//            // Arrange: device repository returns double device list 
//            _deviceRepositoryMock.Setup(x => x.GetAsync()).ReturnsAsync(devices);

//            // Act
//            DeviceDto[] result = await _sut.GetAsync();

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(result.Length, 2);
//            Assert.AreEqual(result[0], device1);
//            Assert.AreEqual(result[1], device2);
//        }
//        #endregion

//        #region GetByIdAsync unit tests
//        [TestMethod]
//        public async Task DataService_GetByIdAsync_NonExistentId_Failure()
//        {
//            // Arrange
//            _deviceRepositoryMock.Setup(x => x.GetByIdAsync(testId)).ReturnsAsync(null);

//            // Act
//            DeviceDto result = await _sut.GetByIdAsync(testId);

//            // Assert
//            Assert.IsNull(result);
//        }

//        [TestMethod]
//        public async Task DataService_GetByIdAsync_ExistingId_Success()
//        {
//            // Arrange
//            var device = new DeviceDto
//            {
//                Id = testId
//            };
//            _deviceRepositoryMock.Setup(x => x.GetByIdAsync(testId)).ReturnsAsync(device);

//            // Act
//            DeviceDto result = await _sut.GetByIdAsync(testId);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(result, device);
//        }
//        #endregion

//        #region CreateAsync unit tests
//        [TestMethod]
//        public async Task DataService_CreateAsync_NullEntity_Failure()
//        {
//            // Arrange: null device
//            DeviceDto device = null;

//            // Arrange: device repository returns null given the invalid device
//            _deviceRepositoryMock.Setup(x => x.InsertAsync(device)).ReturnsAsync(null);

//            // Act
//            DeviceDto result = await _sut.CreateAsync(null);

//            // Assert
//            Assert.IsNull(result);
//        }

//        [TestMethod]
//        public async Task DataService_CreateAsync_ValidEntity_Success()
//        {
//            // Arrange: device
//            var device = new DeviceDto();

//            // Arrange: device repository returns device given the valid device
//            _deviceRepositoryMock.Setup(x => x.InsertAsync(device)).ReturnsAsync(device);

//            // Act
//            DeviceDto result = await _sut.CreateAsync(device);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(result, device);
//        }
//        #endregion

//        #region UpdateAsync unit tests
//        [TestMethod]
//        public async Task DataService_UpdateAsync_NullEntity_Failure()
//        {
//            // Arrange: null device
//            DeviceDto device = null;

//            // Arrange: device repository returns null given the invalid device
//            _deviceRepositoryMock.Setup(x => x.UpdateAsync(device)).ReturnsAsync(null);

//            // Act
//            DeviceDto result = await _sut.UpdateAsync(null);

//            // Assert
//            Assert.IsNull(result);
//        }

//        [TestMethod]
//        public async Task DataService_UpdateAsync_ValidEntity_Success()
//        {
//            // Arrange: device
//            var device = new DeviceDto();

//            // Arrange: device repository returns device given the valid device
//            _deviceRepositoryMock.Setup(x => x.UpdateAsync(device)).ReturnsAsync(device);

//            // Act
//            DeviceDto result = await _sut.UpdateAsync(device);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(result, device);
//        }
//        #endregion

//        #region DeleteAsync unit tests
//        [TestMethod]
//        public async Task DataService_DeleteAsync_NonExistingKey_Failure()
//        {
//            // Arrange
//            const bool success = false;

//            // Arrange: device repository returns false given the invalid device
//            _deviceRepositoryMock.Setup(x => x.DeleteAsync(testId)).ReturnsAsync(success);

//            // Act
//            bool result = await _sut.DeleteAsync(testId);

//            // Assert
//            Assert.AreEqual(result, success);
//        }

//        [TestMethod]
//        public async Task DataService_DeleteAsync_ExistingKey_Success()
//        {
//            // Arrange
//            const bool success = true;

//            // Arrange: device repository returns true given the valid device
//            _deviceRepositoryMock.Setup(x => x.DeleteAsync(testId)).ReturnsAsync(success);

//            // Act
//            bool result = await _sut.DeleteAsync(testId);

//            // Assert
//            Assert.AreEqual(result, success);
//        }
//        #endregion
//    }
//}
