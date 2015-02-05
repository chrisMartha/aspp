using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace PSoC.ManagementService.Services.Interfaces
{
    public interface IHttpWrapper : IDisposable
    {
        Task<string> GetStringAsync(string requestUri, Dictionary<string, string> headers = null);
        Task<Stream> GetStreamAsync(string requestUri, Dictionary<string, string> headers = null);
        Task<string> PostJsonAsync<T>(string requestUri, T data, Dictionary<string, string> headers = null);
        Task<string> PostAsync(string requestUri, HttpContent content, Dictionary<string, string> headers = null);
        Task<string> PutJsonAsync<T>(string requestUri, T data, Dictionary<string, string> headers = null);
    }
}
