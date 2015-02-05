using System;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using PSoC.ManagementService.Data.IntegrationTests;
using PSoC.ManagementService.Data.Models;
using PSoC.ManagementService.Data.Repositories;

namespace PSoC.ManagementService.Data.IntegrationTests.Repositories
{
    [TestClass]
    public class SchoolRepositoryTest
    {
        private SchoolRepository _sut;

        [TestInitialize]
        public void Initialize()
        {
            _sut = new SchoolRepository();
        }

        [TestMethod]
        public async Task SchoolRepository_Insert()
        {
            var dto = new SchoolDto
            {
                SchoolID = Guid.NewGuid(),
                GradeInstructionHoursBegin = new TimeSpan(7, 0, 0),
                GradeInstructionHoursEnd =  new TimeSpan(16, 0, 0),
                GradePreloadHoursBegin = new TimeSpan(1, 0, 0),
                GradePreloadHoursEnd = new TimeSpan(4, 0, 0),
                SchoolAddress1 = "123 School House Road",
                SchoolLicenseExpirySeconds = 300,
                SchoolMaxDownloadLicenses = 200,
                SchoolName = "Test School",
                SchoolState = "NY",
                SchoolTown = "Smartsville",
                SchoolZipCode = "12345",
                District = new DistrictDto {  DistrictId = InitIntegrationTest.TestDistrictId}
            };

            var result = await _sut.InsertAsync(dto).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.SchoolID == dto.SchoolID);
        }

        [TestMethod]
        public async Task SchoolRepository_DeleteAsync()
        {
            var dto = new SchoolDto
            {
                SchoolID = Guid.NewGuid(),
                GradeInstructionHoursBegin = new TimeSpan(7, 0, 0),
                GradeInstructionHoursEnd = new TimeSpan(16, 0, 0),
                GradePreloadHoursBegin = new TimeSpan(1, 0, 0),
                GradePreloadHoursEnd = new TimeSpan(4, 0, 0),
                SchoolAddress1 = "123 School House Road",
                SchoolLicenseExpirySeconds = 300,
                SchoolMaxDownloadLicenses = 200,
                SchoolName = "Test Delete School",
                SchoolState = "NY",
                SchoolTown = "Smartsville",
                SchoolZipCode = "12345",
                District = new DistrictDto { DistrictId = InitIntegrationTest.TestDistrictId }
            };

            var school = await _sut.InsertAsync(dto).ConfigureAwait(false);

            var result = await _sut.DeleteAsync(school.SchoolID).ConfigureAwait(false);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task SchoolRepository_GetAsync()
        {
            var result = await _sut.GetAsync().ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod]
        public async Task SchoolRepository_GetByDistrictIdAsync()
        {
            var result = await _sut.GetByDistrictIdAsync(InitIntegrationTest.TestDistrictId).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod]
        public async Task SchoolRepository_GetByIdAsync()
        {
            var result = await _sut.GetByIdAsync(InitIntegrationTest.TestSchoolId).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.AreEqual(InitIntegrationTest.TestSchoolId, result.SchoolID);
        }

        [TestMethod]
        public async Task SchoolRepository_UpdateAsync()
        {
            var dto = new SchoolDto
            {
                SchoolID = Guid.NewGuid(),
                GradeInstructionHoursBegin = new TimeSpan(7, 0, 0),
                GradeInstructionHoursEnd = new TimeSpan(16, 0, 0),
                GradePreloadHoursBegin = new TimeSpan(1, 0, 0),
                GradePreloadHoursEnd = new TimeSpan(4, 0, 0),
                SchoolAddress1 = "123 School House Road",
                SchoolLicenseExpirySeconds = 300,
                SchoolMaxDownloadLicenses = 200,
                SchoolName = "Test Update School",
                SchoolState = "NY",
                SchoolTown = "Smartsville",
                SchoolZipCode = "12345",
                District = new DistrictDto { DistrictId = InitIntegrationTest.TestDistrictId }
            };

            var school = await _sut.InsertAsync(dto).ConfigureAwait(false);

            school.GradeInstructionHoursBegin = new TimeSpan(9, 8, 7);
            school.GradeInstructionHoursEnd = new TimeSpan(6, 5, 4);
            school.GradePreloadHoursBegin = new TimeSpan(3, 2, 1);
            school.GradePreloadHoursEnd = new TimeSpan(10, 11, 12);
            school.SchoolAddress1 = "321 Blue House Road";
            school.SchoolLicenseExpirySeconds = 200;
            school.SchoolMaxDownloadLicenses = 100;
            school.SchoolName = "Test Update School";
            school.SchoolState = "NJ";
            school.SchoolTown = "Smeedville";
            school.SchoolZipCode = "54321";

            var result = await _sut.UpdateAsync(school).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.AreEqual(school.SchoolID, result.SchoolID);
            Assert.AreEqual(school.GradeInstructionHoursBegin, result.GradeInstructionHoursBegin);
            Assert.AreEqual(school.GradeInstructionHoursEnd, result.GradeInstructionHoursEnd);
            Assert.AreEqual(school.GradePreloadHoursBegin, result.GradePreloadHoursBegin);
            Assert.AreEqual(school.GradePreloadHoursEnd, result.GradePreloadHoursEnd);
            Assert.AreEqual(school.SchoolAddress1, result.SchoolAddress1);
            Assert.AreEqual(school.SchoolLicenseExpirySeconds, result.SchoolLicenseExpirySeconds);
            Assert.AreEqual(school.SchoolMaxDownloadLicenses, result.SchoolMaxDownloadLicenses);
            Assert.AreEqual(school.SchoolName, result.SchoolName);
            Assert.AreEqual(school.SchoolState, result.SchoolState);
            Assert.AreEqual(school.SchoolTown, result.SchoolTown);
            Assert.AreEqual(school.SchoolZipCode, result.SchoolZipCode);
        }
    }
}