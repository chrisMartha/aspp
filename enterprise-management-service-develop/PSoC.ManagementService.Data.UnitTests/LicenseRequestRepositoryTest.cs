using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using PSoC.ManagementService.Data.Models;
using PSoC.ManagementService.Data.Repositories;
using PSoC.ManagementService.Data.Helpers;
using PSoC.ManagementService.Data.QueryFactory;
using PSoC.ManagementService.Data.DataMapper;
using PSoC.ManagementService.Data.Interfaces;
using PSoC.ManagementService.Data.Types;
using PSoC.ManagementService.Core;

namespace PSoC.ManagementService.Data.UnitTests
{
    [TestClass]
    public class LicenseRequestRepositoryTest
    {
        private DbDataReader MockIDataReader()
        {
            var token = new System.Threading.CancellationToken(false);
            var moq = new Mock<DbDataReader>();

            int newRecordCount = 0;
            int newSetCount = 0;

            //bool hasRecord = true;
            //moq.SetupSequence(x => x.ReadAsync(token))
            //   .Returns(hasRecord)
            //   .Callback(() => newRecordCount++);

            //moq.SetupSequence(x => x.ReadAsync(token))
            //    .Returns(newRecordCount <= 1)
            //    .Callback(() => newRecordCount++);

            //moq.Setup(x => x.HasRows)
            //    .Returns(() => true);

            //moq.Setup(x => x.NextResultAsync(token))
            //    .ReturnsAsync(newSetCount >= 1)
            //    .Callback(() => newRecordCount++);

            //moq.Setup(x => x.GetGuid(0)).Returns(Guid.NewGuid());
            //moq.Setup(x => x.GetString(1)).Returns("");
            //moq.Setup(x => x.GetDateTime(2)).Returns(DateTime.UtcNow);
            //moq.Setup(x => x.GetString(3)).Returns(default(string));
            //moq.Setup(x => x.GetString(4)).Returns(default(string));
            //moq.Setup(x => x.GetInt32(5)).Returns(0);
            //moq.Setup(x => x.GetDateTime(6)).Returns(DateTime.UtcNow);
            //moq.Setup(x => x.GetDateTime(7)).Returns(DateTime.UtcNow);
            //moq.Setup(x => x.GetDateTime(8)).Returns(DBNull.Value);
            //moq.Setup(x => x.GetDateTime(9)).Returns(DBNull.Value);
            //moq.Setup(x => x.GetGuid(10)).Returns(DBNull.Value);
            //moq.Setup(x => x.GetString(11)).Returns(default(string));
            //moq.Setup(x => x.GetString(12)).Returns(default(string));
            //moq.Setup(x => x.GetString(13)).Returns(default(string));
            //moq.Setup(x => x.GetString(14)).Returns(default(string));
            //moq.Setup(x => x.GetGuid(15)).Returns(DBNull.Value);
            //moq.Setup(x => x.GetString(16)).Returns(default(string));
            //moq.Setup(x => x.GetInt64(17)).Returns(DBNull.Value);
            //moq.Setup(x => x.GetString(18)).Returns(default(string));
            //moq.Setup(x => x.GetInt16(19)).Returns(DBNull.Value);
            //moq.Setup(x => x.GetDateTime(20)).Returns(DBNull.Value);
            //moq.Setup(x => x.GetInt64(21)).Returns(DBNull.Value);
            //moq.Setup(x => x.GetDateTime(22)).Returns(DBNull.Value);
            //moq.Setup(x => x.GetString(23)).Returns(default(string));
            //moq.Setup(x => x.GetString(24)).Returns(default(string));
            //moq.Setup(x => x.GetDateTime(25)).Returns(DBNull.Value);
            //moq.Setup(x => x.GetGuid(26)).Returns(DBNull.Value);
            //moq.Setup(x => x.GetString(27)).Returns(default(string));
            //moq.Setup(x => x.GetString(28)).Returns(default(string));
            //moq.Setup(x => x.GetDateTime(29)).Returns(DBNull.Value);
            //moq.Setup(x => x.GetGuid(30)).Returns(DBNull.Value);
            //moq.Setup(x => x.GetGuid(31)).Returns(DBNull.Value);
            moq.Setup(x => x.GetInt32(32)).Returns(2);

            return moq.Object;
        }

        [TestInitialize]
        public void Initialize()
        {
           
        }


        [TestMethod]
        public void LicenseRequestQuery_GetDeleteQuery()
        {
            var query = new LicenseRequestQuery();
            var result = query.GetDeleteQuery(Guid.NewGuid());

            Assert.IsNotNull(result);
            Assert.IsTrue(!string.IsNullOrEmpty(result.QueryString));
            Assert.IsTrue(result.SqlParameters.Count > 0);
        }

        [TestMethod]
        public void LicenseRequestQuery_GetInsertQuery()
        {
            var school = new SchoolDto
            {
                District = new DistrictDto
                {
                }
            };

            var user = new UserDto
            {
            };

            var installedCourses = new List<DeviceInstalledCourseDto>() 
            { 
                new DeviceInstalledCourseDto() 
                {
                    Device =  new DeviceDto(),
                    PercentDownloaded = Convert.ToDecimal(0.75),
                    Course = new CourseDto()
                }            
            };

            var device = new DeviceDto
            {
                DeviceInstalledCourses = installedCourses
            };

            var dto = new LicenseRequestDto
            {
                LicenseRequestID = Guid.NewGuid(),
                School = school,
                ConfigCode = "test config",
                AccessPoint = new AccessPointDto { WifiBSSID = "" },
                Device = device,
                RequestDateTime = DateTime.UtcNow,
                User = user,
                LicenseRequestType = LicenseRequestType.RequestLicense
            };

            var license = new LicenseDto
            {
                AccessPoint = new AccessPointDto { WifiBSSID = "" },
                ConfigCode = "test config",
                WifiBSSID = "",
                School = school,
                LicenseRequest = new LicenseRequestDto { LicenseRequestID = dto.LicenseRequestID },
                LicenseIssueDateTime = DateTime.UtcNow,
                LicenseExpiryDateTime = DateTime.UtcNow.AddMinutes(30)
            };

            dto.License = license;

            var query = new LicenseRequestQuery();
            var result = query.GetInsertQuery(dto);

            Assert.IsNotNull(result);
            Assert.IsTrue(!string.IsNullOrEmpty(result.QueryString));
            Assert.IsTrue(result.SqlParameters.Count > 0);
        }

        [TestMethod]
        public void LicenseRequestQuery_GetUpdateQuery()
        {
            var school = new SchoolDto
            {
                District = new DistrictDto
                {
                }
            };

            var user = new UserDto
            {
            };

            var installedCourses = new List<DeviceInstalledCourseDto>() 
            { 
                new DeviceInstalledCourseDto() 
                {
                    Device =  new DeviceDto(),
                    PercentDownloaded = Convert.ToDecimal(0.75),
                    Course = new CourseDto()
                }            
            };

            var device = new DeviceDto
            {
                DeviceInstalledCourses = installedCourses
            };

            var dto = new LicenseRequestDto
            {
                LicenseRequestID = Guid.NewGuid(),
                School = school,
                ConfigCode = "test config",
                AccessPoint = new AccessPointDto { WifiBSSID = "" },
                Device = device,
                RequestDateTime = DateTime.UtcNow,
                User = user,
                LicenseRequestType = LicenseRequestType.RequestLicense
            };

            var license = new LicenseDto
            {
                AccessPoint = new AccessPointDto { WifiBSSID = "" },
                ConfigCode = "test config",
                WifiBSSID = "",
                School = school,
                LicenseRequest = new LicenseRequestDto { LicenseRequestID = dto.LicenseRequestID },
                LicenseIssueDateTime = DateTime.UtcNow,
                LicenseExpiryDateTime = DateTime.UtcNow.AddMinutes(30)
            };

            dto.License = license;

            var query = new LicenseRequestQuery();
            var result = query.GetUpdateQuery(dto);

            Assert.IsNotNull(result);
            Assert.IsTrue(!string.IsNullOrEmpty(result.QueryString));
            Assert.IsTrue(result.SqlParameters.Count > 0);
        }


        //[TestMethod]
        //public async Task LicenseRequestDataMapper_ToEntityList()
        //{
        //    var mapper = new LicenseRequestDataMapper();
        //    IList<LicenseRequestDto> results;

        //    using ( var reader = MockIDataReader() )
        //    {
        //        results = (await mapper.ToEntityList(reader));
        //    }
        //}
    }
}
