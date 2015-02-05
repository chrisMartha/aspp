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
    public class SchoolServiceTest
    {
        private Mock<IAdminRepository> _adminRepositoryMock;
        private Mock<ISchoolRepository> _schoolRepositoryMock;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private SchoolService _sut;

        [TestInitialize]
        public void Initialize()
        {
            _adminRepositoryMock = new Mock<IAdminRepository>();
            _schoolRepositoryMock = new Mock<ISchoolRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            // Pre-arrange
            _unitOfWorkMock.Setup(x => x.GetDataRepository<AdminDto, Guid>()).Returns(_adminRepositoryMock.Object);
            _unitOfWorkMock.Setup(x => x.GetDataRepository<SchoolDto, Guid>()).Returns(_schoolRepositoryMock.Object);
            _unitOfWorkMock.SetupGet(x => x.AdminRepository).Returns(_adminRepositoryMock.Object);
            _unitOfWorkMock.SetupGet(x => x.SchoolRepository).Returns(_schoolRepositoryMock.Object);

            // Subject under test
            _sut = new SchoolService(_unitOfWorkMock.Object, _schoolRepositoryMock.Object);
        }

        #region GetAsync tests
        [TestMethod]
        public async Task SchoolService_GetAsync_ReturnsNone()
        {
            // Arrange
            var schools = new List<SchoolDto>();
            _schoolRepositoryMock.Setup(x => x.GetAsync()).ReturnsAsync(schools);

            // Act
            var result = await _sut.GetAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public async Task SchoolService_GetAsync_ReturnsOne()
        {
            // Arrange
            var school = new SchoolDto { SchoolID = Guid.NewGuid() };

            var schools = new List<SchoolDto> { school };

            _schoolRepositoryMock.Setup(x => x.GetAsync()).ReturnsAsync(schools);

            // Act
            var result = await _sut.GetAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public async Task SchoolService_GetAsync_ReturnsMany()
        {
            // Arrange
            var school1 = new SchoolDto { SchoolID = Guid.NewGuid() };
            var school2 = new SchoolDto { SchoolID = Guid.NewGuid() };
            var school3 = new SchoolDto { SchoolID = Guid.NewGuid() };

            var schools = new List<SchoolDto> { school1, school2, school3 };

            _schoolRepositoryMock.Setup(x => x.GetAsync()).ReturnsAsync(schools);

            // Act
            var result = await _sut.GetAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count);
        }
        #endregion

        #region GetByIdAsync tests
        [TestMethod]
        public async Task SchoolService_GetByIdAsync_NonExistentId_Failure()
        {
            // Arrange
            Guid schoolId = Guid.NewGuid();
            _schoolRepositoryMock.Setup(x => x.GetByIdAsync(schoolId)).ReturnsAsync(null);

            // Act
            School result = await _sut.GetByIdAsync(schoolId).ConfigureAwait(false);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task SchoolService_GetByIdAsync_ExistingId_Success()
        {
            // Arrange
            Guid schoolId = Guid.NewGuid();
            var schoolDto = new SchoolDto { SchoolID = schoolId };
            _schoolRepositoryMock.Setup(x => x.GetByIdAsync(schoolId)).ReturnsAsync(schoolDto);

            // Act
            School result = await _sut.GetByIdAsync(schoolId).ConfigureAwait(false);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result.SchoolId, schoolId);
        }
        #endregion

        #region CreateAsync tests
        [TestMethod]
        public async Task SchoolService_CreateAsync_NullEntity_Failure()
        {
            // Arrange
            SchoolDto schoolDto = null;
            _schoolRepositoryMock.Setup(x => x.InsertAsync(schoolDto)).ReturnsAsync(null);

            // Act
            School result = await _sut.CreateAsync(null).ConfigureAwait(false);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task SchoolService_CreateAsync_ValidEntity_Success()
        {
            // Arrange
            Guid schoolId = Guid.NewGuid();
            var school = new School { SchoolId = schoolId };
            SchoolDto schoolDto = (SchoolDto) school;
            _schoolRepositoryMock.Setup(x => x.InsertAsync(It.IsAny<SchoolDto>())).ReturnsAsync(schoolDto);

            // Act
            School result = await _sut.CreateAsync(school).ConfigureAwait(false);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result.SchoolId, schoolId);
        }
        #endregion

        #region UpdateAsync test
        [TestMethod]
        public async Task SchoolService_UpdateAsync_ThrowsNotImplementedException()
        {
            // Arrange
            Guid schoolId = Guid.NewGuid();
            var school = new School { SchoolId = schoolId };
            
            Exception thrownException = null;
            try
            {
                await _sut.UpdateAsync(school).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                thrownException = e;
            }

            // Assert
            Assert.IsNotNull(thrownException);
            Assert.IsInstanceOfType(thrownException, typeof(NotImplementedException));
        }
        #endregion

        #region DeleteAsync test
        [TestMethod]
        public async Task SchoolService_DeleteAsync_ThrowsNotImplementedException()
        {
            // Arrange
            Guid schoolId = Guid.NewGuid();

            Exception thrownException = null;
            try
            {
                await _sut.DeleteAsync(schoolId).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                thrownException = e;
            }

            // Assert
            Assert.IsNotNull(thrownException);
            Assert.IsInstanceOfType(thrownException, typeof(NotImplementedException));
        }
        #endregion

        #region GetByUsernameAsync tests
        [TestMethod]
        public async Task SchoolService_GetByUsernameAsync_AdminNotFound_ReturnsNone()
        {
            // Arrange
            _adminRepositoryMock.Setup(x => x.GetByUsernameAsync(It.IsAny<string>())).ReturnsAsync(null);

            // Act
            var result = await _sut.GetAsync(string.Empty);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public async Task SchoolService_GetByUsernameAsync_GlobalAdmin_ReturnsNone()
        {
            // Arrange
            var schools = new List<SchoolDto>();
            var admin = new AdminDto
            {
                User = new UserDto { UserID = Guid.NewGuid(), Username = "GlobalAdmin" },                
            };

            _schoolRepositoryMock.Setup(x => x.GetAsync()).ReturnsAsync(schools);
            _adminRepositoryMock.Setup(x => x.GetByUsernameAsync(It.IsAny<string>())).ReturnsAsync(admin);

            // Act
            var result = await _sut.GetAsync(admin.User.Username);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public async Task SchoolService_GetByUsernameAsync_DistrictAdmin_ReturnsNone()
        {
            // Arrange
            var schools = new List<SchoolDto>();
            var admin = new AdminDto
            {
                User = new UserDto { UserID = Guid.NewGuid(), Username = "DistrictAdmin" },
                District = new DistrictDto
                {
                     DistrictId = Guid.NewGuid()
                }
            };

            _schoolRepositoryMock.Setup(x => x.GetByDistrictIdAsync(It.IsAny<Guid>())).ReturnsAsync(schools);
            _adminRepositoryMock.Setup(x => x.GetByUsernameAsync(It.IsAny<string>())).ReturnsAsync(admin);

            // Act
            var result = await _sut.GetAsync(admin.User.Username);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public async Task SchoolService_GetByUsernameAsync_SchoolAdmin_ReturnsNone()
        {
            // Arrange
            var schools = new List<SchoolDto>();
            var admin = new AdminDto
            {
                User = new UserDto { UserID = Guid.NewGuid(), Username = "SchoolAdmin" },
                District = new DistrictDto
                {
                    DistrictId = Guid.NewGuid()
                },
                School = new SchoolDto
                {
                    SchoolID = Guid.NewGuid()
                }
            };

            _schoolRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(null);
            _schoolRepositoryMock.Setup(x => x.GetAsync()).ReturnsAsync(schools);
            _adminRepositoryMock.Setup(x => x.GetByUsernameAsync(It.IsAny<string>())).ReturnsAsync(admin);

            // Act
            var result = await _sut.GetAsync(admin.User.Username);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public async Task SchoolService_GetByUsernameAsync_GlobalAdmin_ReturnsOne()
        {
            // Arrange
            var school = new SchoolDto { SchoolID = Guid.NewGuid() };
            var schools = new List<SchoolDto> { school };

            var admin = new AdminDto
            {
                User = new UserDto { UserID = Guid.NewGuid(), Username = "GlobalAdmin" },
            };

            _schoolRepositoryMock.Setup(x => x.GetAsync()).ReturnsAsync(schools);
            _adminRepositoryMock.Setup(x => x.GetByUsernameAsync(It.IsAny<string>())).ReturnsAsync(admin);

            // Act
            var result = await _sut.GetAsync(admin.User.Username);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public async Task SchoolService_GetByUsernameAsync_DistrictAdmin_ReturnsOne()
        {
            // Arrange
            var school = new SchoolDto { SchoolID = Guid.NewGuid() };
            var schools = new List<SchoolDto> { school };

            var admin = new AdminDto
            {
                User = new UserDto { UserID = Guid.NewGuid(), Username = "DistrictAdmin" },
                District = new DistrictDto
                {
                    DistrictId = Guid.NewGuid()
                }
            };

            _schoolRepositoryMock.Setup(x => x.GetByDistrictIdAsync(It.IsAny<Guid>())).ReturnsAsync(schools);
            _adminRepositoryMock.Setup(x => x.GetByUsernameAsync(It.IsAny<string>())).ReturnsAsync(admin);

            // Act
            var result = await _sut.GetAsync(admin.User.Username);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public async Task SchoolService_GetByUsernameAsync_SchoolAdmin_ReturnsOne()
        {
            // Arrange
            var school = new SchoolDto { SchoolID = Guid.NewGuid() };
            var schools = new List<SchoolDto> { school };

            var admin = new AdminDto
            {
                User = new UserDto { UserID = Guid.NewGuid(), Username = "SchoolAdmin" },
                District = new DistrictDto
                {
                    DistrictId = Guid.NewGuid()
                },
                School = new SchoolDto
                {
                    SchoolID = Guid.NewGuid()
                }
            };

            _schoolRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(school);
            _schoolRepositoryMock.Setup(x => x.GetAsync()).ReturnsAsync(schools);
            _adminRepositoryMock.Setup(x => x.GetByUsernameAsync(It.IsAny<string>())).ReturnsAsync(admin);

            // Act
            var result = await _sut.GetAsync(admin.User.Username);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public async Task SchoolService_GetByUsernameAsync_ReturnsMany()
        {
            // Arrange
            var school1 = new SchoolDto { SchoolID = Guid.NewGuid() };
            var school2 = new SchoolDto { SchoolID = Guid.NewGuid() };
            var school3 = new SchoolDto { SchoolID = Guid.NewGuid() };

            var schools = new List<SchoolDto> { school1, school2, school3 };

            var admin = new AdminDto
            {
                User = new UserDto { UserID = Guid.NewGuid(), Username = "GlobalAdmin" },
            };

            _schoolRepositoryMock.Setup(x => x.GetAsync()).ReturnsAsync(schools);
            _adminRepositoryMock.Setup(x => x.GetByUsernameAsync(It.IsAny<string>())).ReturnsAsync(admin);

            // Act
            var result = await _sut.GetAsync(admin.User.Username);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count);
        }
        #endregion

        #region GetByDistrictIdAsync tests
        [TestMethod]
        public async Task SchoolService_GetByDistrictIdAsync_NullId_ReturnsNone()
        {
            // Arrange
            var schools = new List<SchoolDto>();

            _schoolRepositoryMock.Setup(x => x.GetAsync()).ReturnsAsync(schools);
            _schoolRepositoryMock.Setup(x => x.GetByDistrictIdAsync(It.IsAny<Guid>())).ReturnsAsync(schools);

            // Act
            var result = await _sut.GetByDistrictIdAsync(null);

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public async Task SchoolService_GetByDistrictIdAsync_NullId_ReturnsOne()
        {
            // Arrange
            var school = new SchoolDto { SchoolID = Guid.NewGuid() };
            var schools = new List<SchoolDto> { school };

            _schoolRepositoryMock.Setup(x => x.GetAsync()).ReturnsAsync(schools);
            _schoolRepositoryMock.Setup(x => x.GetByDistrictIdAsync(It.IsAny<Guid>())).ReturnsAsync(schools);

            // Act
            var result = await _sut.GetByDistrictIdAsync(null);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public async Task SchoolService_GetByDistrictIdAsync_NullId_ReturnsMany()
        {
            // Arrange
            var school1 = new SchoolDto { SchoolID = Guid.NewGuid() };
            var school2 = new SchoolDto { SchoolID = Guid.NewGuid() };
            var school3 = new SchoolDto { SchoolID = Guid.NewGuid() };

            var schools = new List<SchoolDto> { school1, school2, school3 };

            _schoolRepositoryMock.Setup(x => x.GetAsync()).ReturnsAsync(schools);
            _schoolRepositoryMock.Setup(x => x.GetByDistrictIdAsync(It.IsAny<Guid>())).ReturnsAsync(schools);

            // Act
            var result = await _sut.GetByDistrictIdAsync(null);

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count);
        }

        [TestMethod]
        public async Task SchoolService_GetByDistrictIdAsync_WithId_ReturnsNone()
        {
            // Arrange
            var schools = new List<SchoolDto>();

            _schoolRepositoryMock.Setup(x => x.GetAsync()).ReturnsAsync(schools);
            _schoolRepositoryMock.Setup(x => x.GetByDistrictIdAsync(It.IsAny<Guid>())).ReturnsAsync(schools);

            // Act
            var result = await _sut.GetByDistrictIdAsync(Guid.NewGuid());

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public async Task SchoolService_GetByDistrictIdAsync_WithId_ReturnsOne()
        {
            // Arrange
            var school = new SchoolDto { SchoolID = Guid.NewGuid() };
            var schools = new List<SchoolDto> { school };

            _schoolRepositoryMock.Setup(x => x.GetAsync()).ReturnsAsync(schools);
            _schoolRepositoryMock.Setup(x => x.GetByDistrictIdAsync(It.IsAny<Guid>())).ReturnsAsync(schools);

            // Act
            var result = await _sut.GetByDistrictIdAsync(Guid.NewGuid());

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public async Task SchoolService_GetByDistrictIdAsync_WithId_ReturnsMany()
        {
            // Arrange
            var school1 = new SchoolDto { SchoolID = Guid.NewGuid() };
            var school2 = new SchoolDto { SchoolID = Guid.NewGuid() };
            var school3 = new SchoolDto { SchoolID = Guid.NewGuid() };

            var schools = new List<SchoolDto> { school1, school2, school3 };

            _schoolRepositoryMock.Setup(x => x.GetAsync()).ReturnsAsync(schools);
            _schoolRepositoryMock.Setup(x => x.GetByDistrictIdAsync(It.IsAny<Guid>())).ReturnsAsync(schools);

            // Act
            var result = await _sut.GetByDistrictIdAsync(Guid.NewGuid());

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count);
        }
        #endregion

    }
}
