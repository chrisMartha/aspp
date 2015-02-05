using System;
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
    public class AccessPointServiceTest
    {
        private Mock<IAdminRepository> _adminRepositoryMock;
        private Mock<IAccessPointRepository> _accessPointRepositoryMock;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private AccessPointService _sut;
        private readonly String testUsername = "test";
        private readonly Guid testGuid = new Guid("E6D60709-C9B3-4083-9D30-1A6F63A5B77C");
        private readonly Guid testGuid2 = new Guid("F7D60709-C9B3-4083-9D30-1A6F63A5B77C");
        private readonly String testId = "E6D60709-C9B3-4083-9D30-1A6F63A5B77C".Substring(0, 17);
        private readonly String testId2 = "F7D60709-C9B3-4083-9D30-1A6F63A5B77C".Substring(0, 17);

        [TestInitialize]
        public void Initialize()
        {
            _adminRepositoryMock = new Mock<IAdminRepository>();
            _accessPointRepositoryMock = new Mock<IAccessPointRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            // Pre-arrange
            _unitOfWorkMock.Setup(x => x.GetDataRepository<AdminDto, Guid>()).Returns(_adminRepositoryMock.Object);
            _unitOfWorkMock.Setup(x => x.GetDataRepository<AccessPointDto, String>()).Returns(_accessPointRepositoryMock.Object);
            _unitOfWorkMock.SetupGet(x => x.AdminRepository).Returns(_adminRepositoryMock.Object);
            _unitOfWorkMock.SetupGet(x => x.AccessPointRepository).Returns(_accessPointRepositoryMock.Object);

            // Subject under test
            _sut = new AccessPointService(_unitOfWorkMock.Object);
        }

        [TestMethod]
        public void AccessPointService_CastAccessPointDtoToAccessPoint()
        {
            // Arrange
            var accessPointDto = new AccessPointDto
            {
                WifiBSSID = testId, District = new DistrictDto { DistrictId = testGuid },
                School = new SchoolDto { SchoolID = testGuid },
                Classroom = new ClassroomDto { ClassroomID = testGuid }
            };

            // Act
            var accessPoint = (AccessPoint)accessPointDto;

            // Assert
            Assert.AreEqual(accessPointDto.WifiBSSID, accessPoint.WifiBSSId);
            Assert.AreEqual(accessPointDto.District.DistrictId, accessPoint.DistrictId);
            Assert.AreEqual(accessPointDto.School.SchoolID, accessPoint.SchoolId);
            Assert.AreEqual(accessPointDto.Classroom.ClassroomID, accessPoint.ClassroomId);
        }

        [TestMethod]
        public void AccessPointService_CastAccessPointToAccessPointDto()
        {
            // Arrange
            var accessPoint = new AccessPoint
            {
                WifiBSSId = testId,
                DistrictId = testGuid,
                SchoolId = testGuid,
                ClassroomId = testGuid
            };

            // Act
            var accessPointDto = (AccessPointDto)accessPoint;

            // Assert
            Assert.AreEqual(accessPoint.WifiBSSId, accessPointDto.WifiBSSID);
            Assert.AreEqual(accessPoint.DistrictId, accessPointDto.District.DistrictId);
            Assert.AreEqual(accessPoint.SchoolId, accessPointDto.School.SchoolID);
            Assert.AreEqual(accessPoint.ClassroomId, accessPointDto.Classroom.ClassroomID);
        }

        [TestMethod]
        public async Task AccessPointService_GetAsync_ReturnsNone()
        {
            // Arrange
            var accessPoints = new List<AccessPointDto>();
            _accessPointRepositoryMock.Setup(x => x.GetAsync()).ReturnsAsync(accessPoints);

            // Act
            var result = await _sut.GetAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public async Task AccessPointService_GetAsync_ReturnsMany()
        {
            // Arrange
            var accessPoint1 = new AccessPointDto { WifiBSSID = testId };
            var accessPoint2 = new AccessPointDto { WifiBSSID = testId2 };
            var accessPoints = new List<AccessPointDto>
            {
                accessPoint1,
                accessPoint2
            };
            _accessPointRepositoryMock.Setup(x => x.GetAsync()).ReturnsAsync(accessPoints);

            // Act
            var result = await _sut.GetAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(accessPoint1.WifiBSSID, result[0].WifiBSSId);
            Assert.AreEqual(accessPoint2.WifiBSSID, result[1].WifiBSSId);
        }

        [TestMethod]
        public async Task AccessPointService_GetAsync_GlobalAdmin_ReturnsMany()
        {
            // Arrange
            var globalAdmin = new AdminDto();
            var accessPoint1 = new AccessPointDto { WifiBSSID = testId };
            var accessPoint2 = new AccessPointDto { WifiBSSID = testId2 };
            var accessPoints = new List<AccessPointDto>
            {
                accessPoint1,
                accessPoint2
            };
            _adminRepositoryMock.Setup(x => x.GetByUsernameAsync(It.IsAny<String>())).ReturnsAsync(globalAdmin);
            _accessPointRepositoryMock.Setup(x => x.GetAsync()).ReturnsAsync(accessPoints);

            // Act
            var result = await _sut.GetAsync(testUsername);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(accessPoint1.WifiBSSID, result[0].WifiBSSId);
            Assert.AreEqual(accessPoint2.WifiBSSID, result[1].WifiBSSId);
        }

        [TestMethod]
        public async Task AccessPointService_GetAsync_DistrictAdmin_ReturnsOne()
        {
            // Arrange
            var districtAdmin = new AdminDto { District = new DistrictDto { DistrictId = testGuid } };
            var accessPoint1 = new AccessPointDto { WifiBSSID = testId };
            var accessPoint2 = new AccessPointDto { WifiBSSID = testId2 };
            var accessPoints = new List<AccessPointDto>
            {
                accessPoint1,
                accessPoint2
            };
            var accessPoints2 = new List<AccessPointDto>
            {
                accessPoint1
            };
            _adminRepositoryMock.Setup(x => x.GetByUsernameAsync(It.IsAny<String>())).ReturnsAsync(districtAdmin);
            _accessPointRepositoryMock.Setup(x => x.GetAsync()).ReturnsAsync(accessPoints);
            _accessPointRepositoryMock.Setup(x => x.GetByDistrictAsync(testGuid)).ReturnsAsync(accessPoints2);

            // Act
            var result = await _sut.GetAsync(testUsername);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(accessPoint1.WifiBSSID, result[0].WifiBSSId);
        }

        [TestMethod]
        public async Task AccessPointService_GetAsync_SchoolAdmin_ReturnsOne()
        {
            // Arrange
            var schoolAdmin = new AdminDto { School = new SchoolDto { SchoolID = testGuid } };
            var accessPoint1 = new AccessPointDto { WifiBSSID = testId };
            var accessPoint2 = new AccessPointDto { WifiBSSID = testId2 };
            var accessPoints = new List<AccessPointDto>
            {
                accessPoint1,
                accessPoint2
            };
            var accessPoints2 = new List<AccessPointDto>
            {
                accessPoint1
            };
            _adminRepositoryMock.Setup(x => x.GetByUsernameAsync(It.IsAny<String>())).ReturnsAsync(schoolAdmin);
            _accessPointRepositoryMock.Setup(x => x.GetAsync()).ReturnsAsync(accessPoints);
            _accessPointRepositoryMock.Setup(x => x.GetBySchoolAsync(testGuid)).ReturnsAsync(accessPoints2);

            // Act
            var result = await _sut.GetAsync(testUsername);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(accessPoint1.WifiBSSID, result[0].WifiBSSId);
        }

        [TestMethod]
        public async Task AccessPointService_GetAsync_NonAdmin_ReturnsOne()
        {
            // Arrange
            Exception expectedException = null;
            var accessPoint1 = new AccessPointDto { WifiBSSID = testId };
            var accessPoint2 = new AccessPointDto { WifiBSSID = testId2 };
            var accessPoints = new List<AccessPointDto>
            {
                accessPoint1,
                accessPoint2
            };
            var accessPoints2 = new List<AccessPointDto>
            {
                accessPoint1
            };
            _adminRepositoryMock.Setup(x => x.GetByUsernameAsync(It.IsAny<String>())).ReturnsAsync(null);
            _accessPointRepositoryMock.Setup(x => x.GetAsync()).ReturnsAsync(accessPoints);
            _accessPointRepositoryMock.Setup(x => x.GetBySchoolAsync(testGuid)).ReturnsAsync(accessPoints2);

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
        public async Task AccessPointService_GetByDistrictAsync_ReturnsMany()
        {
            // Arrange
            var accessPoint1 = new AccessPointDto { WifiBSSID = testId };
            var accessPoint2 = new AccessPointDto { WifiBSSID = testId2 };
            var accessPoints = new List<AccessPointDto>
            {
                accessPoint1,
                accessPoint2
            };
            _accessPointRepositoryMock.Setup(x => x.GetByDistrictAsync(It.IsAny<Guid>())).ReturnsAsync(accessPoints);

            // Act
            var result = await _sut.GetByDistrictAsync(testGuid);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(accessPoint1.WifiBSSID, result[0].WifiBSSId);
            Assert.AreEqual(accessPoint2.WifiBSSID, result[1].WifiBSSId);
        }

        [TestMethod]
        public async Task AccessPointService_GetBySchoolAsync_ReturnsMany()
        {
            // Arrange
            var accessPoint1 = new AccessPointDto { WifiBSSID = testId };
            var accessPoint2 = new AccessPointDto { WifiBSSID = testId2 };
            var accessPoints = new List<AccessPointDto>
            {
                accessPoint1,
                accessPoint2
            };
            _accessPointRepositoryMock.Setup(x => x.GetBySchoolAsync(It.IsAny<Guid>())).ReturnsAsync(accessPoints);

            // Act
            var result = await _sut.GetBySchoolAsync(testGuid);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(accessPoint1.WifiBSSID, result[0].WifiBSSId);
            Assert.AreEqual(accessPoint2.WifiBSSID, result[1].WifiBSSId);
        }

        [TestMethod]
        public async Task AccessPointService_GetByIdAsync_NonExistentId_Failure()
        {
            // Arrange
            _accessPointRepositoryMock.Setup(x => x.GetByIdAsync(testId)).ReturnsAsync(null);

            // Act
            var result = await _sut.GetByIdAsync(testId);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task AccessPointService_GetByIdAsync_ExistingId_Success()
        {
            // Arrange
            var accessPoint = new AccessPointDto { WifiBSSID = testId };
            _accessPointRepositoryMock.Setup(x => x.GetByIdAsync(testId)).ReturnsAsync(accessPoint);

            // Act
            var result = await _sut.GetByIdAsync(testId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(accessPoint.WifiBSSID, result.WifiBSSId);
        }

        // TODO: Add tests for GetByIdAsync for global admin/district admin/invalid district admin/school admin/non-admin

        [TestMethod]
        public async Task AccessPointService_CreateAsync_NullEntity_Failure()
        {
            // Arrange
            AccessPointDto accessPoint = null;
            _accessPointRepositoryMock.Setup(x => x.InsertAsync(accessPoint)).ReturnsAsync(null);

            // Act
            var result = await _sut.CreateAsync(null);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task AccessPointService_CreateAsync_ValidEntity_Success()
        {
            // Arrange
            var accessPoint = new AccessPointDto { WifiBSSID = testId };
            _accessPointRepositoryMock.Setup(x => x.InsertAsync(It.IsAny<AccessPointDto>())).ReturnsAsync(accessPoint);

            // Act
            var result = await _sut.CreateAsync((AccessPoint)accessPoint);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result.WifiBSSId, accessPoint.WifiBSSID);
        }

        // TODO: Add tests for CreateAsync for global admin/district admin/invalid district admin/school admin/non-admin

        [TestMethod]
        public async Task AccessPointService_UpdateAsync_NullEntity_Failure()
        {
            // Arrange
            AccessPointDto accessPoint = null;
            _accessPointRepositoryMock.Setup(x => x.UpdateAsync(accessPoint)).ReturnsAsync(null);

            // Act
            AccessPoint result = await _sut.UpdateAsync(null);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task AccessPointService_UpdateAsync_ValidEntity_Success()
        {
            // Arrange
            var accessPoint = new AccessPointDto { WifiBSSID = testId };
            _accessPointRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<AccessPointDto>())).ReturnsAsync(accessPoint);

            // Act
            var result = await _sut.UpdateAsync((AccessPoint)accessPoint);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(accessPoint.WifiBSSID, result.WifiBSSId);
        }

        [TestMethod]
        public async Task AccessPointService_UpdateAsync_GlobalAdmin_ValidEntity_Success()
        {
            // Arrange
            var globalAdmin = new AdminDto();
            var accessPoint = new AccessPointDto { WifiBSSID = testId };
            _adminRepositoryMock.Setup(x => x.GetByUsernameAsync(It.IsAny<String>())).ReturnsAsync(globalAdmin);
            _accessPointRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<AccessPointDto>())).ReturnsAsync(accessPoint);

            // Act
            var result = await _sut.UpdateAsync(testUsername, (AccessPoint)accessPoint);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(accessPoint.WifiBSSID, result.WifiBSSId);
        }

        [TestMethod]
        public async Task AccessPointService_UpdateAsync_DistrictAdmin_ValidEntity_Success()
        {
            // Arrange
            var districtAdmin = new AdminDto { District = new DistrictDto { DistrictId = testGuid } };
            var accessPoint = new AccessPointDto { WifiBSSID = testId, District = new DistrictDto { DistrictId = testGuid}};
            _adminRepositoryMock.Setup(x => x.GetByUsernameAsync(It.IsAny<String>())).ReturnsAsync(districtAdmin);
            _accessPointRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<AccessPointDto>())).ReturnsAsync(accessPoint);

            // Act
            var result = await _sut.UpdateAsync(testUsername, (AccessPoint)accessPoint);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(accessPoint.WifiBSSID, result.WifiBSSId);
        }

        [TestMethod]
        public async Task AccessPointService_UpdateAsync_InvalidDistrictAdmin_ValidEntity_ThrowException()
        {
            // Arrange
            Exception expectedException = null;
            var districtAdmin = new AdminDto { District = new DistrictDto { DistrictId = testGuid2 } };
            var accessPoint = new AccessPointDto { WifiBSSID = testId, District = new DistrictDto { DistrictId = testGuid } };
            _adminRepositoryMock.Setup(x => x.GetByUsernameAsync(It.IsAny<String>())).ReturnsAsync(districtAdmin);
            _accessPointRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<AccessPointDto>())).ReturnsAsync(accessPoint);

            // Act
            try
            {
                await _sut.UpdateAsync(testUsername, (AccessPoint)accessPoint);
            }
            catch (Exception ex)
            {
                expectedException = ex;
            }

            // Assert
            Assert.IsNotNull(expectedException);
        }

        [TestMethod]
        public async Task AccessPointService_UpdateAsync_SchoolAdmin_ValidEntity_Success()
        {
            // Arrange
            var schoolAdmin = new AdminDto { School = new SchoolDto { SchoolID = testGuid } };
            var accessPoint = new AccessPointDto { WifiBSSID = testId, School = new SchoolDto { SchoolID = testGuid } };
            _adminRepositoryMock.Setup(x => x.GetByUsernameAsync(It.IsAny<String>())).ReturnsAsync(schoolAdmin);
            _accessPointRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<AccessPointDto>())).ReturnsAsync(accessPoint);

            // Act
            var result = await _sut.UpdateAsync(testUsername, (AccessPoint)accessPoint);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(accessPoint.WifiBSSID, result.WifiBSSId);
        }

        [TestMethod]
        public async Task AccessPointService_UpdateAsync_InvalidSchoolAdmin_ValidEntity_ThrowException()
        {
            // Arrange
            Exception expectedException = null;
            var schoolAdmin = new AdminDto { School = new SchoolDto { SchoolID = testGuid2 } };
            var accessPoint = new AccessPointDto { WifiBSSID = testId, School = new SchoolDto { SchoolID = testGuid } };
            _adminRepositoryMock.Setup(x => x.GetByUsernameAsync(It.IsAny<String>())).ReturnsAsync(schoolAdmin);
            _accessPointRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<AccessPointDto>())).ReturnsAsync(accessPoint);

            // Act
            try
            {
                await _sut.UpdateAsync(testUsername, (AccessPoint)accessPoint);
            }
            catch (Exception ex)
            {
                expectedException = ex;
            }

            // Assert
            Assert.IsNotNull(expectedException);
        }

        [TestMethod]
        public async Task AccessPointService_UpdateAsync_NonAdmin_ValidEntity_ThrowException()
        {
            // Arrange
            Exception expectedException = null;
            var accessPoint = new AccessPointDto { WifiBSSID = testId, District = new DistrictDto { DistrictId = testGuid }, School = new SchoolDto { SchoolID = testGuid } };
            _adminRepositoryMock.Setup(x => x.GetByUsernameAsync(It.IsAny<String>())).ReturnsAsync(null);
            _accessPointRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<AccessPointDto>())).ReturnsAsync(accessPoint);

            // Act
            try
            {
                await _sut.UpdateAsync(testUsername, (AccessPoint)accessPoint);
            }
            catch (Exception ex)
            {
                expectedException = ex;
            }

            // Assert
            Assert.IsNotNull(expectedException);
        }

        [TestMethod]
        public async Task AccessPointService_DeleteAsync_NonExistingKey_Failure()
        {
            // Arrange
            const bool success = false;
            _accessPointRepositoryMock.Setup(x => x.DeleteAsync(testId)).ReturnsAsync(success);

            // Act
            var result = await _sut.DeleteAsync(testId);

            // Assert
            Assert.AreEqual(success, result);
        }

        [TestMethod]
        public async Task AccessPointService_DeleteAsync_ExistingKey_Success()
        {
            // Arrange
            const bool success = true;
            _accessPointRepositoryMock.Setup(x => x.DeleteAsync(testId)).ReturnsAsync(success);

            // Act
            var result = await _sut.DeleteAsync(testId);

            // Assert
            Assert.AreEqual(success, result);
        }

        // TODO: Add tests for DeleteAsync for global admin/district admin/invalid district admin/school admin/non-admin
    }
}
