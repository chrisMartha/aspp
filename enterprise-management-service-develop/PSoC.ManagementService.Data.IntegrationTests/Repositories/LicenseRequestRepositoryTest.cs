using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using PSoC.ManagementService.Core;
using PSoC.ManagementService.Data.Models;
using PSoC.ManagementService.Data.Repositories;

namespace PSoC.ManagementService.Data.IntegrationTests.Repositories
{
    [TestClass]
    public class LicenseRequestRepositoryTest
    {
        private LicenseRequestRepository _sut;

        [TestInitialize]
        public void Initialize()
        {
            _sut = new LicenseRequestRepository();
        }

        [TestMethod]
        public async Task LicenseRequestRepository_Insert()
        {
            var school = new SchoolDto
                    {
                        SchoolID = InitIntegrationTest.TestSchoolId,
                        District = new DistrictDto
                        {
                            DistrictId = InitIntegrationTest.TestDistrictId
                        }
                    };

            var user = new UserDto
            {
                UserID = InitIntegrationTest.TestTeacherId,
            };

            var installedCourses = new List<DeviceInstalledCourseDto>()
            {
                new DeviceInstalledCourseDto()
                {
                    Device =  new DeviceDto{ DeviceID = InitIntegrationTest.TestDeviceId},
                    PercentDownloaded = Convert.ToDecimal(0.75),
                    Course = new CourseDto
                    {
                         CourseLearningResourceID = InitIntegrationTest.TestCourseLearningResourceId
                    }
                }
            };

            var device = new DeviceDto
            {
                DeviceID = InitIntegrationTest.TestDeviceId,
                DeviceInstalledCourses = installedCourses
            };

            var dto = new LicenseRequestDto
            {
                LicenseRequestID = Guid.NewGuid(),
                School = school,
                ConfigCode = "test config",
                AccessPoint = new AccessPointDto { WifiBSSID = InitIntegrationTest.TestWifiBSSID },
                Device = device,
                RequestDateTime = DateTime.UtcNow,
                User = user,
                LicenseRequestType = LicenseRequestType.RequestLicense
            };

            var license = new LicenseDto
            {
                AccessPoint = new AccessPointDto { WifiBSSID = InitIntegrationTest.TestWifiBSSID },
                ConfigCode = "test config",
                WifiBSSID = InitIntegrationTest.TestWifiBSSID,
                School = school,
                LicenseRequest = new LicenseRequestDto { LicenseRequestID = dto.LicenseRequestID},
                LicenseIssueDateTime = DateTime.UtcNow,
                LicenseExpiryDateTime = DateTime.UtcNow.AddMinutes(30)
            };

            dto.License = license;

            InitIntegrationTest.TestLicenseRequestIds.Add(dto.LicenseRequestID);

            var result = await _sut.InsertAsync(dto).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.LicenseRequestID == dto.LicenseRequestID);
        }

        [TestMethod]
        public async Task LicenseRequestRepository_Select()
        {
            var result = await _sut.GetAsync().ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
            Assert.IsTrue(result.Any(r => r.LicenseRequestID == InitIntegrationTest.TestLicenseRequestId));
        }
    }
}