using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PSoC.ManagementService.Data.Models;
using PSoC.ManagementService.Data.Repositories;

namespace PSoC.ManagementService.Data.IntegrationTests.Repositories
{
    [TestClass]
    public class AdminRepositoryTest
    {
        private Guid _testUserId;
        private Guid _testUserId2;
        private Guid[] _testUserIds;
        private UserDto _testUser;
        private AdminDto _testAdmin;
        private AdminRepository _sut;   // Subject under test

        [TestInitialize]
        public void Initialize()
        {
            _testUserId = Guid.NewGuid();
            _testUserId2 = Guid.NewGuid();
            _testUserIds = new[] { _testUserId, _testUserId2 };
            _testUser = new UserDto { UserID = _testUserId };
            _testAdmin = new AdminDto
            {
                User = _testUser,
                Active = false
            };
            _sut = new AdminRepository();
        }

        #region GetAsync unit test
        [TestMethod]
        public async Task AdminRepository_GetAsync()
        {
            // Arrange
            await _sut.InsertAsync(_testAdmin).ConfigureAwait(false);

            // Act
            IList<AdminDto> results = await _sut.GetAsync().ConfigureAwait(false);

            // Assert
            Assert.IsNotNull(results);
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.IsTrue(results.Any(admin => admin.User.UserID == _testUserId));

            // Clean up
            bool cleanupSuccessful = await _sut.DeleteAsync(_testUserId).ConfigureAwait(false);
            Assert.IsTrue(cleanupSuccessful);
        }
        #endregion

        #region GetByIdAsync unit test
        [TestMethod]
        public async Task AdminRepository_GetByIdAsync_ValidId_Success()
        {
            // Arrange
            await _sut.InsertAsync(_testAdmin).ConfigureAwait(false);

            // Act
            AdminDto result = await _sut.GetByIdAsync(_testUserId).ConfigureAwait(false);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result.User.UserID, _testUserId);

            // Clean up
            bool cleanupSuccessful = await _sut.DeleteAsync(_testUserId).ConfigureAwait(false);
            Assert.IsTrue(cleanupSuccessful);
        }

        [TestMethod]
        public async Task AdminRepository_GetByIdAsync_InvalidId_Failure()
        {
            // Arrange

            // Act
            AdminDto result = await _sut.GetByIdAsync(_testUserId).ConfigureAwait(false);

            // Assert
            Assert.IsNull(result);
        }
        #endregion

        #region InsertAsync unit test
        [TestMethod]
        public async Task AdminRepository_InsertAsync_ValidData_Success()
        {
            // Arrange

            // Act
            await _sut.InsertAsync(_testAdmin).ConfigureAwait(false);

            // Assert
            AdminDto result = await _sut.GetByIdAsync(_testUserId).ConfigureAwait(false);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.User.UserID, _testUserId);
            Assert.AreEqual(result.Active, _testAdmin.Active);

            // Clean up
            bool cleanupSuccessful = await _sut.DeleteAsync(_testUserId).ConfigureAwait(false);
            Assert.IsTrue(cleanupSuccessful);
        }

        [TestMethod]
        public async Task AdminRepository_InsertAsync_DuplicateUserId_Failure()
        {
            // Arrange
            await _sut.InsertAsync(_testAdmin).ConfigureAwait(false);

            // Act
            AdminDto result = null;
            Exception thrownException = null;
            try
            {
                result = await _sut.InsertAsync(_testAdmin).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                thrownException = e;
            }

            // Assert
            Assert.IsNull(result);
            Assert.IsNotNull(thrownException);
            Assert.IsInstanceOfType(thrownException, typeof(SqlException));

            // Clean up
            bool cleanupSuccessful = await _sut.DeleteAsync(_testUserId).ConfigureAwait(false);
            Assert.IsTrue(cleanupSuccessful);
        }

        [TestMethod]
        public async Task AdminRepository_InsertAsync_InvalidDistrictId_Failure()
        {
            // Arrange
            _testAdmin.District = new DistrictDto { DistrictId = Guid.NewGuid() };

            // Act
            AdminDto result = null;
            Exception thrownException = null;
            try
            {
                result = await _sut.InsertAsync(_testAdmin).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                thrownException = e;
            }

            // Assert
            Assert.IsNull(result);
            Assert.IsNotNull(thrownException);
            Assert.IsInstanceOfType(thrownException, typeof(SqlException));
        }

        [TestMethod]
        public async Task AdminRepository_InsertAsync_InvalidSchoolId_Failure()
        {
            // Arrange
            _testAdmin.School = new SchoolDto { SchoolID = Guid.NewGuid() };

            // Act
            AdminDto result = null;
            Exception thrownException = null;
            try
            {
                result = await _sut.InsertAsync(_testAdmin).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                thrownException = e;
            }

            // Assert
            Assert.IsNull(result);
            Assert.IsNotNull(thrownException);
            Assert.IsInstanceOfType(thrownException, typeof(SqlException));
        }
        #endregion

        #region UpdateAsync unit test
        [TestMethod]
        public async Task AdminRepository_UpdateAsync_ValidData_Success()
        {
            // Arrange
            await _sut.InsertAsync(_testAdmin).ConfigureAwait(false);
            _testAdmin.User.Username = "Test Username";
            _testAdmin.User.UserType = "Test UserType";

            // Act
            await _sut.UpdateAsync(_testAdmin).ConfigureAwait(false);

            // Assert
            AdminDto result = await _sut.GetByIdAsync(_testUserId).ConfigureAwait(false);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.User.UserID, _testUserId);
            Assert.AreEqual(result.Active, _testAdmin.Active);
            Assert.AreEqual(result.User.Username, _testAdmin.User.Username);
            Assert.AreEqual(result.User.UserType, _testAdmin.User.UserType);

            // Clean up
            bool cleanupSuccessful = await _sut.DeleteAsync(_testUserId).ConfigureAwait(false);
            Assert.IsTrue(cleanupSuccessful);
        }

        [TestMethod]
        public async Task AdminRepository_UpdateAsync_InvalidUserId_Failure()
        {
            // Arrange
            await _sut.InsertAsync(_testAdmin).ConfigureAwait(false);
            _testAdmin.User.UserID = Guid.NewGuid();

            // Act
            AdminDto result = null;
            Exception thrownException = null;
            try
            {
                result = await _sut.UpdateAsync(_testAdmin).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                thrownException = e;
            }

            // Assert
            Assert.IsNull(result);
            Assert.IsNull(thrownException);

            // Clean up
            bool cleanupSuccessful = await _sut.DeleteAsync(_testUserId).ConfigureAwait(false);
            Assert.IsTrue(cleanupSuccessful);
        }

        [TestMethod]
        public async Task AdminRepository_UpdateAsync_InvalidDistrictId_Failure()
        {
            // Arrange
            await _sut.InsertAsync(_testAdmin).ConfigureAwait(false);
            _testAdmin.District = new DistrictDto { DistrictId = Guid.NewGuid() };

            // Act
            AdminDto result = null;
            Exception thrownException = null;
            try
            {
                result = await _sut.UpdateAsync(_testAdmin).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                thrownException = e;
            }

            // Assert
            Assert.IsNull(result);
            Assert.IsNotNull(thrownException);
            Assert.IsInstanceOfType(thrownException, typeof(SqlException));

            // Clean up
            bool cleanupSuccessful = await _sut.DeleteAsync(_testUserId).ConfigureAwait(false);
            Assert.IsTrue(cleanupSuccessful);
        }

        [TestMethod]
        public async Task AdminRepository_UpdateAsync_InvalidSchoolId_Failure()
        {
            // Arrange
            await _sut.InsertAsync(_testAdmin).ConfigureAwait(false);
            _testAdmin.School = new SchoolDto { SchoolID = Guid.NewGuid() };

            // Act
            AdminDto result = null;
            Exception thrownException = null;
            try
            {
                result = await _sut.UpdateAsync(_testAdmin).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                thrownException = e;
            }

            // Assert
            Assert.IsNull(result);
            Assert.IsNotNull(thrownException);
            Assert.IsInstanceOfType(thrownException, typeof(SqlException));

            // Clean up
            bool cleanupSuccessful = await _sut.DeleteAsync(_testUserId).ConfigureAwait(false);
            Assert.IsTrue(cleanupSuccessful);
        }
        #endregion

        #region DeleteAsync unit tests
        [TestMethod]
        public async Task AdminRepository_DeleteAsync_ValidId_Success()
        {
            // Arrange
            await _sut.InsertAsync(_testAdmin).ConfigureAwait(false);

            // Act
            bool result = await _sut.DeleteAsync(_testUserId).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(result);
            AdminDto deleted = await _sut.GetByIdAsync(_testUserId);
            Assert.IsNull(deleted);
        }

        [TestMethod]
        public async Task AdminRepository_DeleteAsync_ValidIds_Success()
        {
            // Arrange
            await _sut.InsertAsync(_testAdmin).ConfigureAwait(false);
            var testUser2 = new UserDto { UserID = _testUserId2 };
            var testAdmin2 = new AdminDto { User = testUser2 };
            await _sut.InsertAsync(testAdmin2).ConfigureAwait(false);

            // Act
            bool result = await _sut.DeleteAsync(_testUserIds).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(result);
            AdminDto deleted = await _sut.GetByIdAsync(_testUserId);
            Assert.IsNull(deleted);
            AdminDto deleted2 = await _sut.GetByIdAsync(_testUserId2);
            Assert.IsNull(deleted2);
        }

        [TestMethod]
        public async Task AdminRepository_DeleteAsync_InvalidId_Failure()
        {
            // Arrange

            // Act
            bool result = await _sut.DeleteAsync(_testUserId).ConfigureAwait(false);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task AdminRepository_DeleteAsync_InvalidIds_Failure()
        {
            // Arrange

            // Act
            bool result = await _sut.DeleteAsync(_testUserIds).ConfigureAwait(false);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task AdminRepository_DeleteAsync_MixedIds_Success()
        {
            // Arrange
            await _sut.InsertAsync(_testAdmin).ConfigureAwait(false);

            // Act
            bool result = await _sut.DeleteAsync(_testUserIds).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(result);
            AdminDto deleted = await _sut.GetByIdAsync(_testUserId);
            Assert.IsNull(deleted);
        }
        #endregion

        #region GetByUsernameAsync unit tests

        [TestMethod]
        public async Task AdminRepository_GetByUsernameAsync_NonExistentUsername_ReturnsNull()
        {
            // Arrange
            const string username = "NonExistentUsername";

            // Act
            AdminDto result = await _sut.GetByUsernameAsync(username); ;

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task AdminRepository_GetByUsernameAsync_ExistingUsername_ReturnsAdmin()
        {
            // Arrange
            const string username = "bthompson27"; // TODO: Consider changing this to inject data instead of using sample data

            // Act
            AdminDto result = await _sut.GetByUsernameAsync(username); ;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result.User.Username, username);
        }
        #endregion

        #region UpdateLastLoginDateTime unit tests
        [TestMethod]
        public async Task AdminRepository_UpdateLastLoginDateTime_NonExistentUsername_ThrowsDataException()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            DateTime loginDateTime = DateTime.UtcNow;

            // Act
            Exception thrownException = null;
            try
            {
                await _sut.UpdateLastLoginDateTimeAsync(userId, loginDateTime);
            }
            catch (Exception e)
            {
                thrownException = e;
            }

            // Assert
            Assert.IsNotNull(thrownException);
            Assert.IsInstanceOfType(thrownException, typeof(DataException));
        }

        [TestMethod]
        public async Task AdminRepository_UpdateLastLoginDateTime_ExistingUsername_Success()
        {
            // Arrange
            var userId = new Guid("7c25630e-ffe3-4d84-b42b-17c06a7109cb"); // TODO: Consider changing this to inject data instead of using sample data
            DateTime loginDateTime = DateTime.UtcNow;

            // Act
            await _sut.UpdateLastLoginDateTimeAsync(userId, loginDateTime); ;

            // Nothing to assert :)
        }
        #endregion
    }
}