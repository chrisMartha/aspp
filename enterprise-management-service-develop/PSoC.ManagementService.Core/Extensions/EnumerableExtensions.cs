using System;
using System.Collections.Generic;
using System.Linq;

namespace PSoC.ManagementService.Core.Extensions
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Returns true if given IEnumerable T is not null and has atleast one element otherwise it returns false
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        public static bool HasElements<T>(this IEnumerable<T> items)
        {
            return (items != null && items.Any());
        }

      /// <summary>
      /// Returns true if given IEnumerable T is not null and has atleast n elements otherwise it returns false
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="items"></param>
      /// <param name="n"></param>
      /// <returns></returns>
        public static bool HasAtleastNElements<T>(this IEnumerable<T> items, int n)
        {
            return (items != null && items.Take(n).Count() == n);
        }

        /// <summary>
        /// Iterates through an IEnumerable<T> and applies an action
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="action"></param>
        public static void Each<T>(this IEnumerable<T> items, Action<T> action)
        {
            if (items == null) return;
            var contents = items;
            foreach (var item in contents)
            {
                action(item);
            }
        }
    }
}
