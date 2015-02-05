using System;
using System.ComponentModel;
using System.Reflection;

namespace PSoC.ManagementService.Core.Extensions
{
    public static class Enum<T>
    {
        /// <summary>
        /// Gets the description attribute value for an enum value
        /// </summary>
        /// <param name="value"></param>
        public static string GetDescription(T value)
        {
            FieldInfo info = value.GetType().GetField(value.ToString());
            var attributes =
                (DescriptionAttribute[]) info.GetCustomAttributes(typeof (DescriptionAttribute), false);
            return (attributes.Length > 0 ? attributes[0].Description : value.ToString());
        }

        /// <summary>
        /// Parses string into an Enum of T
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T Parse(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }
    }
}
