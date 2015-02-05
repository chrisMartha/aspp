using PSoC.ManagementService.Services.Interfaces;

namespace PSoC.ManagementService.Services
{
    public class HttpClientFactory : IHttpClientFactory
    {
        public HttpClientFactory()
        {
        }

        public IHttpWrapper CreateHttpClient()
        {
            return new HttpWrapper();
        }
    }
}
