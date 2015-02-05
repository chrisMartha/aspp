using System.Configuration;

namespace PSoC.ManagementService.Core
{
    public static class GlobalAppSettings
    {
        /// <summary>
        /// Gets the configuration value for the specified key as string by default. An exception is thrown if missing/empty.
        /// </summary>
        /// <param name="keyName">Name of the key in app/web config file</param>
        /// <exception cref="ConfigurationErrorsException">The value is not specified, or is blank.</exception>
        /// <returns>Returns value for the key</returns>
        public static string GetValue(string keyName)
        {
            string str = ConfigurationManager.AppSettings[keyName];
            if (str == null || str.Trim().Length == 0)
                throw new ConfigurationErrorsException("No application setting available for key: " + keyName);
            return str;
        }

        /// <summary>
        /// Gets the configuration value for the specified key, parsed as int. An exception is thrown if missing/empty.
        /// </summary>
        /// <param name="keyName">Name of the key in app/web config file</param>
        /// <exception cref="ConfigurationErrorsException">The value is not specified or cannot be parsed as in int.</exception>
        /// <returns>Returns value for the key</returns>
        public static int GetValueAsInt(string keyName)
        {
            string str = GetValue(keyName);
            int value;
            if (!int.TryParse(str, out value))
            {
                string message = string.Format("Unable to parse app setting value for {0} as an int: {1}", keyName, str);
                throw new ConfigurationErrorsException(message);
            }
            return value;
        }

    }
}
