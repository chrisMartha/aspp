using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks; 

using Microsoft.VisualStudio.TestTools.UnitTesting;

using PSoC.ManagementService.Core;
using PSoC.ManagementService.Data.Helpers;
using PSoC.ManagementService.Data.IntegrationTests.Helpers;
using PSoC.ManagementService.Data.Models;
using PSoC.ManagementService.Data.Repositories;
using PSoC.ManagementService.Security;


namespace PSoC.ManagementService.Data.IntegrationTests.Repositories
{
    [TestClass]
    public class LicenseRepositoryTest
    {
        private const string WifiSsId = "lr-epic-pems";
        private const string ConfigCode = "config";
        private const int RequestLicense = 1;
        private const string Username = "LicenseRepoTest";
        private const string UserType = "Student";

        private readonly EncrypedField<string> _usernameEnc = Username;
        private readonly EncrypedField<string> _userTypeEnc = UserType;

        private readonly DataGenerator _dataGenerator = new DataGenerator();
        
        private LicenseRepository _sut;

        [TestInitialize]
        public void Initialize()
        {
            _sut = new LicenseRepository();
        }

        [TestMethod]
        public async Task LicenseRepository_DeleteAsync()
        {
            // Arrange
            AccessPointDto accessPoint = await _dataGenerator.CreateNewAccessPointAsync();
            DeviceDto device = await _dataGenerator.CreateNewDeviceAsync();
            UserDto user = await _dataGenerator.CreateNewUserAsync();
            LicenseRequestDto licenseRequest = await _dataGenerator.CreateNewLicenseRequest(accessPoint, device, user);
            LicenseDto license = await _dataGenerator.CreateNewLicense(licenseRequest);

            InitIntegrationTest.TestLicenseRequestIds.Add(licenseRequest.LicenseRequestID);
            InitIntegrationTest.TestDeviceIds.Add(device.DeviceID);
            InitIntegrationTest.TestUserIds.Add(user.UserID);
            InitIntegrationTest.TestAccessPointIds.Add(accessPoint.WifiBSSID);

            var positiveResult = await _sut.GetByIdAsync(licenseRequest.LicenseRequestID).ConfigureAwait(false);
            Assert.IsNotNull(positiveResult);

            // Act & Assert
            var result1 = await _sut.DeleteAsync(license.LicenseRequest.LicenseRequestID).ConfigureAwait(false);
            Assert.IsTrue(result1);

            var nullResult = await _sut.GetByIdAsync(license.LicenseRequest.LicenseRequestID).ConfigureAwait(false);
            Assert.IsNull(nullResult);

            var result2 = await _sut.DeleteAsync(license.LicenseRequest.LicenseRequestID).ConfigureAwait(false);
            Assert.IsFalse(result2);

            // Cleanup
            await _dataGenerator.DeleteLicenseRequestAsync(licenseRequest);
            await _dataGenerator.DeleteUserAsync(user);
            await _dataGenerator.DeleteDeviceAsync(device);
            await _dataGenerator.DeleteAccessPointAsync(accessPoint);
        }

        [TestMethod]
        public async Task LicenseRepository_DeleteManyAsync()
        {
            // Arrange
            AccessPointDto accessPoint = await _dataGenerator.CreateNewAccessPointAsync();
            DeviceDto device = await _dataGenerator.CreateNewDeviceAsync();
            UserDto user = await _dataGenerator.CreateNewUserAsync();
            LicenseRequestDto licenseRequest1 = await _dataGenerator.CreateNewLicenseRequest(accessPoint, device, user);
            LicenseDto license1 = await _dataGenerator.CreateNewLicense(licenseRequest1);
            LicenseRequestDto licenseRequest2 = await _dataGenerator.CreateNewLicenseRequest(accessPoint, device, user);
            LicenseDto license2 = await _dataGenerator.CreateNewLicense(licenseRequest2);

            InitIntegrationTest.TestLicenseRequestIds.Add(licenseRequest1.LicenseRequestID);
            InitIntegrationTest.TestLicenseRequestIds.Add(licenseRequest2.LicenseRequestID);
            InitIntegrationTest.TestDeviceIds.Add(device.DeviceID);
            InitIntegrationTest.TestUserIds.Add(user.UserID);
            InitIntegrationTest.TestAccessPointIds.Add(accessPoint.WifiBSSID);

            var positiveResult1 = await _sut.GetByIdAsync(license1.LicenseRequest.LicenseRequestID).ConfigureAwait(false);
            Assert.IsNotNull(positiveResult1);

            var positiveResult2 = await _sut.GetByIdAsync(license2.LicenseRequest.LicenseRequestID).ConfigureAwait(false);
            Assert.IsNotNull(positiveResult2);

            Guid[] testLicenseRequestIds = { license1.LicenseRequest.LicenseRequestID, license2.LicenseRequest.LicenseRequestID };

            // Act 1
            var result1 = await _sut.DeleteAsync(testLicenseRequestIds).ConfigureAwait(false);

            // Assert 1
            Assert.IsTrue(result1);

            var nullResult1 = await _sut.GetByIdAsync(license1.LicenseRequest.LicenseRequestID);
            Assert.IsNull(nullResult1);

            var nullResult2 = await _sut.GetByIdAsync(license2.LicenseRequest.LicenseRequestID);
            Assert.IsNull(nullResult2);

            // Act 2
            var result2 = await _sut.DeleteAsync(testLicenseRequestIds).ConfigureAwait(false);

            // Assert 2
            Assert.IsFalse(result2);
            
            // Cleanup
            await _dataGenerator.DeleteLicenseRequestAsync(licenseRequest2);
            await _dataGenerator.DeleteLicenseRequestAsync(licenseRequest1);
            await _dataGenerator.DeleteUserAsync(user);
            await _dataGenerator.DeleteDeviceAsync(device);
            await _dataGenerator.DeleteAccessPointAsync(accessPoint);
        }

        [TestMethod]
        public async Task LicenseRepository_GetExpiredLicensesAsync()
        {
            // Arrange
            AccessPointDto accessPoint = await _dataGenerator.CreateNewAccessPointAsync();
            DeviceDto device = await _dataGenerator.CreateNewDeviceAsync();
            UserDto user = await _dataGenerator.CreateNewUserAsync();
            LicenseRequestDto licenseRequest1 = await _dataGenerator.CreateNewLicenseRequest(accessPoint, device, user);
            LicenseDto license1 = await _dataGenerator.CreateNewLicense(licenseRequest1, true);
            LicenseRequestDto licenseRequest2 = await _dataGenerator.CreateNewLicenseRequest(accessPoint, device, user);
            LicenseDto license2 = await _dataGenerator.CreateNewLicense(licenseRequest2, true);

            InitIntegrationTest.TestLicenseRequestIds.Add(licenseRequest1.LicenseRequestID);
            InitIntegrationTest.TestLicenseRequestIds.Add(licenseRequest2.LicenseRequestID);
            InitIntegrationTest.TestDeviceIds.Add(device.DeviceID);
            InitIntegrationTest.TestUserIds.Add(user.UserID);
            InitIntegrationTest.TestAccessPointIds.Add(accessPoint.WifiBSSID);

            // Act
            IList<LicenseDto> result = await _sut.GetExpiredLicensesAsync().ConfigureAwait(false);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count >= 2);

            LicenseDto license = result.First(x => x.LicenseRequest.LicenseRequestID == license1.LicenseRequest.LicenseRequestID);

            Assert.IsNotNull(license.LicenseRequest);
            Assert.AreEqual(license.School, license1.School);
            Assert.AreEqual(license.ConfigCode, license1.ConfigCode);
            Assert.AreEqual(license.WifiBSSID, license1.WifiBSSID);
            TimeSpan issueDiff = license.LicenseIssueDateTime.Subtract(license1.LicenseIssueDateTime);
            Assert.IsTrue(issueDiff < TimeSpan.FromSeconds(1) && issueDiff > TimeSpan.FromSeconds(-1));
            TimeSpan expiryDiff = license.LicenseExpiryDateTime.Subtract(license1.LicenseExpiryDateTime);
            Assert.IsTrue(expiryDiff < TimeSpan.FromSeconds(1) && expiryDiff > TimeSpan.FromSeconds(-1));

            // Cleanup
            await _dataGenerator.DeleteLicenseAsync(license2);
            await _dataGenerator.DeleteLicenseAsync(license1);
            await _dataGenerator.DeleteLicenseRequestAsync(licenseRequest2);
            await _dataGenerator.DeleteLicenseRequestAsync(licenseRequest1);
            await _dataGenerator.DeleteUserAsync(user);
            await _dataGenerator.DeleteDeviceAsync(device);
            await _dataGenerator.DeleteAccessPointAsync(accessPoint);
        }

        [TestMethod]
        public async Task LicenseRepository_GetLicenseForDeviceAsync_InvalidDeviceId_Failure()
        {
            //Arrange
            var deviceId = Guid.NewGuid();
            LicenseDto licenseDto = null;
            Exception expectedException = null;

            //Act
            try
            {
                licenseDto = await _sut.GetLicenseForDeviceAsync(deviceId).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                expectedException = ex;
            }

            //Assert
            Assert.IsNull(expectedException);
            Assert.IsNull(licenseDto);
        }

        [TestMethod]
        public async Task LicenseRepository_GetLicenseForDeviceAsync_ValidDeviceId_ExpiredLicense_ReturnsNull()
        {
            //Arrange
            LicenseDto licenseDto = null;
            Exception expectedException = null;
            Guid licenseRequestId1 = Guid.NewGuid();
            Guid deviceId1 = Guid.NewGuid();
            string wifiBssId1 = DataGenerator.GetRandomMacAddress();
            Guid userId1 = Guid.NewGuid();

            InitIntegrationTest.TestLicenseRequestIds.Add(licenseRequestId1);
            InitIntegrationTest.TestDeviceIds.Add(deviceId1);
            InitIntegrationTest.TestUserIds.Add(userId1);
            InitIntegrationTest.TestAccessPointIds.Add(wifiBssId1);

            await CreateTestData(deviceId1, userId1, wifiBssId1).ConfigureAwait(false);
            await DataAccessHelper.ExecuteAsync(string.Format(
                "INSERT INTO [dbo].[LicenseRequest] ([LicenseRequestID], [ConfigCode] ,[LicenseRequestTypeID]," +
                "[DeviceID],[WifiBSSID], [UserID], [RequestDateTime],[Response],[ResponseDateTime]) VALUES ('{0}'," +
                "'{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}','{8}')", licenseRequestId1, ConfigCode, RequestLicense,
                deviceId1, wifiBssId1, userId1, DateTime.UtcNow.AddHours(-2), "License Granted",
                DateTime.UtcNow.AddHours(-2))).ConfigureAwait(false);
            await DataAccessHelper.ExecuteAsync(string.Format(
              "INSERT INTO [dbo].[License] ([LicenseRequestID], [ConfigCode] ,[LicenseIssueDateTime]," +
              "[WifiBSSID], [LicenseExpiryDateTime]) VALUES ('{0}'," +
              "'{1}', '{2}', '{3}', '{4}')", licenseRequestId1, ConfigCode, DateTime.UtcNow.AddHours(-2),
              wifiBssId1, DateTime.UtcNow.AddHours(-1))).ConfigureAwait(false);

            //Act
            try
            {
                licenseDto = await _sut.GetLicenseForDeviceAsync(deviceId1).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                expectedException = ex;
            }

            //Assert
            Assert.IsNull(expectedException);
            Assert.IsNull(licenseDto);

            //Cleanup
            await CleanupTestData(licenseRequestId1, deviceId1, userId1, wifiBssId1).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task LicenseRepository_GetLicenseForDeviceAsync_ValidDeviceId_ValidLicense_ReturnsSuccess()
        {
            //Arrange
            LicenseDto licenseDto = null;
            Exception expectedException = null;
            Guid licenseRequestId2 = Guid.NewGuid();
            Guid deviceId2 = Guid.NewGuid();
            string wifiBssId2 = DataGenerator.GetRandomMacAddress();
            Guid userId2 = Guid.NewGuid();

            InitIntegrationTest.TestLicenseRequestIds.Add(licenseRequestId2);
            InitIntegrationTest.TestDeviceIds.Add(deviceId2);
            InitIntegrationTest.TestUserIds.Add(userId2);
            InitIntegrationTest.TestAccessPointIds.Add(wifiBssId2);

            await CreateTestData(deviceId2, userId2, wifiBssId2).ConfigureAwait(false);
            await DataAccessHelper.ExecuteAsync(string.Format(
                "INSERT INTO [dbo].[LicenseRequest] ([LicenseRequestID], [ConfigCode] ,[LicenseRequestTypeID]," +
                "[DeviceID],[WifiBSSID], [UserID], [RequestDateTime],[Response],[ResponseDateTime]) VALUES ('{0}'," +
                "'{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}','{8}')", licenseRequestId2, ConfigCode, RequestLicense,
                deviceId2, wifiBssId2, userId2, DateTime.UtcNow, "License Granted", DateTime.UtcNow)).ConfigureAwait(false);
            await DataAccessHelper.ExecuteAsync(string.Format(
                "INSERT INTO [dbo].[License] ([LicenseRequestID], [ConfigCode] ,[LicenseIssueDateTime]," +
                "[WifiBSSID], [LicenseExpiryDateTime]) VALUES ('{0}'," +
                "'{1}', '{2}', '{3}', '{4}')", licenseRequestId2, ConfigCode, DateTime.UtcNow,
                wifiBssId2, DateTime.UtcNow.AddHours(1))).ConfigureAwait(false);

            //Act
            try
            {
                licenseDto = await _sut.GetLicenseForDeviceAsync(deviceId2).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                expectedException = ex;
            }

            //Assert
            Assert.IsNull(expectedException);
            Assert.IsNotNull(licenseDto);

            //Cleanup
            await CleanupTestData(licenseRequestId2, deviceId2, userId2, wifiBssId2).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task LicenseRepository_GrantLicenseForDeviceAsync_ExceededThreshold_ReturnsNull()
        {
            //Arrange
            bool grantResult = false;
            LicenseDto licenseDto = null;
            Exception expectedException = null;
            Guid licenseRequestId5 = Guid.NewGuid();
            Guid deviceId5 = Guid.NewGuid();
            string wifiBssId5 = DataGenerator.GetRandomMacAddress();
            Guid userId5 = new Guid("A505456E-FFE3-4D84-B42B-17C06A123DEF");
            int apMaxDownloadLicenses = 0;

            InitIntegrationTest.TestLicenseRequestIds.Add(licenseRequestId5);
            InitIntegrationTest.TestDeviceIds.Add(deviceId5);
            InitIntegrationTest.TestUserIds.Add(userId5);
            InitIntegrationTest.TestAccessPointIds.Add(wifiBssId5);

            DataAccessHelper.ExecuteAsync(string.Format(
               "INSERT INTO [dbo].[AccessPoint] ([WifiBSSID],[WifiSSID], [AccessPointMaxDownloadLicenses]) VALUES ('{0}', '{1}', '{2}')"

               , wifiBssId5, WifiSsId, apMaxDownloadLicenses)).Wait();
            var licenseRequest = new LicenseRequestDto()
            {
                User = new UserDto() { UserID = userId5, Username = Username, UserType = UserType },
                AccessPoint = new AccessPointDto()
                {
                    WifiBSSID = wifiBssId5,
                    WifiSSID = WifiSsId
                },
                Device = new DeviceDto()
                {
                    DeviceID = deviceId5,
                    LastUsedConfigCode = ConfigCode
                },
                LearningContentQueued = 1,
                RequestDateTime = DateTime.UtcNow,
                LicenseRequestType = LicenseRequestType.RequestLicense,
                LicenseRequestID = licenseRequestId5
            };

            //Act
            try
            {
                grantResult = await _sut.GrantLicenseForDeviceAsync(licenseRequest)
                                    .ConfigureAwait(false);
                licenseDto = await _sut.GetLicenseForDeviceAsync(deviceId5).ConfigureAwait(false);

            }
            catch (Exception ex)
            {
                expectedException = ex;
            }

            //Assert
            Assert.IsNull(expectedException);
            Assert.IsTrue(grantResult);
            Assert.IsNull(licenseDto);

            //Cleanup
            await CleanupTestData(licenseRequestId5, deviceId5, userId5, wifiBssId5).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task LicenseRepository_GrantLicenseForDeviceAsync_WithinThreshold_ReturnsLicense()
        {
            //Arrange
            bool grantResult = false;
            LicenseDto licenseDto = null;
            Exception expectedException = null;
            Guid licenseRequestId4 = new Guid("4A4BE2F5-A5F3-42F0-A148-73425A1C391F");
            Guid deviceId4 = new Guid("D401A16C-11A2-48E6-A5A3-C6F671943A52");
            string wifiBSSId4 = "a4:b4";
            Guid userId4 = new Guid("A404456E-FFE3-4D84-B42B-17C06A123DEF");

            InitIntegrationTest.TestLicenseRequestIds.Add(licenseRequestId4);
            InitIntegrationTest.TestDeviceIds.Add(deviceId4);
            InitIntegrationTest.TestUserIds.Add(userId4);
            InitIntegrationTest.TestAccessPointIds.Add(wifiBSSId4);

            var licenseRequest = new LicenseRequestDto()
            {
                User = new UserDto() {UserID = userId4, Username = Username, UserType = UserType},
                AccessPoint = new AccessPointDto()
                {
                    WifiBSSID = wifiBSSId4,
                    WifiSSID = WifiSsId
                },
                Device = new DeviceDto()
                {
                    DeviceID = deviceId4,
                    LastUsedConfigCode = ConfigCode
                },
                LearningContentQueued = 1,
                RequestDateTime = DateTime.UtcNow,
                LicenseRequestType = LicenseRequestType.RequestLicense,
                LicenseRequestID = licenseRequestId4
            };

            //Act
            try
            {
                grantResult = await _sut.GrantLicenseForDeviceAsync(licenseRequest)
                                    .ConfigureAwait(false);
                licenseDto = await _sut.GetLicenseForDeviceAsync(deviceId4).ConfigureAwait(false);

            }
            catch (Exception ex)
            {
                expectedException = ex;
            }

            //Assert
            Assert.IsNull(expectedException);
            Assert.IsTrue(grantResult);
            Assert.IsNotNull(licenseDto);

            //Cleanup
            await CleanupTestData(licenseRequestId4, deviceId4, userId4, wifiBSSId4).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task LicenseRepository_GrantLicenseForDeviceAsync_CreatesNewLicenseRequestUpdatesLicense()
        {
            //Arrange
            bool grantResult1 = false;
            bool grantResult2 = false;
            LicenseDto licenseDto1 = null;
            LicenseDto licenseDto2 = null;
            Exception expectedException = null;

            Guid licenseRequestId1 = new Guid("{3CAFE03A-2C69-4073-A6D8-D756533D70B0}");
            Guid licenseRequestId2 = new Guid("{C9BE2418-072F-4CCB-8E7D-FF6B6DEEF23A}");
            Guid deviceId = new Guid("{752493F9-4DCB-4ABA-98F7-E2061ADF7074}");
            Guid userId = new Guid("{8DA9A9AD-A5BF-4E79-A91B-03111A88EBEE}");

            string wifiBSSId = DataGenerator.GetRandomMacAddress();
            while (InitIntegrationTest.TestAccessPointIds.Contains(wifiBSSId))
            {
                wifiBSSId = DataGenerator.GetRandomMacAddress();
            }

            InitIntegrationTest.TestLicenseRequestIds.Add(licenseRequestId1);
            InitIntegrationTest.TestLicenseRequestIds.Add(licenseRequestId2);
            InitIntegrationTest.TestDeviceIds.Add(deviceId);
            InitIntegrationTest.TestUserIds.Add(userId);
            InitIntegrationTest.TestAccessPointIds.Add(wifiBSSId);

             var school = new SchoolDto
                    {
                        SchoolID = InitIntegrationTest.TestSchoolId,
                        District = new DistrictDto
                        {
                            DistrictId = InitIntegrationTest.TestDistrictId
                        }
                    };

            var licenseRequest = new LicenseRequestDto()
            {
                User = new UserDto() { UserID = userId, Username = Username, UserType = UserType },
                AccessPoint = new AccessPointDto()
                {
                    WifiBSSID = wifiBSSId,
                    WifiSSID = WifiSsId
                },
                School =  school,
                Device = new DeviceDto()
                {
                    DeviceID = deviceId,
                    LastUsedConfigCode = ConfigCode
                },
                LearningContentQueued = 1,
                RequestDateTime = DateTime.UtcNow,
                LicenseRequestType = LicenseRequestType.RequestLicense,
                LicenseRequestID = licenseRequestId1
            };

            //Act
            try
            {
                grantResult1 = await _sut.GrantLicenseForDeviceAsync(licenseRequest)
                                    .ConfigureAwait(false);
                licenseDto1 = await _sut.GetLicenseForDeviceAsync(deviceId).ConfigureAwait(false);

            }
            catch (Exception ex)
            {
                expectedException = ex;
            }

            //Assert
            Assert.IsNull(expectedException);
            Assert.IsTrue(grantResult1);
            Assert.IsNotNull(licenseDto1);

            /*************************************   Part 2 **********************************************/
            //Arrange 2  (grant again)
            licenseRequest.LicenseRequestID = licenseRequestId2;

            //Act
            try
            {
                grantResult2 = await _sut.GrantLicenseForDeviceAsync(licenseRequest)
                                    .ConfigureAwait(false);

                licenseDto2 = await _sut.GetLicenseForDeviceAsync(deviceId).ConfigureAwait(false);

            }
            catch (Exception ex)
            {
                expectedException = ex;
            }

            //Assert
            Assert.IsNull(expectedException);
            Assert.IsTrue(grantResult2);
            Assert.IsNotNull(licenseDto2);
            Assert.AreEqual(licenseRequestId2, licenseDto2.LicenseRequest.LicenseRequestID);
            Assert.AreEqual(licenseRequestId1, licenseDto2.LicenseRequest.PrevLicenseRequestID);

        }

        [TestMethod]
        public async Task LicenseRepository_Insert()
        {
            Guid testlicenseRequestId = Guid.NewGuid();

            var school = new SchoolDto
            {
                SchoolID = InitIntegrationTest.TestSchoolId,
                District = new DistrictDto
                {
                    DistrictId = InitIntegrationTest.TestDistrictId
                }
            };

            var dto = new LicenseDto
            {
                LicenseRequest = new LicenseRequestDto
                {
                    LicenseRequestID = InitIntegrationTest.TestLicenseRequestIdForLicenseInsert
                },
                AccessPoint = new AccessPointDto
                {
                    WifiBSSID = InitIntegrationTest.TestWifiBSSID,
                    WifiSSID = "Test AccessPoint",
                    School = new SchoolDto
                    {
                        SchoolID = InitIntegrationTest.TestSchoolId,
                        District = new DistrictDto
                        {
                            DistrictId = InitIntegrationTest.TestDistrictId
                        }
                    }
                },
                School = school,
                ConfigCode = "??",
                LicenseExpiryDateTime = DateTime.Now.AddMinutes(30),
                LicenseIssueDateTime = DateTime.UtcNow,
                WifiBSSID = InitIntegrationTest.TestWifiBSSID
            };

            var result = await _sut.UpdateAsync(dto).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.LicenseRequest);
            Assert.IsTrue(dto.LicenseRequest.LicenseRequestID == result.LicenseRequest.LicenseRequestID);
        }

        [TestMethod]
        public async Task LicenseRepository_RevokeLicenseForDeviceAsync_InvalidLicenseRequestId_Failure()
        {
            //Arrange
            var licenseRequestId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            bool result = false;
            Exception expectedException = null;

            //Act
            try
            {
                result = await _sut.RevokeLicenseForDeviceAsync(licenseRequestId, userId, DateTime.Now, false).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                expectedException = ex;
            }

            //Assert
            Assert.IsNull(expectedException);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task LicenseRepository_RevokeLicenseForDeviceAsync_ValidDeviceId_ValidLicense_Success()
        {
            //Arrange
            bool revokeResult = false;
            LicenseDto licenseDto = null;
            Exception expectedException = null;
            Guid licenseRequestId3 = Guid.NewGuid();
            Guid deviceId3 = Guid.NewGuid();
            string wifiBssId3 = DataGenerator.GetRandomMacAddress();
            Guid userId3 = Guid.NewGuid();

            InitIntegrationTest.TestLicenseRequestIds.Add(licenseRequestId3);
            InitIntegrationTest.TestDeviceIds.Add(deviceId3);
            InitIntegrationTest.TestUserIds.Add(userId3);
            InitIntegrationTest.TestAccessPointIds.Add(wifiBssId3);
           
            await CreateTestData(deviceId3, userId3, wifiBssId3).ConfigureAwait(false);
            await DataAccessHelper.ExecuteAsync(string.Format(
                "INSERT INTO [dbo].[LicenseRequest] ([LicenseRequestID], [ConfigCode] ,[LicenseRequestTypeID]," +
                "[DeviceID],[WifiBSSID], [UserID], [RequestDateTime],[Response],[ResponseDateTime]) VALUES ('{0}'," +
                "'{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}','{8}')", licenseRequestId3, ConfigCode, RequestLicense,
                deviceId3, wifiBssId3, userId3, DateTime.UtcNow, "License Granted", DateTime.UtcNow)).ConfigureAwait(false);

            await DataAccessHelper.ExecuteAsync(string.Format(
                "INSERT INTO [dbo].[License] ([LicenseRequestID], [ConfigCode] ,[LicenseIssueDateTime]," +
                "[WifiBSSID], [LicenseExpiryDateTime]) VALUES ('{0}'," +
                "'{1}', '{2}', '{3}', '{4}')", licenseRequestId3, ConfigCode, DateTime.UtcNow,
                wifiBssId3, DateTime.UtcNow.AddHours(1))).ConfigureAwait(false);

            //Act
            try
            {
                revokeResult = await _sut.RevokeLicenseForDeviceAsync(licenseRequestId3, userId3, DateTime.UtcNow, false)
                                    .ConfigureAwait(false);
                licenseDto = await _sut.GetLicenseForDeviceAsync(deviceId3).ConfigureAwait(false);

            }
            catch (Exception ex)
            {
                expectedException = ex;
            }

            //Assert
            Assert.IsNull(expectedException);
            Assert.IsTrue(revokeResult);
            Assert.IsNull(licenseDto);

            //Cleanup
            await CleanupTestData(licenseRequestId3, deviceId3, userId3, wifiBssId3).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task LicenseRepository_UpdateAsync()
        {
            Guid testlicenseRequestId = Guid.NewGuid();

            var school = new SchoolDto
            {
                SchoolID = InitIntegrationTest.TestSchoolId,
                District = new DistrictDto
                {
                    DistrictId = InitIntegrationTest.TestDistrictId
                }
            };

            var dto = new LicenseDto
            {
                LicenseRequest = new LicenseRequestDto
                {
                    LicenseRequestID = InitIntegrationTest.TestLicenseRequestId
                },
                AccessPoint = new AccessPointDto
                {
                    WifiBSSID = InitIntegrationTest.TestWifiBSSID,
                    WifiSSID = "Test AccessPoint",
                    School = new SchoolDto
                    {
                        SchoolID = InitIntegrationTest.TestSchoolId,
                        District = new DistrictDto
                        {
                            DistrictId = InitIntegrationTest.TestDistrictId
                        }
                    }
                },
                School = school,
                ConfigCode = "??",
                LicenseExpiryDateTime = DateTime.Now.AddMinutes(30),
                LicenseIssueDateTime = DateTime.UtcNow,
                WifiBSSID = InitIntegrationTest.TestWifiBSSID
            };

            var result = await _sut.UpdateAsync(dto).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.IsTrue(dto.LicenseRequest.LicenseRequestID == result.LicenseRequest.LicenseRequestID);
        }

        private async Task CreateTestData(Guid deviceId, Guid userId, string wifiBSSId)
        {

            InitIntegrationTest.TestDeviceIds.Add(deviceId);
            InitIntegrationTest.TestUserIds.Add(userId);
            InitIntegrationTest.TestAccessPointIds.Add(wifiBSSId);
                        
            //Insert test data
            //Device
            await DataAccessHelper.ExecuteAsync(String.Format("INSERT INTO [dbo].[Device] (DeviceID) VALUES ('{0}')",
                deviceId)).ConfigureAwait(false);

            //User
            string query = "INSERT INTO [dbo].[User] "
                            + @"([UserID]
                                ,[Username]
                                ,[UsernameHash]
                                ,[UserType]
                                ,[UserTypeHash])
                            VALUES
                                (@UserID
                                ,@Username
                                ,@UsernameHash
                                ,@UserType
                                ,@UserTypeHash)"
               + Environment.NewLine;

             var paramList = new List<SqlParameter>
            {
                new SqlParameter("@UserID", SqlDbType.UniqueIdentifier) { Value =  userId},
                new SqlParameter("@Username", SqlDbType.Binary) { Value =  _usernameEnc.EncryptedValue.NullIfEmpty()},
                new SqlParameter("@UsernameHash", SqlDbType.Binary) { Value =  _usernameEnc.GetHashBytes().NullIfEmpty()},
                new SqlParameter("@UserType", SqlDbType.Binary) { Value =  _userTypeEnc.EncryptedValue.NullIfEmpty()},
                new SqlParameter("@UserTypeHash", SqlDbType.Binary) { Value =  _userTypeEnc.GetHashBytes().NullIfEmpty()},
            };

            await DataAccessHelper.ExecuteAsync(query, paramList: paramList).ConfigureAwait(false);

            //Accesspoint
            await DataAccessHelper.ExecuteAsync(string.Format(
                "INSERT INTO [dbo].[AccessPoint] ([WifiBSSID],[WifiSSID]) VALUES ('{0}', '{1}')", wifiBSSId,
                WifiSsId)).ConfigureAwait(false);
        }

        private async Task CleanupTestData(Guid licenseRequestId, Guid deviceId, Guid userId, string wifiBSSId)
        {
            //Remove Test Data
            //License
            await
                DataAccessHelper.ExecuteAsync("DELETE FROM [dbo].[License] WHERE [LicenseRequestID] = '" +
                                              licenseRequestId + "'").ConfigureAwait(false);
            //LicenseRequest
            await
                DataAccessHelper.ExecuteAsync("DELETE FROM [dbo].[LicenseRequest] WHERE [DeviceID] = '" +
                                              deviceId + "'").ConfigureAwait(false);

            await
                DataAccessHelper.ExecuteAsync("DELETE FROM [dbo].[Device] WHERE [DeviceID] = '"
                                            + deviceId + "'").ConfigureAwait(false);

            //User
            await
                DataAccessHelper.ExecuteAsync("DELETE FROM [dbo].[User] WHERE [UserID] = '"
                                            + userId + "'").ConfigureAwait(false);

            //Accesspoint
            await
                DataAccessHelper.ExecuteAsync("DELETE FROM [dbo].[AccessPoint] WHERE [WifiBSSID] = '"
                                            + wifiBSSId + "'").ConfigureAwait(false);
        }
    }
}