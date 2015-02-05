using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PSoC.ManagementService.Data.Interfaces;
using PSoC.ManagementService.Data.Models;
using PSoC.ManagementService.Services.Logging;
using PSoC.ManagementService.Services.Models;

namespace PSoC.ManagementService.Services.UnitTest
{
    [TestClass]
    public class LicenseServiceTest
    {
        private LicenseService _sut;
        private Mock<ILicenseRepository> _mockLicenseRepository;
        private Mock<IDeviceInstalledCourseRepository> _mockDeviceInstalledCourseRepository;
        private MockRepository _mockFactory;
        private readonly Guid _deviceId = new Guid("4419C16C-11A2-48E6-A5A3-C6F671943A52");
        private readonly Guid _licenseRequestId = new Guid("22222222-1111-AAAA-BBBB-C6F671943A52");
        private const string ConfigCode = "prsnassess";
        private const string WifiBssId = "ee:ff:e9:81:0b:00";
        private readonly Guid _userId = new Guid("777CCC4E-EDB6-43C9-9DBE-E46CDB845FAE");
        private LicenseDto _licenseDto;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockFactory = new MockRepository(MockBehavior.Loose);
            _mockLicenseRepository = _mockFactory.Create<ILicenseRepository>();
            _mockDeviceInstalledCourseRepository = _mockFactory.Create<IDeviceInstalledCourseRepository>();
            _sut = new LicenseService(_mockLicenseRepository.Object, _mockDeviceInstalledCourseRepository.Object);
            _licenseDto = new LicenseDto()
            {
                ConfigCode = ConfigCode,
                WifiBSSID = WifiBssId,
                LicenseRequest = new LicenseRequestDto()
                {
                    LicenseRequestID = _licenseRequestId,
                    Device = new DeviceDto() { DeviceID = _deviceId },
                    Created = DateTime.Now,
                    RequestDateTime = DateTime.Now,
                    ResponseDateTime = DateTime.Now,
                    User = new UserDto() { UserID = _userId }
                }
            };
        }

        [TestMethod]
        public async Task LicenseService_NoLicensesToDelete()
        {
            //Arrange
            _mockLicenseRepository.Setup(x => x.GetExpiredLicensesAsync())
                .ReturnsAsync(new List<LicenseDto>().ToArray());
            _mockLicenseRepository.Setup(x => x.DeleteAsync(It.Is<Guid[]>(list => list.Length == 0)))
                .Throws(new Exception("Deleting licenses when no licenses supplied!"));
            //Act
            await _sut.DeleteExpiredLicensesAsync();

            //Verify
            _mockFactory.Verify();
        }

        [TestMethod]
        public async Task LicenseService_OneLicenseToDelete()
        {
            //Arrange
            LicenseDto license = new LicenseDto
            {
                LicenseRequest = new LicenseRequestDto
                {
                    LicenseRequestID = new Guid("11111111-1111-1111-1111-111111111111")
                }
            };
            List<LicenseDto> licenseList = new List<LicenseDto> {license};
            _mockLicenseRepository.Setup(x => x.GetExpiredLicensesAsync()).ReturnsAsync(licenseList.ToArray());
            _mockLicenseRepository.Setup(x => x.DeleteAsync(It.Is<Guid[]>(
                                                                        list => 
                                                                        list.Length == 1 &&
                                                                        (list[0]).Equals(license.LicenseRequest.LicenseRequestID))))
                                    .Returns(Task.FromResult(default(bool)))
                                    .Verifiable("No license to delete when one was given!");
            //Act
            await _sut.DeleteExpiredLicensesAsync();

            //Verify
            _mockFactory.Verify();
        }

        [TestMethod]
        public async Task LicenseService_GetLicenseForDeviceAsync_EmptyDeviceId_ThrowsArgumentException()
        {
            // Arrange
            Exception expectedException = null;
            Guid deviceId = Guid.Empty;
            License result = null;

            //Act
            try
            {
                result = await _sut.GetLicenseForDeviceAsync(deviceId, It.IsAny<LogRequest>());
            }
            catch (Exception ex)
            {
                expectedException = ex;
            }

            //Assert
            Assert.IsNull(result);
            Assert.IsInstanceOfType(expectedException, typeof (ArgumentException));
        }

        [TestMethod]
        public async Task LicenseService_GetLicenseForDeviceAsync_ExistingLicense_Success()
        {
            // Arrange
            var license = _licenseDto;
            _mockLicenseRepository.Setup(x => x.GetLicenseForDeviceAsync(_deviceId)).ReturnsAsync(license);

            // Act
            var result = await _sut.GetLicenseForDeviceAsync(_deviceId, It.IsAny<LogRequest>());

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(license.LicenseRequest.Device.DeviceID, result.LicenseRequest.DeviceId);
        }

        [TestMethod]
        public async Task LicenseService_GetLicenseForDeviceAsync_NonExistingLicense_Failure()
        {
            // Arrange
            _mockLicenseRepository.Setup(x => x.GetLicenseForDeviceAsync(_deviceId)).ReturnsAsync(null);

            // Act
            var result = await _sut.GetLicenseForDeviceAsync(_deviceId, It.IsAny<LogRequest>());

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task LicenseService_RevokeLicenseForDeviceAsync_EmptyLicenseRequestId_ThrowsArgumentException()
        {
            // Arrange
            Exception expectedException = null;
           
            //Act
            try
            {
                await _sut.RevokeLicenseForDeviceAsync(Guid.Empty, _userId, DateTime.Now, It.IsAny<LogRequest>(), true);
            }
            catch (Exception ex)
            {
                expectedException = ex;
            }

            //Assert
            Assert.IsInstanceOfType(expectedException, typeof(ArgumentException));
        }

        [TestMethod]
        public async Task LicenseService_RevokeLicenseForDeviceAsync_EmptyUserId_ThrowsArgumentException()
        {
            // Arrange
            Exception expectedException = null;

            //Act
            try
            {
                await _sut.RevokeLicenseForDeviceAsync(_licenseRequestId, Guid.Empty, DateTime.Now, It.IsAny<LogRequest>(), true);
            }
            catch (Exception ex)
            {
                expectedException = ex;
            }

            //Assert
            Assert.IsInstanceOfType(expectedException, typeof(ArgumentException));
        }

        [TestMethod]
        public async Task LicenseService_RevokeLicenseForDeviceAsync_UnableToRevoke_ThrowsException()
        {
            // Arrange
            var requestDateTime = DateTime.Now;
            const bool isAdmin = true;
            Exception expectedException = null;
            _mockLicenseRepository.Setup(
                x => x.RevokeLicenseForDeviceAsync(_licenseRequestId, _userId, requestDateTime, isAdmin))
                .ReturnsAsync(false);
            var errorMessage = String.Format("Device License revocation error for license request id {0}: No New Record add to deviceInstalledCourse table.", _licenseRequestId);

            //Act
            try
            {
                await _sut.RevokeLicenseForDeviceAsync(_licenseRequestId, _userId, requestDateTime, It.IsAny<LogRequest>(), isAdmin);
            }
            catch (Exception ex)
            {
                expectedException = ex;
            }

            //Assert
            Assert.IsNotNull(expectedException);
            Assert.AreEqual(expectedException.Message, errorMessage);
        }

        [TestMethod]
        public async Task LicenseService_RevokeLicenseForDeviceAsync_AbleToRevoke_Success()
        {
            // Arrange
            var requestDateTime = DateTime.Now;
            Exception expectedException = null;
            const bool isAdmin = true;
            _mockLicenseRepository.Setup(
                x => x.RevokeLicenseForDeviceAsync(_licenseRequestId, _userId, requestDateTime, isAdmin))
                .ReturnsAsync(true);
          
            //Act
            try
            {
                await _sut.RevokeLicenseForDeviceAsync(_licenseRequestId, _userId, requestDateTime, It.IsAny<LogRequest>(), isAdmin);
            }
            catch (Exception ex)
            {
                expectedException = ex;
            }

            //Assert
            Assert.IsNull(expectedException);
            _mockFactory.Verify();
        }

        [TestMethod]
        public async Task LicenseService_RequestLicenseForDeviceAsync_NullLicenseRequest_ThrowsArgumentException()
        {
            // Arrange
            Exception expectedException = null;
            bool result = false;
           
            //Act
            try
            {
                result = await _sut.RequestLicenseForDeviceAsync(null, It.IsAny<LogRequest>());
            }
            catch (Exception ex)
            {
                expectedException = ex;
            }

            //Assert
            Assert.IsFalse(result);
            Assert.IsInstanceOfType(expectedException, typeof(ArgumentException));
        }

        [TestMethod]
        public async Task LicenseService_RequestLicenseForDeviceAsync_LicenseRequestWithEmptyDeviceId_ThrowsArgumentException()
        {
            // Arrange
            Exception expectedException = null;
            DeviceLicenseRequest licenseRequest = new DeviceLicenseRequest()
            {
                WifiBSSID = WifiBssId,
                UserId = _userId.ToString(),
                DeviceId = string.Empty
            };
            bool result = false;

            //Act
            try
            {
                result = await _sut.RequestLicenseForDeviceAsync(licenseRequest, It.IsAny<LogRequest>());
            }
            catch (Exception ex)
            {
                expectedException = ex;
            }

            //Assert
            Assert.IsFalse(result);
            Assert.IsInstanceOfType(expectedException, typeof(ArgumentException));
            Assert.IsTrue(expectedException.Message.Contains("Value for License Request is invalid or missing params"));
        }

        [TestMethod]
        public async Task LicenseService_RequestLicenseForDeviceAsync_LicenseRequestWithEmptyUserId_ThrowsArgumentException()
        {
            // Arrange
            Exception expectedException = null;
            DeviceLicenseRequest licenseRequest = new DeviceLicenseRequest()
            {
                WifiBSSID = WifiBssId,
                UserId = string.Empty,
                DeviceId = _deviceId.ToString()
            };
            bool result = false;

            //Act
            try
            {
                result = await _sut.RequestLicenseForDeviceAsync(licenseRequest, It.IsAny<LogRequest>());
            }
            catch (Exception ex)
            {
                expectedException = ex;
            }

            //Assert
            Assert.IsFalse(result);
            Assert.IsInstanceOfType(expectedException, typeof(ArgumentException));
            Assert.IsTrue(expectedException.Message.Contains("Value for License Request is invalid or missing params"));
        }

        [TestMethod]
        public async Task LicenseService_RequestLicenseForDeviceAsync_LicenseRequestWithEmptyWifiBSSId_ThrowsArgumentException()
        {
            // Arrange
            Exception expectedException = null;
            DeviceLicenseRequest licenseRequest = new DeviceLicenseRequest()
            {
                WifiBSSID = null,
                UserId = _userId.ToString(),
                DeviceId = _deviceId.ToString()
            };
            bool result = false;

            //Act
            try
            {
                result = await _sut.RequestLicenseForDeviceAsync(licenseRequest, It.IsAny<LogRequest>());
            }
            catch (Exception ex)
            {
                expectedException = ex;
            }

            //Assert
            Assert.IsFalse(result);
            Assert.IsInstanceOfType(expectedException, typeof(ArgumentException));
            Assert.IsTrue(expectedException.Message.Contains("Value for License Request is invalid or missing params"));
        }

        [TestMethod]
        public async Task LicenseService_RequestLicenseForDeviceAsync_Operation_Failure()
        {
            // Arrange
            Exception expectedException = null;
            bool result = false;
            DeviceLicenseRequest licenseRequest = new DeviceLicenseRequest()
            {
                WifiBSSID = WifiBssId,
                UserId = _userId.ToString(),
                DeviceId = _deviceId.ToString()
            };
            _mockLicenseRepository.Setup(x => x.GrantLicenseForDeviceAsync((LicenseRequestDto) licenseRequest)).ReturnsAsync(false);

            //Act
            try
            {
                result = await _sut.RequestLicenseForDeviceAsync(licenseRequest, It.IsAny<LogRequest>());
            }
            catch (Exception ex)
            {
                expectedException = ex;
            }

            // Assert
            Assert.IsNull(expectedException);
            Assert.IsNotNull(result);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task LicenseService_RequestLicenseForDeviceAsync_Operation_Success()
        {
            // Arrange
            Exception expectedException = null;
            bool result = false;
            DeviceLicenseRequest licenseRequest = new DeviceLicenseRequest()
            {
                WifiBSSID = WifiBssId,
                UserId = _userId.ToString(),
                DeviceId = _deviceId.ToString()
            };
            _mockLicenseRepository.Setup(x => x.GrantLicenseForDeviceAsync((LicenseRequestDto)licenseRequest)).ReturnsAsync(true);

            //Act
            try
            {
                result = await _sut.RequestLicenseForDeviceAsync(licenseRequest, It.IsAny<LogRequest>());
            }
            catch (Exception ex)
            {
                expectedException = ex;
            }

            // Assert
            Assert.IsNull(expectedException);
            Assert.IsNotNull(result);
        }
    }
}