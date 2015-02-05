
namespace PSoC.ManagementService.Services.Interfaces
{
    public interface IHttpClientFactory
    {
        IHttpWrapper CreateHttpClient();
    }
}
