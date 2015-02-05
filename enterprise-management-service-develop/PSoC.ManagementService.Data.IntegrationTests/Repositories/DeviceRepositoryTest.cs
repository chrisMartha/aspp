using System;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using PSoC.ManagementService.Data.Models;
using PSoC.ManagementService.Data.Repositories;

namespace PSoC.ManagementService.Data.IntegrationTests.Repositories
{
    [TestClass]
    public class DeviceRepositoryTest
    {
        private readonly DeviceDto _device = new DeviceDto { DeviceID = Guid.NewGuid(), DeviceName = Guid.NewGuid().ToString() };
        private readonly Guid _invalidDeviceId = Guid.Parse("111A1BC7-4B2B-4183-A624-A8C68C365E30");

        private DeviceRepository _sut;
        private DeviceDto _testDevice;

        #region DeleteAsync unit tests
        [TestMethod]
        public async Task DeviceRepository_DeleteAsync_ValidId_Success()
        {
            // Arrange
            Guid testDeviceId = Guid.NewGuid();
            DeviceDto testDevice = new DeviceDto { DeviceID = testDeviceId };
            await _sut.InsertAsync(testDevice).ConfigureAwait(false);

            // Act
            bool result = await _sut.DeleteAsync(testDeviceId).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(result);
            DeviceDto deleted = await _sut.GetByIdAsync(testDeviceId);
            Assert.IsNull(deleted);
        }

        [TestMethod]
        public async Task DeviceRepository_DeleteAsync_ValidIds_Success()
        {
            // Arrange
            Guid testDeviceId1 = Guid.NewGuid();
            DeviceDto testDevice1 = new DeviceDto { DeviceID = testDeviceId1 };
            await _sut.InsertAsync(testDevice1).ConfigureAwait(false);
            Guid testDeviceId2 = Guid.NewGuid();
            DeviceDto testDevice2 = new DeviceDto { DeviceID = testDeviceId2 };
            await _sut.InsertAsync(testDevice2).ConfigureAwait(false);
            Guid[] testDeviceIds = { testDeviceId1, testDeviceId2 };

            // Act
            bool result = await _sut.DeleteAsync(testDeviceIds).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(result);
            DeviceDto deleted = await _sut.GetByIdAsync(testDeviceId1);
            Assert.IsNull(deleted);
            DeviceDto deleted2 = await _sut.GetByIdAsync(testDeviceId2);
            Assert.IsNull(deleted2);
        }

        [TestMethod]
        public async Task DeviceRepository_DeleteAsync_InvalidId_Failure()
        {
            // Arrange
            Guid testDeviceId = Guid.NewGuid();

            // Act
            bool result = await _sut.DeleteAsync(testDeviceId).ConfigureAwait(false);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task DeviceRepository_DeleteAsync_InvalidIds_Failure()
        {
            // Arrange
            Guid testDeviceId1 = Guid.NewGuid();
            Guid testDeviceId2 = Guid.NewGuid();
            Guid[] testDeviceIds = { testDeviceId1, testDeviceId2 };

            // Act
            bool result = await _sut.DeleteAsync(testDeviceIds).ConfigureAwait(false);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task DeviceRepository_DeleteAsync_MixedIds_Success()
        {
            // Arrange
            Guid testDeviceId1 = Guid.NewGuid();
            DeviceDto testDevice1 = new DeviceDto { DeviceID = testDeviceId1 };
            await _sut.InsertAsync(testDevice1).ConfigureAwait(false);
            Guid testDeviceId2 = Guid.NewGuid();
            Guid[] testDeviceIds = { testDeviceId1, testDeviceId2 };

            // Act
            bool result = await _sut.DeleteAsync(testDeviceIds).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(result);
            DeviceDto deleted = await _sut.GetByIdAsync(testDeviceId1);
            Assert.IsNull(deleted);
        }
        #endregion

        [TestMethod]
        public async Task DeviceRepository_GetByIdAsync_InValidId_Failure()
        {
            //Act
            var result = await _sut.GetByIdAsync(_invalidDeviceId).ConfigureAwait(false);

            //Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task DeviceRepository_GetByIdAsync_ValidId_Success()
        {
            //Act
            var result = await _sut.GetByIdAsync(_testDevice.DeviceID).ConfigureAwait(false);

            //Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task DeviceRepository_InsertAsync()
        {
            InitIntegrationTest.TestDeviceIds.Add(_device.DeviceID);

            var result = await _sut.InsertAsync(_device).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.AreEqual(result.DeviceID, _device.DeviceID);
            Assert.IsTrue(result.DeviceName == _device.DeviceName);
        }

        [TestInitialize]
        public void Initialize()
        {
            // Arrange
            _sut = new DeviceRepository();
            _testDevice = new DeviceDto() { DeviceID = Guid.NewGuid() };
            InitIntegrationTest.TestDistrictIds.Add(_testDevice.DeviceID);
            _sut.InsertAsync(_testDevice).Wait();
        }
    }
}