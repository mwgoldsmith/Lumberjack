using System;
using System.Collections.Generic;
using System.Linq;

namespace Medidata.Lumberjack.Core
{
    /// <summary>
    /// 
    /// </summary>
    internal static class Extensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        internal static bool In<T>(this T source, params T[] list) {
            if (Equals(source, default(T))) {
                throw new ArgumentNullException();
            }

            return list.Contains(source);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="items"></param>
        internal static void Remove<T>(this List<T> source, T[] items) {
            if (source == null) {
                throw new ArgumentNullException();
            }

            source.RemoveAll(x => Array.IndexOf(items, x) != -1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="source"></param>
        /// <param name="items"></param>
        internal static void Remove<T, U>(this Dictionary<T, U> source, T[] items) {
            if (source == null) {
                throw new ArgumentNullException();
            }

            foreach (var t in items) {
                source.Remove(t);
            }
        }
    }
}
