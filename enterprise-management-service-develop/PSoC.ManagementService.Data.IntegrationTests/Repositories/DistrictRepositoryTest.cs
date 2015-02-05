using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using PSoC.ManagementService.Data.Models;
using PSoC.ManagementService.Data.Repositories;

namespace PSoC.ManagementService.Data.IntegrationTests.Repositories
{
    [TestClass]
    public class DistrictRepositoryTest
    {
        private readonly Guid InvalidKey = Guid.Parse("000D1CC7-4B2B-4183-A624-A8C68C365E30");

        // Subject under test
        private DistrictRepository _sut;
        private DistrictDto _testDistrict;

        [TestMethod]
        public async Task DistrictRepository_DeleteAsync_InvalidUpdate()
        {
            // Act
            Boolean result = await _sut.DeleteAsync(InvalidKey).ConfigureAwait(false);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task DistrictRepository_DeleteAsync_Success()
        {
            // Act
            var result = await _sut.DeleteAsync(_testDistrict.DistrictId).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(result);

            var actual = await _sut.GetByIdAsync(_testDistrict.DistrictId).ConfigureAwait(false);
            Assert.IsNull(actual);
        }

        [TestMethod]
        public async Task DistrictRepository_GetAsync_ReturnsTestData()
        {
            // Act
            var result = await _sut.GetAsync().ConfigureAwait(false);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.FirstOrDefault(x => x.DistrictId == InitIntegrationTest.TestDistrictId));
        }

        [TestMethod]
        public async Task DistrictRepository_GetByIdAsync_InvalidId_Failure()
        {
            // Arrange
            Guid id = Guid.NewGuid();

            // Act
            DistrictDto result = await _sut.GetByIdAsync(id).ConfigureAwait(false);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task DistrictRepository_GetByIdAsync_ReturnsNullForInvalidId()
        {
            // Act
            var result = await _sut.GetByIdAsync(InvalidKey).ConfigureAwait(false);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task DistrictRepository_GetByIdAsync_ReturnsTestData()
        {
            // Act
            var result = await _sut.GetByIdAsync(_testDistrict.DistrictId).ConfigureAwait(false);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(_testDistrict.DistrictId, result.DistrictId);
        }

        [TestMethod]
        public async Task DistrictRepository_InsertAsync_Success()
        {
            // Act
            // Done in Initialize

            // Assert
            var actual = await _sut.GetByIdAsync(_testDistrict.DistrictId).ConfigureAwait(false);
            Assert.IsNotNull(actual);
            Assert.AreEqual(_testDistrict.DistrictId, actual.DistrictId);
        }

        [TestMethod]
        public async Task DistrictRepository_InsertAsync_ThrowsExceptionForDuplicateInsert()
        {
            // Act
            DistrictDto result = null;
            Exception thrownException = null;
            try
            {
                result = await _sut.InsertAsync(_testDistrict).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                thrownException = e;
            }

            // Assert
            Assert.IsNull(result);
            Assert.IsNotNull(thrownException);
        }

        [TestMethod]
        public async Task DistrictRepository_UpdateAsync_Success()
        {
            // Arrange
            const Int32 newValue = 1111;
            var newTime = TimeSpan.Parse("11:11");
            _testDistrict = new DistrictDto
            {
                DistrictId = _testDistrict.DistrictId,
                DistrictName = newValue.ToString(),
                DistrictMaxDownloadLicenses = newValue,
                DistrictInstructionHoursStart = newTime,
                DistrictInstructionHoursEnd = newTime,
                DistrictLicenseExpirySeconds = newValue,
                DistrictPreloadHoursStart = newTime,
                DistrictPreloadHoursEnd = newTime,
                DistrictOverrideCode = newValue.ToString(),
                DistrictUserPolicy = newValue,
                DistrictUseCacheServer = newValue.ToString(),
                DistrictAnnotation = newValue.ToString(),
                OAuthApplicationId = newValue.ToString(),
                OAuthClientId = newValue.ToString(),
                OAuthURL = newValue.ToString()
            };

            // Act
            var result = await _sut.UpdateAsync(_testDistrict).ConfigureAwait(false);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(_testDistrict.DistrictId, result.DistrictId);

            var actual = await _sut.GetByIdAsync(_testDistrict.DistrictId).ConfigureAwait(false);
            Assert.IsNotNull(actual);
            Assert.AreEqual(_testDistrict.DistrictId, actual.DistrictId);
            Assert.AreEqual(_testDistrict.DistrictName, actual.DistrictName);
            Assert.AreEqual(_testDistrict.DistrictMaxDownloadLicenses, actual.DistrictMaxDownloadLicenses);
            Assert.AreEqual(_testDistrict.DistrictInstructionHoursStart, actual.DistrictInstructionHoursStart);
            Assert.AreEqual(_testDistrict.DistrictInstructionHoursEnd, actual.DistrictInstructionHoursEnd);
            Assert.AreEqual(_testDistrict.DistrictLicenseExpirySeconds, actual.DistrictLicenseExpirySeconds);
            Assert.AreEqual(_testDistrict.DistrictPreloadHoursStart, actual.DistrictPreloadHoursStart);
            Assert.AreEqual(_testDistrict.DistrictPreloadHoursEnd, actual.DistrictPreloadHoursEnd);
            Assert.AreEqual(_testDistrict.DistrictOverrideCode, actual.DistrictOverrideCode);
            Assert.AreEqual(_testDistrict.DistrictUserPolicy, actual.DistrictUserPolicy);
            Assert.AreEqual(_testDistrict.DistrictUseCacheServer, actual.DistrictUseCacheServer.Trim());
            Assert.AreEqual(_testDistrict.DistrictAnnotation, actual.DistrictAnnotation);
            Assert.AreEqual(_testDistrict.OAuthApplicationId, actual.OAuthApplicationId);
            Assert.AreEqual(_testDistrict.OAuthClientId, actual.OAuthClientId);
            Assert.AreEqual(_testDistrict.OAuthURL, actual.OAuthURL);
        }

        [TestInitialize]
        public void Initialize()
        {
            SetupTestData();
        }

        /// <summary>
        /// Set up test data
        /// </summary>
        private void SetupTestData()
        {
            // Arrange
            _sut = new DistrictRepository();

            _testDistrict = new DistrictDto
            {
                DistrictId = Guid.NewGuid(),
                CreatedBy = "123",
                DistrictName = "234",
                DistrictMaxDownloadLicenses = 345,
                DistrictInstructionHoursStart = TimeSpan.Parse("1:23"),
                DistrictInstructionHoursEnd = TimeSpan.Parse("2:34"),
                DistrictLicenseExpirySeconds = 456,
                DistrictPreloadHoursStart = TimeSpan.Parse("3:45"),
                DistrictPreloadHoursEnd = TimeSpan.Parse("4:56"),
                DistrictOverrideCode = "567",
                DistrictUserPolicy = 678,
                DistrictUseCacheServer = "789",
                DistrictAnnotation = "890",
                OAuthApplicationId = "901",
                OAuthClientId = "012",
                OAuthURL = "123"
            };

            InitIntegrationTest.TestDistrictIds.Add(_testDistrict.DistrictId);
            // Act
            _sut.InsertAsync(_testDistrict).Wait();
        }
    }
}