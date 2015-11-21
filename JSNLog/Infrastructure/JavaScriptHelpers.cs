using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using JSNLog.ValueInfos;
using JSNLog.Exceptions;

namespace JSNLog.Infrastructure
{
    internal class JavaScriptHelpers
    {
        public static void WriteScriptTag(string url, StringBuilder sb)
        {
            sb.AppendLine("<script type=\"text/javascript\" src=\"" + url + "\"></script>");
        }

        public static void WriteJavaScriptBeginTag(StringBuilder sb)
        {
            sb.AppendLine("<script type=\"text/javascript\">");
            sb.AppendLine("//<![CDATA[");
        }

        public static void WriteJavaScriptEndTag(StringBuilder sb)
        {
            sb.AppendLine("//]]>");
            sb.AppendLine("</script>");
        }

        public static void WriteLine(string content, StringBuilder sb)
        {
            sb.AppendLine(content);
        }

        /// <summary>
        /// Generates the JavaScript to set options on an object
        /// </summary>
        /// <param name="parentName">
        /// Name of the JavaScript variable that holds the object.
        /// </param>
        /// <param name="element">
        /// The object (logger, etc.) whose fields are to be converted to options.
        /// </param>
        /// <param name="sb">
        /// The JavaScript code is added to this StringBuilder.
        /// </param>
        /// <param name="initialJsonFields">
        /// If not null, the fields in this array will be included in the JSON object passed to the setOptions method. 
        /// </param>
        internal static void GenerateSetOptions(string parentName, ICanCreateJsonFields element, 
            Dictionary<string, string> appenderNames, Func<string, string> virtualToAbsoluteFunc, 
            StringBuilder sb, IList<string> initialJsonFields = null)
        {
            var jsonFields = new List<string>();
            
            if (initialJsonFields != null) 
            {
                jsonFields.AddRange(initialJsonFields);
            }

            element.AddJsonFields(jsonFields, appenderNames, virtualToAbsoluteFunc);

            string setOptionsJS = string.Format("{0}.setOptions({{{1}}});", parentName, string.Join(",\n", jsonFields));
            sb.AppendLine(setOptionsJS);
        }

        internal static string ToJavaScript(bool b)
        {
            if (b) { return "true"; }
            return "false";
        }

        internal static string ToJavaScript(uint i)
        {
            return i.ToString();
        }

        internal static string ToJavaScript(IEnumerable<string> jsValues)
        {
            return "[" + string.Join(", ", jsValues) + "]";
        }

        /// <summary>
        /// Creates a JSON field of the form:
        /// "field name": "field value"
        /// 
        /// This string is added to jsonFields.
        /// 
        /// If value is null or empty, nothing is added.
        /// </summary>
        /// <param name="jsonFields"></param>
        /// <param name="jsonFieldName">
        /// Name of the field, without quotes. Will not be escaped.
        /// </param>
        /// <param name="value">
        /// The unescaped value.
        /// </param>
        /// <param name="valueInfo">
        /// Used to validate the value, and to convert it to proper JavaScript.
        /// </param>
        internal static void AddJsonField(IList<string> jsonFields, string jsonFieldName, string value, IValueInfo valueInfo) 
        {
            // null - the string has not been set in the config
            // "" - the string has been set in the config to "" (a valid value)
            if (value == null) { return; }

            try
            {
                AddJsonField(jsonFields, jsonFieldName, valueInfo.ToJavaScript(value));
            }
            catch (Exception e)
            {
                throw new PropertyException(jsonFieldName, e);
            }
        }

        internal static void AddJsonField(IList<string> jsonFields, string jsonFieldName, uint value)
        {
            AddJsonField(jsonFields, jsonFieldName, ToJavaScript(value));
        }

        internal static void AddJsonField(IList<string> jsonFields, string jsonFieldName, bool value)
        {
            AddJsonField(jsonFields, jsonFieldName, ToJavaScript(value));
        }

        // valueInfo will be applied to each individual string in value
        internal static void AddJsonField(IList<string> jsonFields, string jsonFieldName, IEnumerable<string> value, IValueInfo valueInfo)
        {
            try
            {
                // If an element in value is null, don't include it in the field. This can be used to create an empty array.
                AddJsonField(jsonFields, jsonFieldName, ToJavaScript(value.Where(v=>v != null).Select(v=>valueInfo.ToJavaScript(v))));
            }
            catch (Exception e)
            {
                throw new PropertyException(jsonFieldName, e);
            }
        }

        internal static void AddJsonField(IList<string> jsonFields, string jsonFieldName, string jsValue)
        {
            // Note: no quotes around {1}. If jsValue represents a string, it will already be quoted.
            string jsonField = string.Format("\"{0}\": {1}", jsonFieldName, jsValue);
            jsonFields.Add(jsonField);
        }

        /// <summary>
        /// Generates the JavaScript create an object.
        /// </summary>
        /// <param name="objectVariableName"></param>
        /// <param name="createMethodName"></param>
        /// <param name="name">
        /// Name of the object as known to the user. For example the appender name.
        /// </param>
        /// <param name="sb"></param>
        public static void GenerateCreate(string objectVariableName, string createMethodName, string name, StringBuilder sb)
        {
            JavaScriptHelpers.WriteLine(string.Format("var {0}=JL.{1}('{2}');", objectVariableName, createMethodName, name), sb);
        }

        /// <summary>
        /// Generate the JavaScript to create a logger. 
        /// </summary>
        /// <param name="loggerVariableName">
        /// New logger object will be assigned to this JS variable.
        /// </param>
        /// <param name="loggerName">
        /// Name of the logger. Could be null (for the root logger).
        /// </param>
        /// <param name="sb">
        /// JS code will be appended to this.
        /// </param>
        public static void GenerateLogger(string loggerVariableName, string loggerName, StringBuilder sb)
        {
            string quotedLoggerName =
                loggerName == null ? "" : @"""" + loggerName + @"""";
            JavaScriptHelpers.WriteLine(string.Format("var {0}=JL({1});", loggerVariableName, quotedLoggerName), sb);
        }
    }
}
