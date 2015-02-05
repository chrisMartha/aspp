using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using PSoC.ManagementService.Services.Interfaces;
using PSoC.ManagementService.Services.Models.Schoolnet;

namespace PSoC.ManagementService.Services.Test
{
    [TestClass]
    public class SchoolnetServiceTests
    {
        private SchoolnetService _schoolnetService;
        private Mock<IHttpClientFactory> _mockHttpClientFactory;
        private const string OAuthClientId = "42b1233a-2020-4bb2-8f5f-78daff0cb84d";
        private const string OAuthSecret = "c23cd56c-0fd0-4de6-8b8f-97e4332d9eea";

        [TestInitialize]
        public void TestInitialize()
        {
            _mockHttpClientFactory = new Mock<IHttpClientFactory>();
            _schoolnetService = new SchoolnetService(_mockHttpClientFactory.Object);
        }

        #region IsAuthorizedAsync

        [TestMethod]
        public async Task IsAuthorizedAsync_InvalidRestUrl()
        {
            var result = await _schoolnetService.IsAuthorizedAsync(null, OAuthClientId, OAuthSecret, "abc", "def");
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task IsAuthorizedAsync_InvalidClientId()
        {
            var result = await _schoolnetService.IsAuthorizedAsync("def", null, OAuthSecret, "abc", "def");
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task IsAuthorizedAsync_InvalidClientSecret()
        {
            var result = await _schoolnetService.IsAuthorizedAsync("def", OAuthClientId, null, "abc", "def");
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task IsAuthorizedAsync_InvalidUsername()
        {
            var result = await _schoolnetService.IsAuthorizedAsync("def", OAuthClientId, OAuthSecret, null, "def");
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task IsAuthorizedAsync_InvalidPassword()
        {
            var result = await _schoolnetService.IsAuthorizedAsync("def", OAuthClientId, OAuthSecret, "abc", null);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task IsAuthorizedAsync_PostException()
        {
            var client = new TestExceptionHttpClient(new Exception());
            _mockHttpClientFactory.Setup(x => x.CreateHttpClient()).Returns(client);

            var result = await _schoolnetService.IsAuthorizedAsync("def", OAuthClientId, OAuthSecret, "abc", "xyz");

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task IsAuthorizedAsync_PostNullReturn()
        {
            var client = new TestHttpClient<string>(null);
            _mockHttpClientFactory.Setup(x => x.CreateHttpClient()).Returns(client);

            var result = await _schoolnetService.IsAuthorizedAsync("def", OAuthClientId, OAuthSecret, "abc", "xyz");

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task IsAuthorizedAsync_InvalidAccessTokenReturned()
        {
            var accessToken = new AccessToken { access_token = null };
            var accessTokenJson = JsonConvert.SerializeObject(accessToken);
            var client = new TestHttpClient<string>(accessTokenJson);
            _mockHttpClientFactory.Setup(x => x.CreateHttpClient()).Returns(client);

            var result = await _schoolnetService.IsAuthorizedAsync("def", OAuthClientId, OAuthSecret, "abc", "xyz");

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task IsAuthorizedAsync_Success()
        {
            var accessToken = new AccessToken { access_token = "abcdefg" };
            var accessTokenJson = JsonConvert.SerializeObject(accessToken);
            var client = new TestHttpClient<string>(accessTokenJson);
            _mockHttpClientFactory.Setup(x => x.CreateHttpClient()).Returns(client);

            var result = await _schoolnetService.IsAuthorizedAsync("def", OAuthClientId, OAuthSecret, "abc", "xyz");

            Assert.IsTrue(result);
        }

        #endregion
    }
}
