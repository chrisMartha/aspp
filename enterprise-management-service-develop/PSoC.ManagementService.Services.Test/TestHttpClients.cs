using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using PSoC.ManagementService.Services.Interfaces;

namespace PSoC.ManagementService.Services.Test
{
    public class TestHttpClient<T> : IHttpWrapper
    {
        private string _result;
        public TestHttpClient(string result)
        {
            _result = result;
        }

        public Task<string> GetStringAsync(string requestUri, Dictionary<string, string> headers = null)
        {
            throw new NotImplementedException();
        }

        public Task<System.IO.Stream> GetStreamAsync(string requestUri, Dictionary<string, string> headers = null)
        {
            throw new NotImplementedException();
        }

        public Task<string> PostJsonAsync<T>(string requestUri, T data, Dictionary<string, string> headers = null)
        {
            throw new NotImplementedException();
        }

        public Task<string> PostAsync(string requestUri, HttpContent content, Dictionary<string, string> headers = null)
        {
            return Task.FromResult(_result);
        }

        public Task<string> PutJsonAsync<T>(string requestUri, T data, Dictionary<string, string> headers = null)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {

        }
    }

    public class TestExceptionHttpClient : IHttpWrapper
    {
        private readonly Exception _ex;

        public TestExceptionHttpClient(Exception ex)
        {
            _ex = ex;
        }

        public Task<string> GetStringAsync(string requestUri, Dictionary<string, string> headers = null)
        {
            throw _ex;
        }

        public Task<System.IO.Stream> GetStreamAsync(string requestUri, Dictionary<string, string> headers = null)
        {
            throw _ex;
        }

        public Task<string> PostJsonAsync<T>(string requestUri, T data, Dictionary<string, string> headers = null)
        {
            throw _ex;
        }

        public Task<string> PostAsync(string requestUri, HttpContent content, Dictionary<string, string> headers = null)
        {
            throw _ex;
        }

        public Task<string> PutJsonAsync<T>(string requestUri, T data, Dictionary<string, string> headers = null)
        {
            throw _ex;
        }

        public void Dispose()
        {

        }
    }
}
