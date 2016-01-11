using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Collections.Specialized;

namespace JSNLog.Infrastructure
{
    internal static class Utils
    {
        /// <summary>
        /// Takes a DateTime in UTC and returns the same timestamp in local time.
        /// </summary>
        /// <param name="utcTime"></param>
        /// <returns></returns>
        public static DateTime UtcToLocalDateTime(DateTime utcTime)
        {
            DateTime localTime = utcTime.ToLocalTime();
            return localTime;
        }

#if NET40
        // NameValueCollection is unknown in DNX environments

        public static Dictionary<string, string> ToDictionary(NameValueCollection nameValueCollection)
        {
            var result = new Dictionary<string, string>();

            foreach (string key in nameValueCollection.AllKeys)
            {
                result[key] = nameValueCollection[key];
            }

            return result;
        }
#endif

        public static Dictionary<string, string> ToDictionary(IEnumerable<KeyValuePair<string, string>> nameValueCollection)
        {
            var result = new Dictionary<string, string>();

            foreach (var kvp in nameValueCollection)
            {
                result[kvp.Key] = kvp.Value;
            }

            return result;
        }

        public static T SafeGet<K, T>(this IDictionary<K, T> dictionary, K key)
        {
            T value;
            dictionary.TryGetValue(key, out value);
            return value;
        }

        public static string SafeToString<T>(this T obj)
        {
            if (obj == null)
            {
                return "";
            }

            return obj.ToString();
        }

        /// <summary>
        /// The given url may be virtual (starts with ~). This method returns a version of the url that is not virtual.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string AbsoluteUrl(string url, Func<string, string> virtualToAbsoluteFunc)
        {
            string urlLc = url.ToLower();
            if (urlLc.StartsWith("//") || urlLc.StartsWith("http://") || urlLc.StartsWith("https://"))
            {
                return url;
            }

            string absoluteUrl = virtualToAbsoluteFunc(url);
            return absoluteUrl;
        }
    }
}
