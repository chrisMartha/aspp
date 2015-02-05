using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PSoC.ManagementService.Data.Helpers;
using PSoC.ManagementService.Data.IntegrationTests.Helpers;
using PSoC.ManagementService.Data.Models;
using PSoC.ManagementService.Data.Repositories;

namespace PSoC.ManagementService.Data.IntegrationTests.Repositories
{
    [TestClass]
    public class AccessPointRepositoryTest
    {
        private readonly String InvalidKey = "000D1CC7-4B2B-4183-A624-A8C68C365E30".Substring(0, 17);

        private AccessPointRepository _sut;
        private AccessPointDto _testAccessPoint;
        private ClassroomDto _testClassRoom;
        private DistrictDto _testDistrict;
        private SchoolDto _testSchool;

        [TestMethod]
        public async Task AccessPointRepository_DeleteAsync_InvalidUpdate()
        {
            // Act
            Boolean result = await _sut.DeleteAsync(InvalidKey).ConfigureAwait(false);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task AccessPointRepository_DeleteAsync_Success()
        {
            // Act
            var result = await _sut.DeleteAsync(_testAccessPoint.WifiBSSID).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(result);

            var actual = await _sut.GetByIdAsync(_testAccessPoint.WifiBSSID).ConfigureAwait(false);
            Assert.IsNull(actual);
        }

        [TestMethod]
        public async Task AccessPointRepository_DeleteManyAsync_ThrowsNotImplementedException()
        {
            Exception thrownException = null;
            try
            {
                var result = await _sut.DeleteAsync(new string[] { }).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                thrownException = e;
            }

            // Assert
            Assert.IsNotNull(thrownException);
            Assert.IsInstanceOfType(thrownException, typeof(NotImplementedException));
        }

        [TestMethod]
        public async Task AccessPointRepository_GetAsync_ReturnsTestData()
        {
            // Act
            var result = await _sut.GetAsync().ConfigureAwait(false);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.FirstOrDefault(x => x.WifiBSSID == _testAccessPoint.WifiBSSID));
        }

        [TestMethod]
        public async Task AccessPointRepository_GetByDistrictAsync_ReturnsTestData()
        {
            // Act
            var result = await _sut.GetByDistrictAsync(_testDistrict.DistrictId).ConfigureAwait(false);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.FirstOrDefault(x => x.WifiBSSID == _testAccessPoint.WifiBSSID));
        }

        [TestMethod]
        public async Task AccessPointRepository_GetByIdAsync_ReturnsNullForInvalidId()
        {
            // Act
            var result = await _sut.GetByIdAsync(InvalidKey).ConfigureAwait(false);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task AccessPointRepository_GetByIdAsync_ReturnsTestData()
        {
            // Act
            var result = await _sut.GetByIdAsync(_testAccessPoint.WifiBSSID).ConfigureAwait(false);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(_testAccessPoint.WifiBSSID, result.WifiBSSID);
        }

        [TestMethod]
        public async Task AccessPointRepository_GetBySchoolAsync_ReturnsTestData()
        {
            // Act
            var result = await _sut.GetBySchoolAsync(_testSchool.SchoolID).ConfigureAwait(false);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.FirstOrDefault(x => x.WifiBSSID == _testAccessPoint.WifiBSSID));
        }

        [TestMethod]
        public async Task AccessPointRepository_InsertAsync_Success()
        {
            // Act
            // Done in Initialize

            // Assert
            var actual = await _sut.GetByIdAsync(_testAccessPoint.WifiBSSID).ConfigureAwait(false);
            Assert.IsNotNull(actual);
            Assert.AreEqual(_testAccessPoint.WifiBSSID, actual.WifiBSSID);
        }

        [TestMethod]
        public async Task AccessPointRepository_InsertAsync_ThrowsExceptionForDuplicateInsert()
        {
            // Act
            AccessPointDto result = null;
            Exception thrownException = null;
            try
            {
                result = await _sut.InsertAsync(_testAccessPoint).ConfigureAwait(false);
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
        public async Task AccessPointRepository_UpdateAsync_Success()
        {
            // Arrange
            const Int32 newValue = 999;
            _testAccessPoint = new AccessPointDto
            {
                WifiBSSID = _testAccessPoint.WifiBSSID,
                WifiSSID = newValue.ToString(),
                District = new DistrictDto { DistrictId = _testDistrict.DistrictId },
                School = new SchoolDto { SchoolID = _testSchool.SchoolID },
                Classroom = new ClassroomDto { ClassroomID = _testClassRoom.ClassroomID },
                AccessPointMaxDownloadLicenses = newValue,
                AccessPointExpiryTimeSeconds = newValue,
                AccessPointAnnotation = newValue.ToString(),
                AccessPointModel = newValue.ToString()
            };

            // Act
            var result = await _sut.UpdateAsync(_testAccessPoint).ConfigureAwait(false);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(_testAccessPoint.WifiBSSID, result.WifiBSSID);

            var actual = await _sut.GetByIdAsync(_testAccessPoint.WifiBSSID).ConfigureAwait(false);
            Assert.IsNotNull(actual);
            Assert.AreEqual(_testAccessPoint.WifiBSSID, actual.WifiBSSID);
            Assert.AreEqual(_testAccessPoint.WifiSSID, actual.WifiSSID);
            Assert.AreEqual(_testAccessPoint.AccessPointMaxDownloadLicenses, actual.AccessPointMaxDownloadLicenses);
            Assert.AreEqual(_testAccessPoint.AccessPointExpiryTimeSeconds, actual.AccessPointExpiryTimeSeconds);
            Assert.AreEqual(_testAccessPoint.AccessPointAnnotation, actual.AccessPointAnnotation);
            Assert.AreEqual(_testAccessPoint.AccessPointModel, actual.AccessPointModel);
        }

        [TestCleanup]
        public void Cleanup()
        {
            ClearTestData();
        }

        [TestInitialize]
        public void Initialize()
        {
            SetupTestData();
            _sut = new AccessPointRepository();
        }

        /// <summary>
        /// Clear test data
        /// </summary>
        private void ClearTestData()
        {
            // Delete test data
            _sut.DeleteAsync(_testAccessPoint.WifiBSSID).Wait();
            DataAccessHelper.ExecuteAsync(String.Format("DELETE FROM dbo.Classroom WHERE ClassroomID = '{0}'", _testClassRoom.ClassroomID)).Wait(); // TODO: Replace with repo
            DataAccessHelper.ExecuteAsync(String.Format("DELETE FROM dbo.School WHERE SchoolID = '{0}'", _testSchool.SchoolID)).Wait(); // TODO: Replace with repo
            (new DistrictRepository()).DeleteAsync(_testDistrict.DistrictId).Wait();
        }

        /// <summary>
        /// Set up test data
        /// </summary>
        private void SetupTestData()
        {
            // Arrange
            _sut = new AccessPointRepository();

            _testDistrict = new DistrictDto
            {
                DistrictId = Guid.NewGuid(),
                DistrictName = "234",
                DistrictMaxDownloadLicenses = 345,
                DistrictLicenseExpirySeconds = 456,
                OAuthApplicationId = "901",
                OAuthClientId = "012",
                OAuthURL = "123"
            };

            _testSchool = new SchoolDto
            {
                SchoolID = Guid.NewGuid()
            };

            _testClassRoom = new ClassroomDto
            {
                ClassroomID = Guid.NewGuid(),
                ClassroomName = "123",
                School = new SchoolDto { SchoolID = _testSchool.SchoolID, District = new DistrictDto { DistrictId = _testDistrict.DistrictId }, }
            };

            string macAdress = DataGenerator.GetRandomMacAddress();
            while (InitIntegrationTest.TestAccessPointIds.Contains(macAdress))
            {
                macAdress = DataGenerator.GetRandomMacAddress();
            }

            _testAccessPoint = new AccessPointDto
            {
                WifiBSSID = macAdress,
                WifiSSID = "123",
                District = new DistrictDto { DistrictId = _testDistrict.DistrictId },
                School = new SchoolDto { SchoolID = _testSchool.SchoolID },
                Classroom = new ClassroomDto { ClassroomID = _testClassRoom.ClassroomID },
                AccessPointMaxDownloadLicenses = 234,
                AccessPointExpiryTimeSeconds = 345,
                AccessPointAnnotation = "456",
                AccessPointModel = "567"
            };

            InitIntegrationTest.TestAccessPointIds.Add(_testAccessPoint.WifiBSSID);
            InitIntegrationTest.TestClassroomIds.Add(_testClassRoom.ClassroomID);
            InitIntegrationTest.TestDistrictIds.Add(_testDistrict.DistrictId);

            // Act
            (new DistrictRepository()).InsertAsync(_testDistrict).Wait();
            DataAccessHelper.ExecuteAsync(String.Format("INSERT INTO dbo.School (SchoolID, DistrictID) VALUES ('{0}','{1}')", _testSchool.SchoolID, _testDistrict.DistrictId)).Wait(); // TODO: Replace with repo
            DataAccessHelper.ExecuteAsync(String.Format("INSERT INTO dbo.Classroom (ClassroomID, ClassroomName, DistrictID, SchoolID) VALUES ('{0}', '{1}', '{2}', '{3}')", _testClassRoom.ClassroomID, _testClassRoom.ClassroomName, _testClassRoom.School.District.DistrictId, _testClassRoom.School.SchoolID)).Wait(); // TODO: Replace with repo
            _sut.InsertAsync(_testAccessPoint).Wait();
        }
    }
}