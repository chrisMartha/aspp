using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PSoC.ManagementService.Services.Interfaces;
using PSoC.ManagementService.Services.Logging;
using PSoC.ManagementService.Services.Models.Schoolnet;

namespace PSoC.ManagementService.Services
{
    public class SchoolnetService : ISchoolnetService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public SchoolnetService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<bool> IsAuthorizedAsync(string restUrl, string clientId, string clientSecret, string userName,
            string password)
        {
            var logRequest = new LogRequest();
            try
            {
                if (string.IsNullOrEmpty(restUrl) || string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret)
               || string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
                {
                    throw new ArgumentNullException("A required parameter was null or empty");
                }

                var url = string.Format("{0}{1}", restUrl, @"api/oauth/token");
                logRequest.Url = url;
                logRequest.HttpMethod = HttpMethod.Post;

                PEMSEventSource.Log.SchoolnetAuthorizationRequested(url, logRequest);

                var paramList = new Dictionary<string, string>
                {
                    {"grant_type", "password"},
                    {"client_id", clientId.ToUpper()}, //Case Sensitive
                    {"client_secret", clientSecret.ToUpper()},
                    {"username", userName},
                    {"password", password}
                };

                using (var client = _httpClientFactory.CreateHttpClient())
                {
                    var result = await client.PostAsync(url, new FormUrlEncodedContent(paramList)).ConfigureAwait(false);
                    if (result == null)
                    {
                        PEMSEventSource.Log.SchoolnetAuthorizationFailure("Post request returned null response", null, logRequest);
                        return false;
                    }

                    var accessToken = JsonConvert.DeserializeObject<AccessToken>(result);
                    if (accessToken == null || accessToken.access_token == null)
                    {
                        PEMSEventSource.Log.SchoolnetAuthorizationFailure("Missing or incorrect access token", result, logRequest);
                        return false;
                    }

                    PEMSEventSource.Log.SchoolnetAuthorizationSucceeded();
                    return true;
                }
            }
            catch (Exception ex)
            {
                logRequest.Exception = ex;
                PEMSEventSource.Log.SchoolnetAuthorizationFailure(ex.Message, null, logRequest);
                return false;
            }
        }
    }
}
