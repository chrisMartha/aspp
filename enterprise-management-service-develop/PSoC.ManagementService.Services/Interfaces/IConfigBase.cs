
namespace PSoC.ManagementService.Services.Interfaces
{
    public interface IConfigBase
    {
        T GetApplicationSetting<T>(string key);
        T GetApplicationSetting<T>(string Key, T defaultValue);
        T GetApplicationSetting<T>(string Key, bool isRequired);
        T GetApplicationSetting<T>(string Key, T defaultValue, bool isRequired);
    }
}
