using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PSoC.ManagementService.Data.Interfaces;
using PSoC.ManagementService.Data.Models;
using PSoC.ManagementService.Services.Models;

namespace PSoC.ManagementService.Services.UnitTest
{
    [TestClass]
    public class DistrictServiceTest
    {
        private Mock<IAdminRepository> _adminRepositoryMock;
        private Mock<IDistrictRepository> _districtRepositoryMock;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private DistrictService _sut;
        private readonly String testUsername = "test";
        private readonly Guid testId = Guid.Parse("E6D60709-C9B3-4083-9D30-1A6F63A5B77C");
        private readonly Guid testId2 = Guid.Parse("F7D60709-C9B3-4083-9D30-1A6F63A5B77C");

        [TestInitialize]
        public void Initialize()
        {
            _adminRepositoryMock = new Mock<IAdminRepository>();
            _districtRepositoryMock = new Mock<IDistrictRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            // Pre-arrange
            _unitOfWorkMock.Setup(x => x.GetDataRepository<AdminDto, Guid>()).Returns(_adminRepositoryMock.Object);
            _unitOfWorkMock.Setup(x => x.GetDataRepository<DistrictDto, Guid>()).Returns(_districtRepositoryMock.Object);
            _unitOfWorkMock.SetupGet(x => x.AdminRepository).Returns(_adminRepositoryMock.Object);
            _unitOfWorkMock.SetupGet(x => x.DistrictRepository).Returns(_districtRepositoryMock.Object);

            // Subject under test
            _sut = new DistrictService(_unitOfWorkMock.Object);
        }

        [TestMethod]
        public void DistrictService_CastDistrictDtoToDistrict()
        {
            // Arrange
            var districtDto = new DistrictDto { DistrictId = testId, OAuthApplicationId = testId.ToString(), OAuthClientId = testId.ToString() };

            // Act
            var district = (District) districtDto;

            // Assert
            Assert.AreEqual(districtDto.DistrictId, district.DistrictId);
            Assert.AreEqual(districtDto.OAuthApplicationId, district.OAuthApplicationId.ToString());
            Assert.AreEqual(districtDto.OAuthClientId, district.OAuthClientId.ToString());
        }

        [TestMethod]
        public void DistrictService_CastDistrictToDistrictDto()
        {
            // Arrange
            var district = new District { DistrictId = testId, OAuthApplicationId = testId.ToString(), OAuthClientId = testId.ToString() };

            // Act
            var districtDto = (DistrictDto)district;

            // Assert
            Assert.AreEqual(district.DistrictId, districtDto.DistrictId);
            Assert.AreEqual(district.OAuthApplicationId.ToString(), districtDto.OAuthApplicationId);
            Assert.AreEqual(district.OAuthClientId.ToString(), districtDto.OAuthClientId);
        }

        [TestMethod]
        public async Task DistrictService_GetAsync_ReturnsNone()
        {
            // Arrange
            var districts = new List<DistrictDto>();
            _districtRepositoryMock.Setup(x => x.GetAsync()).ReturnsAsync(districts);

            // Act
            var result = await _sut.GetAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public async Task DistrictService_GetAsync_ReturnsMany()
        {
            // Arrange
            var district1 = new DistrictDto { DistrictId = testId, OAuthApplicationId = testId.ToString(), OAuthClientId = testId.ToString() };
            var district2 = new DistrictDto { DistrictId = testId2, OAuthApplicationId = testId2.ToString(), OAuthClientId = testId2.ToString() }; 
            var districts = new List<DistrictDto>
            {
                district1,
                district2
            };
            _districtRepositoryMock.Setup(x => x.GetAsync()).ReturnsAsync(districts);

            // Act
            var result = await _sut.GetAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.Any(r=> r.DistrictId ==  district1.DistrictId));
            Assert.IsTrue(result.Any(r => r.DistrictId == district2.DistrictId));
        }

        [TestMethod]
        public async Task DistrictService_GetAsync_GlobalAdmin_ReturnsMany()
        {
            // Arrange
            var globalAdmin = new AdminDto();
            var district1 = new DistrictDto { DistrictId = testId, OAuthApplicationId = testId.ToString(), OAuthClientId = testId.ToString() };
            var district2 = new DistrictDto { DistrictId = testId2, OAuthApplicationId = testId2.ToString(), OAuthClientId = testId2.ToString() };
            var districts = new List<DistrictDto>
            {
                district1,
                district2
            };
            _adminRepositoryMock.Setup(x => x.GetByUsernameAsync(It.IsAny<String>())).ReturnsAsync(globalAdmin);
            _districtRepositoryMock.Setup(x => x.GetAsync()).ReturnsAsync(districts);
            _districtRepositoryMock.Setup(x => x.GetByIdAsync(testId)).ReturnsAsync(district1);

            // Act
            var result = await _sut.GetAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.Any(r => r.DistrictId == district1.DistrictId));
            Assert.IsTrue(result.Any(r => r.DistrictId == district2.DistrictId));
        }

        [TestMethod]
        public async Task DistrictService_GetAsync_DistrictAdmin_ReturnsOne()
        {
            // Arrange
            var districtAdmin = new AdminDto { District = new DistrictDto{DistrictId = testId} };
            var district1 = new DistrictDto { DistrictId = testId, OAuthApplicationId = testId.ToString(), OAuthClientId = testId.ToString() };
            var district2 = new DistrictDto { DistrictId = testId2, OAuthApplicationId = testId2.ToString(), OAuthClientId = testId2.ToString() };
            var districts = new List<DistrictDto>
            {
                district1,
                district2
            };
            _adminRepositoryMock.Setup(x => x.GetByUsernameAsync(It.IsAny<String>())).ReturnsAsync(districtAdmin);
            _districtRepositoryMock.Setup(x => x.GetAsync()).ReturnsAsync(districts);
            _districtRepositoryMock.Setup(x => x.GetByIdAsync(testId)).ReturnsAsync(district1);

            // Act
            var result = await _sut.GetAsync(testUsername);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.IsTrue(result.Any(r => r.DistrictId == district1.DistrictId));
        }

        [TestMethod]
        public async Task DistrictService_GetAsync_SchoolAdmin_ThrowException()
        {
            // Arrange
            Exception expectedException = null;
            var schoolAdmin = new AdminDto { School = new SchoolDto() };
            var district1 = new DistrictDto { DistrictId = testId, OAuthApplicationId = testId.ToString(), OAuthClientId = testId.ToString() };
            var district2 = new DistrictDto { DistrictId = testId2, OAuthApplicationId = testId2.ToString(), OAuthClientId = testId2.ToString() };
            var districts = new List<DistrictDto>
            {
                district1,
                district2
            };
            _adminRepositoryMock.Setup(x => x.GetByUsernameAsync(It.IsAny<String>())).ReturnsAsync(schoolAdmin);
            _districtRepositoryMock.Setup(x => x.GetAsync()).ReturnsAsync(districts);
            _districtRepositoryMock.Setup(x => x.GetByIdAsync(testId)).ReturnsAsync(district1);

            // Act
            try
            {
                var result = await _sut.GetAsync(testUsername);
            }
            catch (Exception ex)
            {
                expectedException = ex;
            }

            // Assert
            Assert.IsNotNull(expectedException);
        }

        [TestMethod]
        public async Task DistrictService_GetAsync_NonAdmin_ThrowException()
        {
            // Arrange
            Exception expectedException = null;
            var district1 = new DistrictDto { DistrictId = testId, OAuthApplicationId = testId.ToString(), OAuthClientId = testId.ToString() };
            var district2 = new DistrictDto { DistrictId = testId2, OAuthApplicationId = testId2.ToString(), OAuthClientId = testId2.ToString() };
            var districts = new List<DistrictDto>
            {
                district1,
                district2
            };
            _adminRepositoryMock.Setup(x => x.GetByUsernameAsync(It.IsAny<String>())).ReturnsAsync(null);
            _districtRepositoryMock.Setup(x => x.GetAsync()).ReturnsAsync(districts);
            _districtRepositoryMock.Setup(x => x.GetByIdAsync(testId)).ReturnsAsync(district1);

            // Act
            try
            {
                await _sut.GetAsync(testUsername);
            }
            catch (Exception ex)
            {
                expectedException = ex;
            }

            // Assert
            Assert.IsNotNull(expectedException);
        }

        [TestMethod]
        public async Task DistrictService_GetByIdAsync_NonExistentId_Failure()
        {
            // Arrange
            _districtRepositoryMock.Setup(x => x.GetByIdAsync(testId)).ReturnsAsync(null);

            // Act
            var result = await _sut.GetByIdAsync(testId);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task DistrictService_GetByIdAsync_ExistingId_Success()
        {
            // Arrange
            var district = new DistrictDto { DistrictId = testId, OAuthApplicationId = testId.ToString(), OAuthClientId = testId.ToString() };
            _districtRepositoryMock.Setup(x => x.GetByIdAsync(testId)).ReturnsAsync(district);

            // Act
            var result = await _sut.GetByIdAsync(testId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(district.DistrictId, result.DistrictId);
        }

        [TestMethod]
        public async Task DistrictService_GetByIdAsync_GlobalAdmin_ExistingId_Success()
        {
            // Arrange
            var globalAdmin = new AdminDto();
            var district = new DistrictDto { DistrictId = testId, OAuthApplicationId = testId.ToString(), OAuthClientId = testId.ToString() };
            _adminRepositoryMock.Setup(x => x.GetByUsernameAsync(It.IsAny<String>())).ReturnsAsync(globalAdmin);
            _districtRepositoryMock.Setup(x => x.GetByIdAsync(testId)).ReturnsAsync(district);

            // Act
            var result = await _sut.GetByIdAsync(testUsername, testId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(district.DistrictId, result.DistrictId);
        }

        [TestMethod]
        public async Task DistrictService_GetByIdAsync_DistrictAdmin_ExistingId_Success()
        {
            // Arrange
            var districtAdmin = new AdminDto { District = new DistrictDto { DistrictId = testId } };
            var district = new DistrictDto { DistrictId = testId, OAuthApplicationId = testId.ToString(), OAuthClientId = testId.ToString() };
            _adminRepositoryMock.Setup(x => x.GetByUsernameAsync(It.IsAny<String>())).ReturnsAsync(districtAdmin);
            _districtRepositoryMock.Setup(x => x.GetByIdAsync(testId)).ReturnsAsync(district);

            // Act
            var result = await _sut.GetByIdAsync(testUsername, testId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(district.DistrictId, result.DistrictId);
        }

        [TestMethod]
        public async Task DistrictService_GetByIdAsync_InvalidDistrictAdmin_ExistingId_ThrowException()
        {
            // Arrange
            Exception expectedException = null;
            var districtAdmin = new AdminDto { District = new DistrictDto { DistrictId = testId2 } };
            var district = new DistrictDto { DistrictId = testId, OAuthApplicationId = testId.ToString(), OAuthClientId = testId.ToString() };
            _adminRepositoryMock.Setup(x => x.GetByUsernameAsync(It.IsAny<String>())).ReturnsAsync(districtAdmin);
            _districtRepositoryMock.Setup(x => x.GetByIdAsync(testId)).ReturnsAsync(district);

            // Act
            try
            {
                await _sut.GetByIdAsync(testUsername, testId);
            }
            catch (Exception ex)
            {
                expectedException = ex;
            }

            // Assert
            Assert.IsNotNull(expectedException);
        }

        [TestMethod]
        public async Task DistrictService_GetByIdAsync_SchoolAdmin_ExistingId_ThrowException()
        {
            // Arrange
            Exception expectedException = null;
            var schoolAdmin = new AdminDto { School = new SchoolDto() };
            var district = new DistrictDto { DistrictId = testId, OAuthApplicationId = testId.ToString(), OAuthClientId = testId.ToString() };
            _adminRepositoryMock.Setup(x => x.GetByUsernameAsync(It.IsAny<String>())).ReturnsAsync(schoolAdmin);
            _districtRepositoryMock.Setup(x => x.GetByIdAsync(testId)).ReturnsAsync(district);

            // Act
            try
            {
                await _sut.GetByIdAsync(testUsername, testId);
            }
            catch (Exception ex)
            {
                expectedException = ex;
            }

            // Assert
            Assert.IsNotNull(expectedException);
        }

        [TestMethod]
        public async Task DistrictService_GetByIdAsync_NonAdmin_ExistingId_ThrowException()
        {
            // Arrange
            Exception expectedException = null;
            var district = new DistrictDto { DistrictId = testId, OAuthApplicationId = testId.ToString(), OAuthClientId = testId.ToString() };
            _adminRepositoryMock.Setup(x => x.GetByUsernameAsync(It.IsAny<String>())).ReturnsAsync(null);
            _districtRepositoryMock.Setup(x => x.GetByIdAsync(testId)).ReturnsAsync(district);

            // Act
            try
            {
                await _sut.GetByIdAsync(testUsername, testId);
            }
            catch (Exception ex)
            {
                expectedException = ex;
            }

            // Assert
            Assert.IsNotNull(expectedException);
        }

        [TestMethod]
        public async Task DistrictService_CreateAsync_NullEntity_Failure()
        {
            // Arrange
            DistrictDto district = null;
            _districtRepositoryMock.Setup(x => x.InsertAsync(district)).ReturnsAsync(null);

            // Act
            var result = await _sut.CreateAsync(null);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task DistrictService_CreateAsync_ValidEntity_Success()
        {
            // Arrange
            var district = new DistrictDto { DistrictId = testId, OAuthApplicationId = testId.ToString(), OAuthClientId = testId.ToString() };
            _districtRepositoryMock.Setup(x => x.InsertAsync(It.IsAny<DistrictDto>())).ReturnsAsync(district);

            // Act
            var result = await _sut.CreateAsync((District)district);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(district.DistrictId, result.DistrictId);
        }

        [TestMethod]
        public async Task DistrictService_CreateAsync_GlobalAdmin_ValidEntity_Success()
        {
            // Arrange
            var globalAdmin = new AdminDto();
            var district = new DistrictDto { DistrictId = testId, OAuthApplicationId = testId.ToString(), OAuthClientId = testId.ToString() };
            _adminRepositoryMock.Setup(x => x.GetByUsernameAsync(It.IsAny<String>())).ReturnsAsync(globalAdmin);
            _districtRepositoryMock.Setup(x => x.InsertAsync(It.IsAny<DistrictDto>())).ReturnsAsync(district);

            // Act
            var result = await _sut.CreateAsync(testUsername, (District)district);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(district.DistrictId, result.DistrictId);
        }

        [TestMethod]
        public async Task DistrictService_CreateAsync_DistrictAdmin_ValidEntity_Success()
        {
            // Arrange
            var districtAdmin = new AdminDto { District = new DistrictDto { DistrictId = testId } };
            var district = new DistrictDto { DistrictId = testId, OAuthApplicationId = testId.ToString(), OAuthClientId = testId.ToString() };
            _adminRepositoryMock.Setup(x => x.GetByUsernameAsync(It.IsAny<String>())).ReturnsAsync(districtAdmin);
            _districtRepositoryMock.Setup(x => x.InsertAsync(It.IsAny<DistrictDto>())).ReturnsAsync(district);

            // Act
            var result = await _sut.CreateAsync(testUsername, (District)district);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(district.DistrictId, result.DistrictId);
        }

        [TestMethod]
        public async Task DistrictService_CreateAsync_InvalidDistrictAdmin_ValidEntity_ThrowException()
        {
            // Arrange
            Exception expectedException = null;
            var districtAdmin = new AdminDto { District = new DistrictDto { DistrictId = testId2 } };
            var district = new DistrictDto { DistrictId = testId, OAuthApplicationId = testId.ToString(), OAuthClientId = testId.ToString() };
            _adminRepositoryMock.Setup(x => x.GetByUsernameAsync(It.IsAny<String>())).ReturnsAsync(districtAdmin);
            _districtRepositoryMock.Setup(x => x.InsertAsync(It.IsAny<DistrictDto>())).ReturnsAsync(district);

            // Act
            try
            {
                await _sut.CreateAsync(testUsername, (District)district);
            }
            catch (Exception ex)
            {
                expectedException = ex;
            }

            // Assert
            Assert.IsNotNull(expectedException);
        }

        [TestMethod]
        public async Task DistrictService_CreateAsync_SchoolAdmin_ValidEntity_ThrowException()
        {
            // Arrange
            Exception expectedException = null;
            var schoolAdmin = new AdminDto { School = new SchoolDto() };
            var district = new DistrictDto { DistrictId = testId, OAuthApplicationId = testId.ToString(), OAuthClientId = testId.ToString() };
            _adminRepositoryMock.Setup(x => x.GetByUsernameAsync(It.IsAny<String>())).ReturnsAsync(schoolAdmin);
            _districtRepositoryMock.Setup(x => x.InsertAsync(It.IsAny<DistrictDto>())).ReturnsAsync(district);

            // Act
            try
            {
                await _sut.CreateAsync(testUsername, (District)district);
            }
            catch (Exception ex)
            {
                expectedException = ex;
            }

            // Assert
            Assert.IsNotNull(expectedException);
        }

        [TestMethod]
        public async Task DistrictService_CreateAsync_NonAdmin_ValidEntity_ThrowException()
        {
            // Arrange
            Exception expectedException = null;
            var district = new DistrictDto { DistrictId = testId, OAuthApplicationId = testId.ToString(), OAuthClientId = testId.ToString() };
            _adminRepositoryMock.Setup(x => x.GetByUsernameAsync(It.IsAny<String>())).ReturnsAsync(null);
            _districtRepositoryMock.Setup(x => x.InsertAsync(It.IsAny<DistrictDto>())).ReturnsAsync(district);

            // Act
            try
            {
                await _sut.CreateAsync(testUsername, (District)district);
            }
            catch (Exception ex)
            {
                expectedException = ex;
            }

            // Assert
            Assert.IsNotNull(expectedException);
        }

        [TestMethod]
        public async Task DistrictService_UpdateAsync_NullEntity_Failure()
        {
            // Arrange
            DistrictDto district = null;
            _districtRepositoryMock.Setup(x => x.UpdateAsync(district)).ReturnsAsync(null);

            // Act
            var result = await _sut.UpdateAsync(null);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task DistrictService_UpdateAsync_ValidEntity_Success()
        {
            // Arrange
            var district = new DistrictDto { DistrictId = testId, OAuthApplicationId = testId.ToString(), OAuthClientId = testId.ToString() };
            _districtRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<DistrictDto>())).ReturnsAsync(district);

            // Act
            var result = await _sut.UpdateAsync((District)district);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(district.DistrictId, result.DistrictId);
        }

        [TestMethod]
        public async Task DistrictService_UpdateAsync_GlobalAdmin_ValidEntity_Success()
        {
            // Arrange
            var globalAdmin = new AdminDto();
            var district = new DistrictDto { DistrictId = testId, OAuthApplicationId = testId.ToString(), OAuthClientId = testId.ToString() };
            _adminRepositoryMock.Setup(x => x.GetByUsernameAsync(It.IsAny<String>())).ReturnsAsync(globalAdmin);
            _districtRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<DistrictDto>())).ReturnsAsync(district);

            // Act
            var result = await _sut.UpdateAsync(testUsername, (District)district);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(district.DistrictId, result.DistrictId);
        }

        [TestMethod]
        public async Task DistrictService_UpdateAsync_DistrictAdmin_ValidEntity_Success()
        {
            // Arrange
            var districtAdmin = new AdminDto { District = new DistrictDto { DistrictId = testId } };
            var district = new DistrictDto { DistrictId = testId, OAuthApplicationId = testId.ToString(), OAuthClientId = testId.ToString() };
            _adminRepositoryMock.Setup(x => x.GetByUsernameAsync(It.IsAny<String>())).ReturnsAsync(districtAdmin);
            _districtRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<DistrictDto>())).ReturnsAsync(district);

            // Act
            var result = await _sut.UpdateAsync(testUsername, (District)district);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(district.DistrictId, result.DistrictId);
        }

        [TestMethod]
        public async Task DistrictService_UpdateAsync_InvalidDistrictAdmin_ValidEntity_ThrowException()
        {
            // Arrange
            Exception expectedException = null;
            var districtAdmin = new AdminDto { District = new DistrictDto { DistrictId = testId2 } };
            var district = new DistrictDto { DistrictId = testId, OAuthApplicationId = testId.ToString(), OAuthClientId = testId.ToString() };
            _adminRepositoryMock.Setup(x => x.GetByUsernameAsync(It.IsAny<String>())).ReturnsAsync(districtAdmin);
            _districtRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<DistrictDto>())).ReturnsAsync(district);

            // Act
            try
            {
                await _sut.UpdateAsync(testUsername, (District)district);
            }
            catch (Exception ex)
            {
                expectedException = ex;
            }

            // Assert
            Assert.IsNotNull(expectedException);
        }

        [TestMethod]
        public async Task DistrictService_UpdateAsync_SchoolAdmin_ValidEntity_ThrowException()
        {
            // Arrange
            Exception expectedException = null;
            var schoolAdmin = new AdminDto { School = new SchoolDto() };
            var district = new DistrictDto { DistrictId = testId, OAuthApplicationId = testId.ToString(), OAuthClientId = testId.ToString() };
            _adminRepositoryMock.Setup(x => x.GetByUsernameAsync(It.IsAny<String>())).ReturnsAsync(schoolAdmin);
            _districtRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<DistrictDto>())).ReturnsAsync(district);

            // Act
            try
            {
                await _sut.UpdateAsync(testUsername, (District)district);
            }
            catch (Exception ex)
            {
                expectedException = ex;
            }

            // Assert
            Assert.IsNotNull(expectedException);
        }

        [TestMethod]
        public async Task DistrictService_UpdateAsync_NonAdmin_ValidEntity_ThrowException()
        {
            // Arrange
            Exception expectedException = null;
            var district = new DistrictDto { DistrictId = testId, OAuthApplicationId = testId.ToString(), OAuthClientId = testId.ToString() };
            _adminRepositoryMock.Setup(x => x.GetByUsernameAsync(It.IsAny<String>())).ReturnsAsync(null);
            _districtRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<DistrictDto>())).ReturnsAsync(district);

            // Act
            try
            {
                await _sut.UpdateAsync(testUsername, (District)district);
            }
            catch (Exception ex)
            {
                expectedException = ex;
            }

            // Assert
            Assert.IsNotNull(expectedException);
        }

        [TestMethod]
        public async Task DistrictService_DeleteAsync_NonExistingKey_Failure()
        {
            // Arrange
            const bool success = false;
            _districtRepositoryMock.Setup(x => x.DeleteAsync(testId)).ReturnsAsync(success);

            // Act
            var result = await _sut.DeleteAsync(testId);

            // Assert
            Assert.AreEqual(success, result);
        }

        [TestMethod]
        public async Task DistrictService_DeleteAsync_ExistingKey_Success()
        {
            // Arrange
            const bool success = true;
            _districtRepositoryMock.Setup(x => x.DeleteAsync(testId)).ReturnsAsync(success);

            // Act
            var result = await _sut.DeleteAsync(testId);

            // Assert
            Assert.AreEqual(success, result);
        }

        [TestMethod]
        public async Task DistrictService_DeleteAsync_GlobalAdmin_ExistingKey_Success()
        {
            // Arrange
            const bool success = true;
            var globalAdmin = new AdminDto();
            _adminRepositoryMock.Setup(x => x.GetByUsernameAsync(It.IsAny<String>())).ReturnsAsync(globalAdmin);
            _districtRepositoryMock.Setup(x => x.DeleteAsync(testId)).ReturnsAsync(success);

            // Act
            var result = await _sut.DeleteAsync(testUsername, testId);

            // Assert
            Assert.AreEqual(success, result);
        }

        [TestMethod]
        public async Task DistrictService_DeleteAsync_DistrictAdmin_ExistingKey_Success()
        {
            // Arrange
            const bool success = true;
            var districtAdmin = new AdminDto { District = new DistrictDto { DistrictId = testId } };
            _adminRepositoryMock.Setup(x => x.GetByUsernameAsync(It.IsAny<String>())).ReturnsAsync(districtAdmin);
            _districtRepositoryMock.Setup(x => x.DeleteAsync(testId)).ReturnsAsync(success);

            // Act
            var result = await _sut.DeleteAsync(testUsername, testId);

            // Assert
            Assert.AreEqual(success, result);
        }

        [TestMethod]
        public async Task DistrictService_DeleteAsync_InvalidDistrictAdmin_ExistingKey_ThrowException()
        {
            // Arrange
            Exception expectedException = null;
            const bool success = true;
            var districtAdmin = new AdminDto { District = new DistrictDto { DistrictId = testId2 } };
            _adminRepositoryMock.Setup(x => x.GetByUsernameAsync(It.IsAny<String>())).ReturnsAsync(districtAdmin);
            _districtRepositoryMock.Setup(x => x.DeleteAsync(testId)).ReturnsAsync(success);

            // Act
            try
            {
                await _sut.DeleteAsync(testUsername, testId);
            }
            catch (Exception ex)
            {
                expectedException = ex;
            }

            // Assert
            Assert.IsNotNull(expectedException);
        }

        [TestMethod]
        public async Task DistrictService_DeleteAsync_SchoolAdmin_ExistingKey_ThrowException()
        {
            // Arrange
            Exception expectedException = null;
            const bool success = true;
            var schoolAdmin = new AdminDto { School = new SchoolDto() };
            _adminRepositoryMock.Setup(x => x.GetByUsernameAsync(It.IsAny<String>())).ReturnsAsync(schoolAdmin);
            _districtRepositoryMock.Setup(x => x.DeleteAsync(testId)).ReturnsAsync(success);

            // Act
            try
            {
                await _sut.DeleteAsync(testUsername, testId);
            }
            catch (Exception ex)
            {
                expectedException = ex;
            }

            // Assert
            Assert.IsNotNull(expectedException);
        }

        [TestMethod]
        public async Task DistrictService_DeleteAsync_NonAdmin_ExistingKey_ThrowException()
        {
            // Arrange
            Exception expectedException = null;
            const bool success = true;
            _adminRepositoryMock.Setup(x => x.GetByUsernameAsync(It.IsAny<String>())).ReturnsAsync(null);
            _districtRepositoryMock.Setup(x => x.DeleteAsync(testId)).ReturnsAsync(success);

            // Act
            try
            {
                await _sut.DeleteAsync(testUsername, testId);
            }
            catch (Exception ex)
            {
                expectedException = ex;
            }

            // Assert
            Assert.IsNotNull(expectedException);
        }
    }
}
