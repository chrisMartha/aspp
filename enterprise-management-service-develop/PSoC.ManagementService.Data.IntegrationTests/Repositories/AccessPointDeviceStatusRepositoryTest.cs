using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting; 

using PSoC.ManagementService.Core;
using PSoC.ManagementService.Data.Helpers;
using PSoC.ManagementService.Data.Repositories;

namespace PSoC.ManagementService.Data.IntegrationTests.Repositories
{
    [TestClass]
    public class AccessPointDeviceStatusRepositoryTest
    {
        private AccessPointDeviceStatusRepository _accessPointDeviceStatusRepository;
        private string _wifibssid;
        private Guid _deviceId;
        private Guid _userId;
        private Guid _licenseRequestId;
        private string _configCode;
        private int _licenseRequestTypeId;
        private DateTime _requestDateTime;

        [TestInitialize]
        public void TestInit()
        {
            _accessPointDeviceStatusRepository = new AccessPointDeviceStatusRepository();
            _wifibssid = Guid.NewGuid().ToString().Substring(0, 17);
            _deviceId = Guid.NewGuid();
            _userId = Guid.NewGuid();
            _licenseRequestId = Guid.NewGuid();
            _configCode = "APDeviceStatus_Get_Test";
            _licenseRequestTypeId = 1;
            _requestDateTime = DateTime.UtcNow;

            DataAccessHelper.ExecuteAsync(String.Format(@"INSERT INTO [dbo].[AccessPoint] ([WifiBSSID], [WifiSSID]) VALUES ('{0}', '{1}')",
                                                                                                                        _wifibssid, 
                                                                                                                        _wifibssid.Substring(0, 6))).Wait();
            DataAccessHelper.ExecuteAsync(String.Format(@"INSERT INTO [dbo].[Device] ([DeviceID]) VALUES ('{0}')",
                                                                                                        _deviceId)).Wait();
            DataAccessHelper.ExecuteAsync(String.Format(@"INSERT INTO [dbo].[User] ([UserID]) VALUES ('{0}')",
                                                                                                        _userId)).Wait();
            DataAccessHelper.ExecuteAsync(String.Format(@"INSERT INTO [dbo].[LicenseRequest] ([LicenseRequestID], 
                                                                                                    [ConfigCode],
                                                                                                    [WifiBSSID],
                                                                                                    [LicenseRequestTypeID],
                                                                                                    [DeviceID],
                                                                                                    [UserID],
                                                                                                    [RequestDateTime])
                                                                                            VALUES ('{0}','{1}', '{2}', {3}, '{4}', '{5}', '{6}')", 
                                                                                                    _licenseRequestId,
                                                                                                    _configCode,
                                                                                                    _wifibssid,
                                                                                                    _licenseRequestTypeId,
                                                                                                    _deviceId,
                                                                                                    _userId,
                                                                                                    _requestDateTime)).Wait();

            InitIntegrationTest.TestLicenseRequestIds.Add(_licenseRequestId);
            InitIntegrationTest.TestDeviceIds.Add(_deviceId);
            InitIntegrationTest.TestUserIds.Add(_userId);
            InitIntegrationTest.TestAccessPointIds.Add(_wifibssid);

        }

        [TestCleanup]
        public void CleanUp()
        {
            DataAccessHelper.ExecuteAsync("DELETE FROM [dbo].[LicenseRequest] WHERE [LicenseRequestID] = '" + _licenseRequestId + "'").Wait();
            DataAccessHelper.ExecuteAsync("DELETE FROM [dbo].[LicenseRequest] WHERE [UserID] = '" + _userId + "'").Wait();
            DataAccessHelper.ExecuteAsync("DELETE FROM [dbo].[User] WHERE [UserID] = '" + _userId + "'").Wait();
            DataAccessHelper.ExecuteAsync("DELETE FROM [dbo].[Device] WHERE [DeviceID] = '" + _deviceId + "'").Wait();
            DataAccessHelper.ExecuteAsync("DELETE FROM [dbo].[AccessPoint] WHERE [WifiBSSID] = '" + _wifibssid + "'").Wait();
        }

        [TestMethod]
        public async Task APDeviceStatus_GetGlobal_Test()
        {
            var result = await _accessPointDeviceStatusRepository.GetByAdminTypeAsync(AdminType.GlobalAdmin, null, 10, 0).ConfigureAwait(false);
            Assert.IsTrue(result.Item1.Count > 0);
            Assert.IsTrue(result.Item1.Count <= 10);
            Assert.IsTrue(result.Item2 > 0);
        }

        [TestMethod]
        public async Task APDeviceStatus_GetGlobal_Pagination_FirstPage()
        {
            var deviceIdList = new List<Guid>();
            var licenseRequestIdList = new List<Guid>();
            for (int i = 0; i < 4; i++)
            {
                deviceIdList.Add(Guid.NewGuid());
                licenseRequestIdList.Add(Guid.NewGuid());

                InitIntegrationTest.TestDeviceIds.Add(deviceIdList[i]);
                InitIntegrationTest.TestLicenseRequestIds.Add(licenseRequestIdList[i]);

                await DataAccessHelper.ExecuteAsync(String.Format(@"INSERT INTO [dbo].[Device] ([DeviceID]) VALUES ('{0}')", deviceIdList[i])).ConfigureAwait(false);
                await DataAccessHelper.ExecuteAsync(String.Format(@"INSERT INTO [dbo].[LicenseRequest] ([LicenseRequestID], 
                                                                                              [DeviceID], 
                                                                                              [ConfigCode], [WifiBSSID], [LicenseRequestTypeID], [UserID], [RequestDateTime])
                                                                                            VALUES (
                                                                                                '{0}', 
                                                                                                '{1}',
                                                                                                '{2}', '{3}', {4}, '{5}', '{6}')",
                                                                                                licenseRequestIdList[i],
                                                                                                deviceIdList[i],
                                                                                                _configCode, _wifibssid, _licenseRequestTypeId, _userId, _requestDateTime)).ConfigureAwait(false);
            }
            
            var result = await _accessPointDeviceStatusRepository.GetByAdminTypeAsync(AdminType.GlobalAdmin, null, 3, 0).ConfigureAwait(false);

            //Assert
            Assert.AreEqual(3, result.Item1.Count);
            Assert.IsFalse(result.Item1.Exists(x => x.DeviceID == deviceIdList[0]));
            for (int i = 1; i < 4; i++)
            {
                Assert.IsTrue(result.Item1.Exists(x => x.DeviceID == deviceIdList[i]));
            }
            Assert.IsTrue(result.Item2 >= 4);

            //CLean Up
            for (int i = 0; i < 4; i++)
            {
                await DataAccessHelper.ExecuteAsync(String.Format("DELETE FROM [dbo].[LicenseRequest] WHERE [LicenseRequestID] = '{0}'", licenseRequestIdList[i])).ConfigureAwait(false);
                await DataAccessHelper.ExecuteAsync(String.Format("DELETE FROM [dbo].[Device] WHERE [DeviceID] = '{0}'", deviceIdList[i])).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task APDeviceStatus_GetGlobal_Pagination_SecondPage()
        {
            var deviceIdList = new List<Guid>();
            var licenseRequestIdList = new List<Guid>();
            for (int i = 0; i < 4; i++)
            {
                deviceIdList.Add(Guid.NewGuid());
                licenseRequestIdList.Add(Guid.NewGuid());

                InitIntegrationTest.TestDeviceIds.Add(deviceIdList[i]);
                InitIntegrationTest.TestLicenseRequestIds.Add(licenseRequestIdList[i]);

                await DataAccessHelper.ExecuteAsync(String.Format(@"INSERT INTO [dbo].[Device] ([DeviceID]) VALUES ('{0}')", deviceIdList[i])).ConfigureAwait(false);
                await DataAccessHelper.ExecuteAsync(String.Format(@"INSERT INTO [dbo].[LicenseRequest] ([LicenseRequestID], 
                                                                                              [DeviceID], 
                                                                                              [ConfigCode], [WifiBSSID], [LicenseRequestTypeID], [UserID], [RequestDateTime])
                                                                                            VALUES (
                                                                                                '{0}', 
                                                                                                '{1}',
                                                                                                '{2}', '{3}', {4}, '{5}', '{6}')",
                                                                                                licenseRequestIdList[i],
                                                                                                deviceIdList[i],
                                                                                                _configCode, _wifibssid, _licenseRequestTypeId, _userId, _requestDateTime)).ConfigureAwait(false);
            }
            

            var result = await _accessPointDeviceStatusRepository.GetByAdminTypeAsync(AdminType.GlobalAdmin, null, 3, 3).ConfigureAwait(false);

            //Assert
            Assert.IsTrue(result.Item1.Count >= 1);
            Assert.IsTrue(result.Item1.Count <= 3);
            Assert.IsTrue(result.Item1.Exists(x => x.DeviceID == deviceIdList[0]));
            for (int i = 1; i < 4; i++)
            {
                Assert.IsFalse(result.Item1.Exists(x => x.DeviceID == deviceIdList[i]));
            }
            Assert.IsTrue(result.Item2 >= 4);

            //CLean Up
            for (int i = 0; i < 4; i++)
            {
                await DataAccessHelper.ExecuteAsync(String.Format("DELETE FROM [dbo].[LicenseRequest] WHERE [LicenseRequestID] = '{0}'", licenseRequestIdList[i])).ConfigureAwait(false);
                await DataAccessHelper.ExecuteAsync(String.Format("DELETE FROM [dbo].[Device] WHERE [DeviceID] = '{0}'", deviceIdList[i])).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task APDeviceStatus_GetGlobal_Pagination_InvalidSizeAndIndex_WillResetToDefault()
        {
            var deviceIdList = new List<Guid>();
            var licenseRequestIdList = new List<Guid>();
            for (int i = 0; i < 10; i++)
            {
                deviceIdList.Add(Guid.NewGuid());
                licenseRequestIdList.Add(Guid.NewGuid());

                InitIntegrationTest.TestDeviceIds.Add(deviceIdList[i]);
                InitIntegrationTest.TestLicenseRequestIds.Add(licenseRequestIdList[i]);

                await DataAccessHelper.ExecuteAsync(String.Format(@"INSERT INTO [dbo].[Device] ([DeviceID]) VALUES ('{0}')", deviceIdList[i])).ConfigureAwait(false);
                await DataAccessHelper.ExecuteAsync(String.Format(@"INSERT INTO [dbo].[LicenseRequest] ([LicenseRequestID], 
                                                                                              [DeviceID], 
                                                                                              [ConfigCode], [WifiBSSID], [LicenseRequestTypeID], [UserID], [RequestDateTime])
                                                                                            VALUES (
                                                                                                '{0}', 
                                                                                                '{1}',
                                                                                                '{2}', '{3}', {4}, '{5}', '{6}')",
                                                                                                licenseRequestIdList[i],
                                                                                                deviceIdList[i],
                                                                                                _configCode, _wifibssid, _licenseRequestTypeId, _userId, _requestDateTime)).ConfigureAwait(false);
            }

            var result = await _accessPointDeviceStatusRepository.GetByAdminTypeAsync(AdminType.GlobalAdmin, null, 0, -1).ConfigureAwait(false); // will reset (invalid) 0, -1 to (default) 10, 0
            
            //Assert
            Assert.AreEqual(10, result.Item1.Count);
            for (int i = 0; i < 10; i++)
            {
                Assert.IsTrue(result.Item1.Exists(x => x.DeviceID == deviceIdList[i]));
            }
            Assert.IsTrue(result.Item2 >= 10);

            //CLean Up
            for (int i = 0; i < 10; i++)
            {
                await DataAccessHelper.ExecuteAsync(String.Format("DELETE FROM [dbo].[LicenseRequest] WHERE [LicenseRequestID] = '{0}'", licenseRequestIdList[i])).ConfigureAwait(false);
                await DataAccessHelper.ExecuteAsync(String.Format("DELETE FROM [dbo].[Device] WHERE [DeviceID] = '{0}'", deviceIdList[i])).ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task APDeviceStatus_GetGlobal_HaveRevokable()
        {
            DateTime expiryDateTime = _requestDateTime + new TimeSpan(30, 0, 0, 0);

            await DataAccessHelper.ExecuteAsync(String.Format(@"INSERT INTO [dbo].[License] ([LicenseRequestID],
                                                                                             [ConfigCode],
                                                                                             [WifiBSSID],
                                                                                             [LicenseIssueDateTime],
                                                                                             [LicenseExpiryDateTime]
                                                                                            )
                                                                                        VALUES
                                                                                            ('{0}'
                                                                                            ,'{1}'
                                                                                            ,'{2}'
                                                                                            ,'{3}'
                                                                                            ,'{4}')",
                                                                                        _licenseRequestId,
                                                                                        _configCode,
                                                                                        _wifibssid, 
                                                                                        _requestDateTime, 
                                                                                        expiryDateTime)).ConfigureAwait(false);

            var result = await _accessPointDeviceStatusRepository.GetByAdminTypeAsync(AdminType.GlobalAdmin, null, 10, 0).ConfigureAwait(false);
            Assert.IsTrue(result.Item1.Count(x => x.CanRevoke) > 0);
            Assert.IsTrue(result.Item1.Count <= 10);
            Assert.IsTrue(result.Item2 > 0);

            await DataAccessHelper.ExecuteAsync("DELETE FROM [dbo].[License] WHERE [LicenseRequestID] = '" + _licenseRequestId + "'").ConfigureAwait(false);
        }

        [TestMethod]
        public async Task APDeviceStatus_GetDistrict_NotExist()
        {
            var result = await _accessPointDeviceStatusRepository.GetByAdminTypeAsync(AdminType.DistrictAdmin, Guid.NewGuid(), 10, 0).ConfigureAwait(false);
            Assert.AreEqual(0, result.Item1.Count);
        }

        [TestMethod]
        public async Task APDeviceStatus_GetDistrict_Exist()
        {
            Guid districtId = Guid.NewGuid();
            string wifiBSSID = Guid.NewGuid().ToString().Substring(0, 17);

            InitIntegrationTest.TestDistrictIds.Add(districtId);
            InitIntegrationTest.TestAccessPointIds.Add(wifiBSSID);

            await DataAccessHelper.ExecuteAsync(String.Format(@"INSERT [dbo].[District]([DistrictID], 
                                                                                        [CreatedBy], 
                                                                                        [CreationDate], 
                                                                                        [DistrictName], 
                                                                                        [DistrictMaxDownloadLicenses], 
                                                                                        [DistrictInstructionHoursStart],
                                                                                        [DistrictInstructionHoursEnd],
                                                                                        [DistrictLicenseExpirySeconds],
                                                                                        [DistrictPreloadHoursStart], 
                                                                                        [DistrictPreloadHoursEnd],
                                                                                        [DistrictOverrideCode],
                                                                                        [DistrictUserPolicy],
                                                                                        [DistrictUseCacheServer], 
                                                                                        [DistrictAnnotation],
                                                                                        [OAuthApplicationId],
                                                                                        [OAuthClientId],
                                                                                        [OAuthURL]) 
                                                                            VALUES (N'{0}', 
                                                                                    N'APDeviceStatus_GetDistrict_Exist',
                                                                                    SYSUTCDATETIME(),
                                                                                    N'Test APDeviceStatus_GetDistrict_Exist', 
                                                                                    500, CAST(N'01:00:00' AS Time),
                                                                                    CAST(N'01:00:00' AS Time), 
                                                                                    300,
                                                                                    CAST(N'01:00:00' AS Time),
                                                                                    CAST(N'05:00:00' AS Time),
                                                                                    NULL,
                                                                                    2,
                                                                                    N'CACHE01   ', 
                                                                                    N'Test APDeviceStatus_GetDistrict_Exist',
                                                                                    N'PEMS TEST', 
                                                                                    N'IT TEST',
                                                                                    N'https://nowhere')", 
                                                                                    districtId)).ConfigureAwait(false);

            await DataAccessHelper.ExecuteAsync(String.Format(@"INSERT [dbo].[AccessPoint] ([WifiBSSID],
                                                                                            [WifiSSID], 
                                                                                            [DistrictID], 
                                                                                            --[SchoolID], 
                                                                                            [ClassroomID], 
                                                                                            [AccessPointMaxDownloadLicenses],
                                                                                            [AccessPointExpiryTimeSeconds],
                                                                                            [AccessPointAnnotation], 
                                                                                            [AccessPointModel], 
                                                                                            [Created]) 
                                                                                    VALUES (N'{0}',
                                                                                            N'APDeviceStatus Integration Test', 
                                                                                            N'{1}', 
                                                                                            --N'', 
                                                                                            NULL, 
                                                                                            200,
                                                                                            NULL,
                                                                                            NULL,
                                                                                            NULL, 
                                                                                            SYSUTCDATETIME())", 
                                                                                    wifiBSSID, districtId)).ConfigureAwait(false);



            await DataAccessHelper.ExecuteAsync(String.Format(@"UPDATE [dbo].[LicenseRequest] SET [WifiBSSID] = '{0}' WHERE [LicenseRequestID] = '{1}'", wifiBSSID, _licenseRequestId)).ConfigureAwait(false);
            var result = await _accessPointDeviceStatusRepository.GetByAdminTypeAsync(AdminType.DistrictAdmin, districtId, 10, 0).ConfigureAwait(false);
            Assert.IsTrue(result.Item1.Count > 0);
            Assert.IsTrue(result.Item1.Count <= 10);
            Assert.IsTrue(result.Item2 > 0);

            await DataAccessHelper.ExecuteAsync("DELETE FROM [dbo].[LicenseRequest] WHERE [LicenseRequestID] = '" + _licenseRequestId + "'").ConfigureAwait(false);
            await DataAccessHelper.ExecuteAsync("DELETE FROM [dbo].[AccessPoint] WHERE [WifiBSSID] = '" + wifiBSSID + "'").ConfigureAwait(false);
            await DataAccessHelper.ExecuteAsync("DELETE FROM [dbo].[District] WHERE [DistrictID] = '" + districtId + "'").ConfigureAwait(false);
        }

        [TestMethod]
        public async Task APDeviceStatus_GetSchool_NotExist()
        {
            var result = await _accessPointDeviceStatusRepository.GetByAdminTypeAsync(AdminType.SchoolAdmin, Guid.NewGuid(), 10, 0).ConfigureAwait(false);
            Assert.AreEqual(0, result.Item1.Count);
        }

        [TestMethod]
        public async Task APDeviceStatus_GetSchool_Exist()
        {
            Guid districtId = Guid.NewGuid();
            Guid schoolId = Guid.NewGuid();
            string wifiBSSID = Guid.NewGuid().ToString().Substring(0, 17);

            InitIntegrationTest.TestAccessPointIds.Add(wifiBSSID);
            InitIntegrationTest.TestDistrictIds.Add(districtId);
           
            await DataAccessHelper.ExecuteAsync(String.Format(@"INSERT [dbo].[District]([DistrictID], 
                                                                                        [CreatedBy], 
                                                                                        [CreationDate], 
                                                                                        [DistrictName], 
                                                                                        [DistrictMaxDownloadLicenses], 
                                                                                        [DistrictInstructionHoursStart],
                                                                                        [DistrictInstructionHoursEnd],
                                                                                        [DistrictLicenseExpirySeconds],
                                                                                        [DistrictPreloadHoursStart], 
                                                                                        [DistrictPreloadHoursEnd],
                                                                                        [DistrictOverrideCode],
                                                                                        [DistrictUserPolicy],
                                                                                        [DistrictUseCacheServer], 
                                                                                        [DistrictAnnotation],
                                                                                        [OAuthApplicationId],
                                                                                        [OAuthClientId],
                                                                                        [OAuthURL]) 
                                                                            VALUES (N'{0}', 
                                                                                    N'APDeviceStatus_GetDistrict_Exist',
                                                                                    SYSUTCDATETIME(),
                                                                                    N'Test APDeviceStatus_GetDistrict_Exist', 
                                                                                    500,
                                                                                    CAST(N'01:00:00' AS Time), 
                                                                                    CAST(N'01:00:00' AS Time), 
                                                                                    300,
                                                                                    CAST(N'01:00:00' AS Time), 
                                                                                    CAST(N'05:00:00' AS Time),
                                                                                    NULL, 
                                                                                    2, 
                                                                                    N'CACHE01   ', 
                                                                                    N'Test APDeviceStatus_GetDistrict_Exist',
                                                                                    N'PEMS TEST',
                                                                                    N'IT TEST',
                                                                                    N'https://nowhere')",
                                                                                    districtId)).ConfigureAwait(false);

            await DataAccessHelper.ExecuteAsync(String.Format(@"INSERT [dbo].[School]  ([SchoolID], 
                                                                                        [DistrictID], 
                                                                                        [SchoolName]) 
                                                                            VALUES (N'{0}', N'{1}', N'APDeviceStatus_GetSchool_Exist Sch00l')"
                                                , schoolId, districtId)).ConfigureAwait(false);

            await DataAccessHelper.ExecuteAsync(String.Format(@"INSERT [dbo].[AccessPoint] ([WifiBSSID],
                                                                                            [WifiSSID], 
                                                                                            [DistrictID], 
                                                                                            [SchoolID], 
                                                                                            [ClassroomID], 
                                                                                            [AccessPointMaxDownloadLicenses],
                                                                                            [AccessPointExpiryTimeSeconds],
                                                                                            [AccessPointAnnotation], 
                                                                                            [AccessPointModel], 
                                                                                            [Created]) 
                                                                                    VALUES (N'{0}',
                                                                                            N'APDeviceStatus Integration Test2', 
                                                                                            N'{1}', 
                                                                                            N'{2}', 
                                                                                            NULL, 
                                                                                            200,
                                                                                            NULL,
                                                                                            NULL,
                                                                                            NULL, 
                                                                                            SYSUTCDATETIME())",
                                                                                    wifiBSSID, districtId, schoolId)).ConfigureAwait(false);

            await DataAccessHelper.ExecuteAsync(String.Format(@"UPDATE [dbo].[LicenseRequest] SET [WifiBSSID] = '{0}' WHERE [LicenseRequestID] = '{1}'", wifiBSSID, _licenseRequestId)).ConfigureAwait(false);

            var result = await _accessPointDeviceStatusRepository.GetByAdminTypeAsync(AdminType.SchoolAdmin, schoolId, 10, 0).ConfigureAwait(false);
            Assert.IsTrue(result.Item1.Count > 0);
            Assert.IsTrue(result.Item1.Count <= 10);
            Assert.IsTrue(result.Item2 > 0);

            await DataAccessHelper.ExecuteAsync("DELETE FROM [dbo].[LicenseRequest] WHERE [LicenseRequestID] = '" + _licenseRequestId + "'").ConfigureAwait(false);
            await DataAccessHelper.ExecuteAsync("DELETE FROM [dbo].[AccessPoint] WHERE [WifiBSSID] = '" + wifiBSSID + "'").ConfigureAwait(false);
            await DataAccessHelper.ExecuteAsync("DELETE FROM [dbo].[School] WHERE [SchoolID] = '" + schoolId + "'").ConfigureAwait(false);
            await DataAccessHelper.ExecuteAsync("DELETE FROM [dbo].[District] WHERE [DistrictID] = '" + districtId + "'").ConfigureAwait(false);
        }
    }
}
