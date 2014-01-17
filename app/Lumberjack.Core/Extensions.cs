using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
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
        internal static bool In<T>(this T source, params T[] list)
            where T : IList, IDictionary, IOrderedDictionary {
            if (Equals(source, default(T)))
                throw new ArgumentNullException("source");

            return list.Contains(source);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="items"></param>
        internal static void Remove<T>(this List<T> source, T[] items) {
            if (source == null)
                throw new ArgumentNullException();

            source.RemoveAll(x => Array.IndexOf(items, x) != -1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="source"></param>
        /// <param name="items"></param>
        internal static void Remove<TKey, TValue>(this Dictionary<TKey, TValue> source, TKey[] items) {
            if (source == null) 
                throw new ArgumentNullException("source");

            foreach (var t in items) 
                source.Remove(t);
        }
    }
}
