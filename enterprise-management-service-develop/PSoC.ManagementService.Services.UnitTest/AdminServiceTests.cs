using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

using PSoC.ManagementService.Core;
using PSoC.ManagementService.Data.Interfaces;
using PSoC.ManagementService.Data.Models;
using PSoC.ManagementService.Services.Models;

namespace PSoC.ManagementService.Services.UnitTest
{
    [TestClass]
    public class AdminServiceTests
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IAdminRepository> _adminRepositoryMock;
        private AdminService _sut;

        [TestInitialize]
        public void Initialize()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _adminRepositoryMock = new Mock<IAdminRepository>();

            // Subject under test
            _sut = new AdminService(_unitOfWorkMock.Object,_adminRepositoryMock.Object);
        }

        [TestMethod]
        public async Task AdminService_GetByUsernameAsync_EmptyUsername_ThrowsArgumentException()
        {
            //Arrange
            string userName = string.Empty;
            
            // Act
            Admin result = null;
            Exception thrownException = null;
            try
            {
                result = await _sut.GetByUsernameAsync(userName);
            }
            catch (Exception e)
            {
                thrownException = e;
            }

            // Assert
            Assert.IsNull(result);
            Assert.IsNotNull(thrownException);
            Assert.IsInstanceOfType(thrownException, typeof(ArgumentException));
        }

        [TestMethod]
        public async Task AdminService_GetByUsernameAsync_Success()
        {
            var dto = new AdminDto
            {
                Active = true,
                AdminEmail = "gadmin@pearson.com",
                Created = DateTime.Now.AddDays(-50),
                District = null,
                LastLoginDateTime = DateTime.Now.AddMinutes(-5),
                School = null,
                User = new UserDto
                {
                    Created = DateTime.Now.AddDays(-50),
                    UserID = Guid.NewGuid(),
                    Username = "gadmintest001",
                    UserType = "Global Admin"
                }
            };

            _adminRepositoryMock.Setup(x => x.GetByUsernameAsync(It.IsAny<string>())).ReturnsAsync(dto);
            
            // Act
            Admin result = null;
            Exception thrownException = null;
            try
            {
                result = await _sut.GetByUsernameAsync("testusername");
            }
            catch (Exception e)
            {
                thrownException = e;
            }

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNull(thrownException);

            Assert.AreEqual(dto.Active, result.Active);
            Assert.AreEqual(dto.AdminEmail, result.AdminEmail);
            Assert.AreEqual(dto.LastLoginDateTime, result.LastLoginDateTime);

            Assert.IsNull(result.DistrictId);
            Assert.IsNull(result.DistrictName);
            Assert.IsNull(result.SchoolId);
            Assert.IsNull(result.SchoolName);

            Assert.AreEqual(dto.User.UserID, result.UserId);
            Assert.AreEqual(dto.User.Username, result.Username);
            Assert.AreEqual(dto.User.UserType, result.UserType);
            Assert.AreEqual(AdminType.GlobalAdmin, result.AdminType);
        }


        [TestMethod]
        public async Task AdminService_UpdateLastLoginDateTime_EmptyId_ThrowsArgumentException()
        {
            // Arrange
            Guid id = Guid.Empty;
            DateTime lastLogin = DateTime.UtcNow;

            // Act
            Exception thrownException = null;
            try
            {
                await _sut.UpdateLastLoginDateTime(id, lastLogin);
            }
            catch (Exception e)
            {
                thrownException = e;
            }

            // Assert
            Assert.IsNotNull(thrownException);
            Assert.IsInstanceOfType(thrownException, typeof(ArgumentException));
        }
        
        [TestMethod]
        public async Task AdminService_DeleteAsync_Failure()
        {
            _adminRepositoryMock.Setup(x => x.DeleteAsync(It.IsAny<Guid>())).ReturnsAsync(false);

            var result = (await _sut.DeleteAsync(Guid.NewGuid()).ConfigureAwait(false));

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task AdminService_DeleteAsync_Success()
        {
            _adminRepositoryMock.Setup(x => x.DeleteAsync(It.IsAny<Guid>())).ReturnsAsync(true);

            var result = (await _sut.DeleteAsync(Guid.NewGuid()).ConfigureAwait(false));

            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task AdminService_GetAsync_ReturnsNone()
        {
            _adminRepositoryMock.Setup(x => x.GetAsync()).ReturnsAsync(new List<AdminDto>());

            var result = await _sut.GetAsync().ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count() == 0);
        }


        [TestMethod]
        public async Task AdminService_GetAsync_ReturnsOne()
        {
            var dto = new AdminDto
            {
                Active = true,
                AdminEmail = "gadmin@pearson.com",
                Created = DateTime.Now.AddDays(-50),
                District = null,
                LastLoginDateTime = DateTime.Now.AddMinutes(-5),
                School = null, 
                User = new UserDto
                {
                    Created = DateTime.Now.AddDays(-50),
                    UserID = Guid.NewGuid(),
                    Username = "gadmintest001",
                    UserType = "Global Admin"
                }
            };

            _adminRepositoryMock.Setup(x => x.GetAsync()).ReturnsAsync(new List<AdminDto> { dto });

            var result = await _sut.GetAsync().ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count() == 1);

            var admin = result.FirstOrDefault();

            Assert.AreEqual(dto.Active, admin.Active);
            Assert.AreEqual(dto.AdminEmail, admin.AdminEmail);
            Assert.AreEqual(dto.LastLoginDateTime, admin.LastLoginDateTime);

            Assert.IsNull(admin.DistrictId);
            Assert.IsNull(admin.DistrictName);
            Assert.IsNull(admin.SchoolId);
            Assert.IsNull(admin.SchoolName);

            Assert.AreEqual(dto.User.UserID, admin.UserId);
            Assert.AreEqual(dto.User.Username, admin.Username);
            Assert.AreEqual(dto.User.UserType, admin.UserType);
            Assert.AreEqual(AdminType.GlobalAdmin, admin.AdminType);
        }


        [TestMethod]
        public async Task AdminService_GetAsync_ReturnsMany()
        {
            var dto1 = new AdminDto
            {
                Active = true,
                AdminEmail = "dadmin@pearson.com",
                Created = DateTime.Now.AddDays(-20),
                District = new DistrictDto
                {
                    DistrictId = Guid.NewGuid(),
                    DistrictName = "Test District"
                },
                LastLoginDateTime = DateTime.Now.AddMinutes(-1),
                School = null,
                User = new UserDto
                {
                    Created = DateTime.Now.AddDays(-20),
                    UserID = Guid.NewGuid(),
                    Username = "dadmintest001",
                    UserType = "District Admin"
                }
            };

            var dto2 = new AdminDto
            {
                Active = false,
                AdminEmail = "sadmin@pearson.com",
                Created = DateTime.Now.AddDays(-10),
                District = null,
                LastLoginDateTime = DateTime.Now.AddMinutes(-60),
                School = new SchoolDto
                {
                    SchoolID = Guid.NewGuid(),
                    SchoolName = "Private Test School"
                },
                User = new UserDto
                {
                    Created = DateTime.Now.AddDays(-80),
                    UserID = Guid.NewGuid(),
                    Username = "sadmintest001",
                    UserType = "School Admin"
                }
            };

            _adminRepositoryMock.Setup(x => x.GetAsync()).ReturnsAsync(new List<AdminDto> { dto1, dto2 });

            var result = await _sut.GetAsync().ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count() == 2);

            var admin1 = result.FirstOrDefault(a => a.UserId == dto1.User.UserID);
            var admin2 = result.FirstOrDefault(a => a.UserId == dto2.User.UserID);

            Assert.IsNotNull(admin1);
            Assert.IsNotNull(admin2);

            //Test for first Admin
            Assert.AreEqual(dto1.Active, admin1.Active);
            Assert.AreEqual(dto1.AdminEmail, admin1.AdminEmail);
            Assert.AreEqual(dto1.LastLoginDateTime, admin1.LastLoginDateTime);

            Assert.IsNotNull(admin1.DistrictId);
            Assert.IsNotNull(admin1.DistrictName);
            Assert.AreEqual(dto1.District.DistrictId, admin1.DistrictId);
            Assert.AreEqual(dto1.District.DistrictName, admin1.DistrictName);

            Assert.IsNull(admin1.SchoolId);
            Assert.IsNull(admin1.SchoolName);

            Assert.AreEqual(dto1.User.UserID, admin1.UserId);
            Assert.AreEqual(dto1.User.Username, admin1.Username);
            Assert.AreEqual(dto1.User.UserType, admin1.UserType);
            Assert.AreEqual(AdminType.DistrictAdmin, admin1.AdminType);

            //Test for second Admin
            Assert.AreEqual(dto2.Active, admin2.Active);
            Assert.AreEqual(dto2.AdminEmail, admin2.AdminEmail);
            Assert.AreEqual(dto2.LastLoginDateTime, admin2.LastLoginDateTime);

            Assert.IsNull(admin2.DistrictId);
            Assert.IsNull(admin2.DistrictName);

            Assert.IsNotNull(admin2.SchoolId);
            Assert.IsNotNull(admin2.SchoolName);
            Assert.AreEqual(dto2.School.SchoolID, admin2.SchoolId);
            Assert.AreEqual(dto2.School.SchoolName, admin2.SchoolName);

            Assert.AreEqual(dto2.User.UserID, admin2.UserId);
            Assert.AreEqual(dto2.User.Username, admin2.Username);
            Assert.AreEqual(dto2.User.UserType, admin2.UserType);
            Assert.AreEqual(AdminType.SchoolAdmin, admin2.AdminType);
        }

        [TestMethod]
        public async Task AdminService_GetByIdAsync_Success()
        {
            var dto = new AdminDto
            {
                Active = true,
                AdminEmail = "sadmin@pearson.com",
                Created = DateTime.Now.AddDays(-20),
                District = new DistrictDto
                {
                    DistrictId = Guid.NewGuid(),
                    DistrictName = "Test District"
                },
                LastLoginDateTime = DateTime.Now.AddMinutes(-1),
                School = new SchoolDto
                {
                    SchoolID = Guid.NewGuid(),
                    SchoolName = "Private Test School"
                },
                User = new UserDto
                {
                    Created = DateTime.Now.AddDays(-80),
                    UserID = Guid.NewGuid(),
                    Username = "sadmintest001",
                    UserType = "School Admin"
                }
            };

            _adminRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(dto);

            var result = await _sut.GetByIdAsync(Guid.NewGuid()).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.AreEqual(dto.Active, result.Active);
            Assert.AreEqual(dto.AdminEmail, result.AdminEmail);
            Assert.AreEqual(dto.LastLoginDateTime, result.LastLoginDateTime);

            Assert.IsNotNull(result.DistrictId);
            Assert.IsNotNull(result.DistrictName);
            Assert.AreEqual(dto.District.DistrictId, result.DistrictId);
            Assert.AreEqual(dto.District.DistrictName, result.DistrictName);

            Assert.IsNotNull(result.SchoolId);
            Assert.IsNotNull(result.SchoolName);
            Assert.AreEqual(dto.School.SchoolID, result.SchoolId);
            Assert.AreEqual(dto.School.SchoolName, result.SchoolName);

            Assert.AreEqual(dto.User.UserID, result.UserId);
            Assert.AreEqual(dto.User.Username, result.Username);
            Assert.AreEqual(dto.User.UserType, result.UserType);
            Assert.AreEqual(AdminType.SchoolAdmin, result.AdminType);
        }

        [TestMethod]
        public async Task AdminService_GetByIdAsync_NotFound()
        {
            _adminRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(null);

            var result = await _sut.GetByIdAsync(Guid.NewGuid()).ConfigureAwait(false);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task AdminService_InsertAsync_Success()
        {
            var dto = new AdminDto
            {
                Active = true,
                AdminEmail = "sadmin@pearson.com",
                Created = DateTime.Now.AddDays(-20),
                District = new DistrictDto
                {
                    DistrictId = Guid.NewGuid(),
                    DistrictName = "Test District"
                },
                LastLoginDateTime = DateTime.Now.AddMinutes(-1),
                School = new SchoolDto
                {
                    SchoolID = Guid.NewGuid(),
                    SchoolName = "Private Test School"
                },
                User = new UserDto
                {
                    Created = DateTime.Now.AddDays(-80),
                    UserID = Guid.NewGuid(),
                    Username = "sadmintest001",
                    UserType = "School Admin"
                }
            };

            Admin admin = (Admin)dto;

            _adminRepositoryMock.Setup(x => x.InsertAsync(It.IsAny<AdminDto>())).ReturnsAsync(dto);

            var result = await _sut.InsertAsync(admin).ConfigureAwait(false);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task AdminService_InsertAsync_Failure()
        {
            var admin = new Admin
            {
                DistrictId = Guid.NewGuid(),
                Active = false,
                UserId = Guid.NewGuid(),
                Username = "Test Admin"

            };

            _adminRepositoryMock.Setup(x => x.InsertAsync(It.IsAny<AdminDto>())).ReturnsAsync(null);

            var result = await _sut.InsertAsync(admin).ConfigureAwait(false);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task AdminService_UpdateAsync_Success()
        {
            var dto = new AdminDto
            {
                Active = true,
                AdminEmail = "sadmin@pearson.com",
                Created = DateTime.Now.AddDays(-20),
                District = new DistrictDto
                {
                    DistrictId = Guid.NewGuid(),
                    DistrictName = "Test District"
                },
                LastLoginDateTime = DateTime.Now.AddMinutes(-1),
                School = new SchoolDto
                {
                    SchoolID = Guid.NewGuid(),
                    SchoolName = "Private Test School"
                },
                User = new UserDto
                {
                    Created = DateTime.Now.AddDays(-80),
                    UserID = Guid.NewGuid(),
                    Username = "sadmintest001",
                    UserType = "School Admin"
                }
            };

            Admin admin = (Admin)dto;

            _adminRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<AdminDto>())).ReturnsAsync(dto);

            var result = await _sut.UpdateAsync(admin).ConfigureAwait(false);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task AdminService_UpdateAsync_Failure()
        {
            var admin = new Admin
            {
                DistrictId = Guid.NewGuid(),
                Active = false,
                UserId = Guid.NewGuid(),
                Username = "Test Admin"

            };

            _adminRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<AdminDto>())).ReturnsAsync(null);

            var result = await _sut.UpdateAsync(admin).ConfigureAwait(false);

            Assert.IsFalse(result);
        }       
    }
}
