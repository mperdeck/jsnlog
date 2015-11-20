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
        /// Generates the JavaScript for the call to setOptions to set the options for an object
        /// (the JL object itself, an appender or a logger).
        /// </summary>
        /// <param name="parentName">
        /// JavaScript variable that holds the element.
        /// </param>
        /// <param name="xe">
        /// XML element. The attributes on this element will provide the values for the options.
        /// </param>
        /// <param name="attributeInfos">
        /// Describes which attributes to use as options and how to validate them.
        /// 
        /// As regards attributeInfos that have a SubTagName:
        /// * The value of such an attribute is an array, for example [ 'a', 'b' ]
        /// * If there are no child elements with the given sub tag name, there is no value, and no entry for that attributeinfo in 
        ///   the generated setOption.
        /// * If there is only one child element and it does not have an attribute, the value is an empty array [].
        /// </param>
        /// <param name="initialAttributeValues">
        /// Initial attribute values. The elements found in xe will be added to this.
        /// If null, this method will create an empty collection itself.
        /// </param>
        /// <param name="sb">
        /// The JS code is added to this.
        /// </param>
        /// <param name="validate">
        /// If not null, this method is called on the generated attribute values. This can be used to throw an exception
        /// if the given parameters are not valid.
        /// </param>
        public static void ProcessOptionAttributes(string parentName, XmlElement xe, IEnumerable<AttributeInfo> attributeInfos,
            AttributeValueCollection initialAttributeValues, StringBuilder sb,
            Action<AttributeValueCollection> validate = null)
        {
            var attributeValues = initialAttributeValues ?? new AttributeValueCollection();
            XmlHelpers.ProcessAttributes(xe, attributeInfos, attributeValues);

            if (validate != null) { validate(attributeValues); }

            JavaScriptHelpers.GenerateSetOptions2(parentName, attributeValues, sb);
        }

        /// <summary>
        /// Takes a DateTime in UTC and returns the same timestamp in local time.
        /// </summary>
        /// <param name="utcTime"></param>
        /// <returns></returns>
        public static DateTime UtcToLocalDateTime(DateTime utcTime)
        {
            TimeZoneInfo localZone = TimeZoneInfo.Local;
            DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, localZone);
            return localTime;
        }

        public static Dictionary<string, string> ToDictionary(NameValueCollection nameValueCollection)
        {
            var result = new Dictionary<string, string>();

            foreach (string key in nameValueCollection.AllKeys)
            {
                result[key] = nameValueCollection[key];
            }

            return result;
        }

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
