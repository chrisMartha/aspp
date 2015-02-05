using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using PSoC.ManagementService.Services.Interfaces;
using PSoC.ManagementService.Services.Logging;

namespace PSoC.ManagementService.Services
{
    public class HttpWrapper : IHttpWrapper
    {
        private HttpClient _client;

        public HttpWrapper()
        {
            _client = new HttpClient();
        }

        public async Task<string> GetStringAsync(string requestUri, Dictionary<string, string> headers = null)
        {
            var logRequest = new LogRequest { HttpMethod = HttpMethod.Get, Url = requestUri };
            try
            {
                if (string.IsNullOrEmpty(requestUri))
                {
                    throw new ArgumentNullException("Null or empty request uri");
                }

                _client.DefaultRequestHeaders.Clear();
                if (headers != null)
                {
                    foreach (var item in headers)
                    {
                        _client.DefaultRequestHeaders.Add(item.Key, item.Value);
                    }
                }

                var result = await _client.GetStringAsync(requestUri).ConfigureAwait(false); 
                return result;
            }
            catch (Exception ex)
            {
                logRequest.Exception = ex;
                PEMSEventSource.Log.ApplicationException(ex.Message, logRequest);
                return null;
            }
        }


        public async Task<Stream> GetStreamAsync(string requestUri, Dictionary<string, string> headers = null)
        {
            var logRequest = new LogRequest { HttpMethod = HttpMethod.Get, Url = requestUri };
            try
            {
                if (string.IsNullOrEmpty(requestUri))
                {
                    throw new ArgumentNullException("Null or empty request uri");
                }

                _client.DefaultRequestHeaders.Clear();
                if (headers != null)
                {
                    foreach (var item in headers)
                    {
                        _client.DefaultRequestHeaders.Add(item.Key, item.Value);
                    }
                }

                var result = await _client.GetStreamAsync(requestUri).ConfigureAwait(false);
                return result;
            }
            catch (Exception ex)
            {
                logRequest.Exception = ex;
                PEMSEventSource.Log.ApplicationException(ex.Message, logRequest);
                return null;
            }
        }

        public async Task<string> PostJsonAsync<T>(string requestUri, T data, Dictionary<string, string> headers = null)
        {
            var logRequest = new LogRequest { HttpMethod = HttpMethod.Post, Url = requestUri };
            try
            {
                if (string.IsNullOrEmpty(requestUri))
                {
                    throw new ArgumentNullException("Null or empty request uri");
                }

                _client.DefaultRequestHeaders.Clear();
                if (headers != null)
                {
                    foreach (var item in headers)
                    {
                        _client.DefaultRequestHeaders.Add(item.Key, item.Value);
                    }
                }

                var response = await _client.PostAsJsonAsync(requestUri, data).ConfigureAwait(false);
                logRequest.HttpStatusCode = response.StatusCode;
                response.EnsureSuccessStatusCode();
                var resultString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return resultString;
            }
            catch (Exception ex)
            {
                logRequest.Exception = ex;
                PEMSEventSource.Log.ApplicationException(ex.Message, logRequest);
                return null;
            }
        }

        public async Task<string> PutJsonAsync<T>(string requestUri, T data, Dictionary<string, string> headers = null)
        {
            var logRequest = new LogRequest { HttpMethod = HttpMethod.Put, Url = requestUri };
            try
            {
                if (string.IsNullOrEmpty(requestUri))
                {
                    throw new ArgumentNullException("Null or empty request uri");
                }

                _client.DefaultRequestHeaders.Clear();
                if (headers != null)
                {
                    foreach (var item in headers)
                    {
                        _client.DefaultRequestHeaders.Add(item.Key, item.Value);
                    }
                }

                var response = await _client.PutAsJsonAsync(requestUri, data).ConfigureAwait(false);
                logRequest.HttpStatusCode = response.StatusCode;
                response.EnsureSuccessStatusCode();
                var resultString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return resultString;
            }
            catch (Exception ex)
            {
                logRequest.Exception = ex;
                PEMSEventSource.Log.ApplicationException(ex.Message, logRequest);
                return null;
            }
        }

        public async Task<string> PostAsync(string requestUri, HttpContent content, Dictionary<string, string> headers = null)
        {
            var logRequest = new LogRequest{ HttpMethod = HttpMethod.Post, Url = requestUri };
            try
            {
                if (string.IsNullOrEmpty(requestUri))
                {
                    throw new ArgumentNullException("Null or empty request uri");
                }

                _client.DefaultRequestHeaders.Clear();
                if (headers != null)
                {
                    foreach (var item in headers)
                    {
                        _client.DefaultRequestHeaders.Add(item.Key, item.Value);
                    }
                }

                var response = await _client.PostAsync(requestUri, content).ConfigureAwait(false);
                logRequest.HttpStatusCode = response.StatusCode;
                response.EnsureSuccessStatusCode();
                var resultString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return resultString;
            }
            catch (Exception ex)
            {
                logRequest.Exception = ex;
                PEMSEventSource.Log.ApplicationException(ex.Message, logRequest);
                return null;
            }
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
