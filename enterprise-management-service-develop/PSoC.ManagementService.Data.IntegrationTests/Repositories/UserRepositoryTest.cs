using System;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using PSoC.ManagementService.Data.Models;
using PSoC.ManagementService.Data.Repositories;

namespace PSoC.ManagementService.Data.IntegrationTests.Repositories
{
    [TestClass]
    public class UserRepositoryTest
    {
        private readonly Guid _invalidUserId = Guid.Parse("2119C16C-11A2-48E6-A5A3-C6F671943A52");

        private UserRepository _sut;
        private UserDto _testUser;

        /// <summary>
        /// Clear Test Data
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            _sut.DeleteAsync(_testUser.UserID).ConfigureAwait(false);
        }

        /// <summary>
        /// Setup Test Data
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            // Arrange
            _sut = new UserRepository();
            _testUser = new UserDto()
            {
                UserID = Guid.NewGuid(),
                UserType = "student",
                Username = "abc"
            };
            InitIntegrationTest.TestDistrictIds.Add(_testUser.UserID);
            _sut.InsertAsync(_testUser).Wait();
        }

        [TestMethod]
        public async Task UserRepository_DeleteAsync_InvalidUpdate()
        {
            // Act
            Boolean result = await _sut.DeleteAsync(_invalidUserId).ConfigureAwait(false);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task UserRepository_DeleteAsync_Success()
        {
            // Act
            var result = await _sut.DeleteAsync(_testUser.UserID).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(result);

            var actual = await _sut.GetByIdAsync(_testUser.UserID).ConfigureAwait(false);
            Assert.IsNull(actual);
        }

        [TestMethod]
        public async Task UserRepository_GetByIdAsync_Failed()
        {
            var result = await _sut.GetByIdAsync(Guid.NewGuid()).ConfigureAwait(false);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task UserRepository_GetByIdAsync_InvalidId_Failure()
        {
            // Arrange
            Guid id = Guid.NewGuid();

            // Act
            UserDto result = await _sut.GetByIdAsync(id).ConfigureAwait(false);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task UserRepository_GetByIdAsync_ReturnsNullForInvalidId()
        {
            // Act
            var result = await _sut.GetByIdAsync(_invalidUserId).ConfigureAwait(false);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task UserRepository_GetByIdAsync_ReturnsTestData()
        {
            // Act
            var result = await _sut.GetByIdAsync(_testUser.UserID).ConfigureAwait(false);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(_testUser.UserID, result.UserID);
        }

        [TestMethod]
        public async Task UserRepository_InsertAsync()
        {
            var dto = new UserDto
            {
                UserID = Guid.NewGuid(),
                Username = "testUser000001",
                UserType = "Teacher",
            };

            InitIntegrationTest.TestUserIds.Add(dto.UserID);

            var result = await _sut.InsertAsync(dto);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.UserID == dto.UserID);
            Assert.IsTrue(result.Username == dto.Username);
            Assert.IsTrue(result.UserType == dto.UserType);
        }

        [TestMethod]
        public async Task UserRepository_InsertAsync_Success()
        {
            // Act
            // Done in Initialize

            // Assert
            var actual = await _sut.GetByIdAsync(_testUser.UserID).ConfigureAwait(false);
            Assert.IsNotNull(actual);
            Assert.AreEqual(_testUser.UserID, actual.UserID);
            Assert.AreEqual(_testUser.Username, actual.Username);
            Assert.AreEqual(_testUser.UserType, actual.UserType);
        }

        [TestMethod]
        public async Task UserRepository_InsertAsync_ThrowsExceptionForDuplicateInsert()
        {
            // Act
            UserDto result = null;
            Exception thrownException = null;
            try
            {
                result = await _sut.InsertAsync(_testUser).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                thrownException = e;
            }

            // Assert
            Assert.IsNull(result);
            Assert.IsNotNull(thrownException);
        }
    }
}