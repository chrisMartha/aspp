using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

using PSoC.ManagementService.Core;
using PSoC.ManagementService.Data.Helpers;
using PSoC.ManagementService.Security;
using PSoC.ManagementService.Services.Models;

namespace PSoC.ManagementService.IntegrationTests
{
    /// <summary>
    /// Device License Service API - Admin Revoke - Endpoint Integration Tests 
    /// </summary>
    [TestClass]
    public class DeviceLicenseServiceAdminRevokeTest
    {
        #region Settings

        private const string LicenseService = "api/v1/devices";
        private const string AccountLogin = "Account/Login";
        private const string AccountLogOut = "Account/Logout";

        private const int RevokeLicenseTypeId = 3;
        private const int RequestLicenseTypeId = 1;
        private readonly Guid _deviceId = Guid.NewGuid();
        private readonly Guid _userId = Guid.NewGuid();
        private readonly Guid _licenseRequestId = Guid.NewGuid();
        private const string Username = "ApiAdminRevokeTest";
        private const string UserType = "Student";
        private readonly EncrypedField<string> _usernameEnc = Username;
        private readonly EncrypedField<string> _userTypeEnc = UserType;
        private const string WifiSSId = "ar-pems";
        private readonly string _wifiBssId = GetRandomString();
        private const string ConfigCode = "configCode";

        protected static HttpClient HttpClientHelper { get; set; }
        public static string AdminUserName { get { return ConfigurationManager.AppSettings["AdminUserName"]; } }
        public static string AdminPassword { get { return ConfigurationManager.AppSettings["AdminPassword"]; } }
        public static string ApiEndpoint { get { return ConfigurationManager.AppSettings["Api.Endpoint.BaseUrl"]; } }

        #endregion

        [TestInitialize]
        public void TestInitialize()
        {
            // Set up a HTTP client
            HttpClientHelper = new HttpClient();
            HttpClientHelper.DefaultRequestHeaders.Accept.Clear();
            HttpClientHelper.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (HttpClientHelper != null)
                HttpClientHelper.Dispose();
        }

        // "api/v1/devices/{deviceId}"
        [TestMethod]
        public async Task DeviceLicenseServiceAdminRevoke_MissingDeviceId_ShouldThrowBadRequest()
        {
            var requestUri = string.Format("{0}/{1}/{2}", ApiEndpoint, LicenseService, _deviceId);

            //Attempt to revoke with insufficient params
            using (var req = new HttpRequestMessage(HttpMethod.Put, requestUri))
            {
                using (var content = new ObjectContent(typeof(DeviceLicenseRequest),
                    new DeviceLicenseRequest { RequestType = LicenseRequestType.RevokeLicense },
                    new JsonMediaTypeFormatter()))
                {
                    req.Content = content;
                    var response = await HttpClientHelper.SendAsync(req).ConfigureAwait(false);
                    Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
                }
            }
        }

        // "api/v1/devices/{deviceId}"
        [TestMethod]
        public async Task DeviceLicenseServiceAdminRevoke_NotLoggedInUser_ShouldThrowUnAuthorizedError()
        {
            var logoutUrl = string.Format("{0}/{1}", ApiEndpoint, AccountLogOut);
            var requestUri = string.Format("{0}/{1}/{2}", ApiEndpoint, LicenseService, _deviceId);

            //Logout
            using (var logoutRequest = new HttpRequestMessage(HttpMethod.Get, logoutUrl))
            {
                var logoutResponse = await HttpClientHelper.SendAsync(logoutRequest).ConfigureAwait(false);
                Assert.AreEqual(HttpStatusCode.OK, logoutResponse.StatusCode);
            }

            //Attempt to revoke when not authorized on portal
            var sb = new StringBuilder();
            var sw = new StringWriter(sb);
            using (var writer = new JsonTextWriter(sw))
            {
                writer.WriteStartObject();
                writer.WritePropertyName("deviceId");
                writer.WriteValue(_deviceId.ToString());
                writer.WritePropertyName("requestType");
                writer.WriteValue(RevokeLicenseTypeId);
                writer.WriteEndObject();

                Assert.IsNotNull(sb.ToString());
                var payload = new StringContent(sb.ToString(), Encoding.UTF8, "application/json");
                var response = await HttpClientHelper.PutAsync(requestUri, payload).ConfigureAwait(false);
                Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode);
            }
        }

        // "api/v1/devices/{deviceId}"
        [TestMethod]
        public async Task DeviceLicenseServiceAdminRevoke_LoggedInUser_LicenseNotExists_ReturnsOK()
        {
            var loginUrl = string.Format("{0}/{1}", ApiEndpoint, AccountLogin);
            var logoutUrl = string.Format("{0}/{1}", ApiEndpoint, AccountLogOut);
            var requestUri = string.Format("{0}/{1}/{2}", ApiEndpoint, LicenseService, _deviceId);

            //Logout
            using (var logoutRequest = new HttpRequestMessage(HttpMethod.Get, logoutUrl))
            {
                var logoutResponse = await HttpClientHelper.SendAsync(logoutRequest).ConfigureAwait(false);
                Assert.AreEqual(HttpStatusCode.OK, logoutResponse.StatusCode);
            }

            //Login
            var loginFormResponseText = await HttpClientHelper.GetStringAsync(loginUrl).ConfigureAwait(false);
            string suppliedAntiForgeryToken = ExtractAntiForgeryToken(loginFormResponseText);
            using (var loginRequest = new HttpRequestMessage(HttpMethod.Post, loginUrl))
            {
                var values = new Dictionary<string, string>();
                values.Add("Username", AdminUserName);
                values.Add("Password", AdminPassword);
                values.Add("__RequestVerificationToken", suppliedAntiForgeryToken);
                var content = new FormUrlEncodedContent(values);
                loginRequest.Content = content;
                var loginResponse = await HttpClientHelper.SendAsync(loginRequest).ConfigureAwait(false);
                Assert.AreEqual(HttpStatusCode.OK, loginResponse.StatusCode);
            }

            //Attempt to revoke once authorized on portal - silent failure as device license doesn't exist
            var sb = new StringBuilder();
            var sw = new StringWriter(sb);
            using (var writer = new JsonTextWriter(sw))
            {
                writer.WriteStartObject();
                writer.WritePropertyName("deviceId");
                writer.WriteValue(_deviceId.ToString());
                writer.WritePropertyName("requestType");
                writer.WriteValue(RevokeLicenseTypeId);
                writer.WriteEndObject();

                Assert.IsNotNull(sb.ToString());
                var payload = new StringContent(sb.ToString(), Encoding.UTF8, "application/json");
                var response = await HttpClientHelper.PutAsync(requestUri, payload).ConfigureAwait(false);
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                var result = await response.Content.ReadAsAsync<DeviceLicenseRequest>().ConfigureAwait(false);
                Assert.IsFalse(result.CanDownloadLearningContent);
            }

            //Logout
            using (var logoutRequest = new HttpRequestMessage(HttpMethod.Get, logoutUrl))
            {
                var logoutResponse = await HttpClientHelper.SendAsync(logoutRequest).ConfigureAwait(false);
                Assert.AreEqual(HttpStatusCode.OK, logoutResponse.StatusCode);
            }
        }

        // "api/v1/devices/{deviceId}"
        [TestMethod]
        public async Task DeviceLicenseServiceAdminRevoke_LoggedInUser_LicenseExists_ReturnsOK()
        {
            var loginUrl = string.Format("{0}/{1}", ApiEndpoint, AccountLogin);
            var logoutUrl = string.Format("{0}/{1}", ApiEndpoint, AccountLogOut);
            var requestUri = string.Format("{0}/{1}/{2}", ApiEndpoint, LicenseService, _deviceId);

            //Create TestData
            await CreateTestData().ConfigureAwait(false);

            //Login
            var loginFormResponseText = await HttpClientHelper.GetStringAsync(loginUrl).ConfigureAwait(false);
            string suppliedAntiForgeryToken = ExtractAntiForgeryToken(loginFormResponseText);
            using (var loginRequest = new HttpRequestMessage(HttpMethod.Post, loginUrl))
            {
                var values = new Dictionary<string, string>();
                values.Add("Username", AdminUserName);
                values.Add("Password", AdminPassword);
                values.Add("__RequestVerificationToken", suppliedAntiForgeryToken);
                var content = new FormUrlEncodedContent(values);
                loginRequest.Content = content;
                var loginResponse = await HttpClientHelper.SendAsync(loginRequest).ConfigureAwait(false);
                Assert.AreEqual(HttpStatusCode.OK, loginResponse.StatusCode);
            }

            //Attempt to revoke once authorized on portal - silent failure as device license doesn't exist
            var sb = new StringBuilder();
            var sw = new StringWriter(sb);
            using (var writer = new JsonTextWriter(sw))
            {
                writer.WriteStartObject();
                writer.WritePropertyName("deviceId");
                writer.WriteValue(_deviceId.ToString());
                writer.WritePropertyName("requestType");
                writer.WriteValue(RevokeLicenseTypeId);
                writer.WriteEndObject();

                Assert.IsNotNull(sb.ToString());
                var payload = new StringContent(sb.ToString(), Encoding.UTF8, "application/json");
                var response = await HttpClientHelper.PutAsync(requestUri, payload).ConfigureAwait(false);
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                var result = await response.Content.ReadAsAsync<DeviceLicenseRequest>().ConfigureAwait(false);
                Assert.IsFalse(result.CanDownloadLearningContent);
            }

            //Logout
            using (var logoutRequest = new HttpRequestMessage(HttpMethod.Get, logoutUrl))
            {
                var logoutResponse = await HttpClientHelper.SendAsync(logoutRequest).ConfigureAwait(false);
                Assert.AreEqual(HttpStatusCode.OK, logoutResponse.StatusCode);
            }

            //Cleanup TestData
            await DeleteTestData().ConfigureAwait(false);
        }

        private async Task CreateTestData()
        {
            //Insert test data
            //Device
            int rowDevice = await DataAccessHelper.ExecuteAsync(String.Format("INSERT INTO [dbo].[Device] (DeviceID) VALUES ('{0}')",
                _deviceId)).ConfigureAwait(false);
            
            //User
            string query = @"INSERT INTO [dbo].[User] (
                                 [UserID]
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
                new SqlParameter("@UserID", SqlDbType.UniqueIdentifier) { Value =  _userId},
                new SqlParameter("@Username", SqlDbType.Binary) { Value =  _usernameEnc.EncryptedValue.NullIfEmpty()},
                new SqlParameter("@UsernameHash", SqlDbType.Binary) { Value =  _usernameEnc.GetHashBytes().NullIfEmpty()},
                new SqlParameter("@UserType", SqlDbType.Binary) { Value =  _userTypeEnc.EncryptedValue.NullIfEmpty()},
                new SqlParameter("@UserTypeHash", SqlDbType.Binary) { Value =  _userTypeEnc.GetHashBytes().NullIfEmpty()},
            };
            int rowUser = await DataAccessHelper.ExecuteAsync(query, paramList).ConfigureAwait(false);

            //Accesspoint
            int rowAccesspoint = await DataAccessHelper.ExecuteAsync(string.Format(
                @"INSERT INTO [dbo].[AccessPoint] (
                        [WifiBSSID],
                        [WifiSSID]) 
                 VALUES ('{0}', 
                         '{1}')",
                    _wifiBssId,
                    WifiSSId)).ConfigureAwait(false);

            if (rowDevice > 0 && rowUser > 0 && rowAccesspoint > 0)
            {

                int rowLicenseRequest = await DataAccessHelper.ExecuteAsync(string.Format(
                    @"INSERT INTO [dbo].[LicenseRequest] (
                        [LicenseRequestID], 
                        [ConfigCode],
                        [LicenseRequestTypeID],
                        [DeviceID],
                        [WifiBSSID], 
                        [UserID], 
                        [RequestDateTime],
                        [Response],
                        [ResponseDateTime]) 
                      VALUES (
                        '{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}','{8}')", 
                        _licenseRequestId, 
                        ConfigCode,
                        RequestLicenseTypeId,
                        _deviceId, 
                        _wifiBssId, 
                        _userId, 
                        DateTime.UtcNow.AddHours(-2), 
                        "License Granted",
                        DateTime.UtcNow.AddHours(-2))).ConfigureAwait(false);

                if (rowLicenseRequest > 0)
                {
                    await DataAccessHelper.ExecuteAsync(string.Format(
                        @"INSERT INTO [dbo].[License] (
                            [LicenseRequestID], 
                            [ConfigCode] ,
                            [LicenseIssueDateTime],
                            [WifiBSSID], 
                            [LicenseExpiryDateTime]) 
                            VALUES (
                        '{0}', '{1}', '{2}', '{3}', '{4}')", 
                             _licenseRequestId, 
                             ConfigCode, 
                             DateTime.UtcNow.AddHours(-2),
                            _wifiBssId, 
                            DateTime.UtcNow.AddHours(-1))).ConfigureAwait(false);
                }
            }
        }

        private async Task DeleteTestData()
        {
            //Remove Test Data
            //License
            int rowLicense = await DataAccessHelper.ExecuteAsync(
                "DELETE FROM [dbo].[License] WHERE [LicenseRequestID] = '" + _licenseRequestId + "'").ConfigureAwait(false);
            //LicenseRequest
            int rowLicenseRequest = await DataAccessHelper.ExecuteAsync(
                "DELETE FROM [dbo].[LicenseRequest] WHERE [LicenseRequestID] = '" + _licenseRequestId + "'").ConfigureAwait(false);
          
            if (rowLicense > 0 && rowLicenseRequest > 0)
            {
                //Remove Reference Data
                //User
                await DataAccessHelper.ExecuteAsync(
                    "DELETE FROM [dbo].[User] WHERE [UserID] = '" + _userId + "'").ConfigureAwait(false);
                //Accesspoint
                await DataAccessHelper.ExecuteAsync(
                    "DELETE FROM [dbo].[AccessPoint] WHERE [WifiBSSID] = '" + _wifiBssId + "'").ConfigureAwait(false);
                //Device
                await DataAccessHelper.ExecuteAsync(
                    "DELETE FROM [dbo].[Device] WHERE [DeviceID] = '" + _deviceId + "'").ConfigureAwait(false);
            }
        }

        private static string ExtractAntiForgeryToken(string htmlResponseText)
        {
            if (htmlResponseText == null) throw new ArgumentNullException("htmlResponseText");

            Match match = Regex.Match(htmlResponseText, @"\<input name=""__RequestVerificationToken"" type=""hidden"" value=""([^""]+)"" \/\>");
            return match.Success ? match.Groups[1].Captures[0].Value : null;
        }

        private static string GetRandomString()
        {
            Random random = new Random();

            const string chars = "#-_abcdefgh12345678";
            var result = new string(
                Enumerable.Repeat(chars, 8)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());

            return result;
        }
    }
}
