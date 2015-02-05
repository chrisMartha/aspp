using System;
using System.Configuration;
using Microsoft.WindowsAzure;
using PSoC.ManagementService.Services.Interfaces;

namespace PSoC.ManagementService.Services
{
    public class ConfigBase : IConfigBase
    {
        public T GetApplicationSetting<T>(string key)
        {
            return GetApplicationSetting(key, default(T), false);
        }

        public T GetApplicationSetting<T>(string key, T defaultValue)
        {
            return GetApplicationSetting(key, defaultValue, false);
        }

        public T GetApplicationSetting<T>(string key, bool isRequired)
        {
            return GetApplicationSetting(key, default(T), isRequired);
        }

        public T GetApplicationSetting<T>(string key, T defaultValue, bool isRequired)
        {
            try
            {
                var stringValue = GetString(key);

                if (typeof(T) == typeof(string) || typeof(T) == typeof(String)) 
                {
                    return (T)((object)stringValue);
                }

                if (typeof(T) == typeof(Int32) || typeof(T) == typeof(int)) 
                { 
                    return (T)((object)Int32.Parse(stringValue)); 
                }

                if (typeof(T) == typeof(Int64) || typeof(T) == typeof(long)) 
                {
                    return (T)((object)Int64.Parse(stringValue)); 
                }

                if (typeof(T) == typeof(bool) || typeof(T) == typeof(Boolean)) 
                {
                    return (T)((object)Boolean.Parse(stringValue)); 
                }

                throw new NotSupportedException(string.Format("Haven't supported to parse an appsetting as {0} yet.", typeof(T)));
            }
            catch (NotSupportedException)
            {
                throw;
            }
            catch (ConfigurationErrorsException)
            {
                if (isRequired) { throw; }
                return defaultValue;
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        private string GetString(string key)
        {
            string value = CloudConfigurationManager.GetSetting(key);
            if (value == null) 
            { 
                throw new ConfigurationErrorsException("Config missing for : " + key); 
            }

            return value;
        }
    }
}
