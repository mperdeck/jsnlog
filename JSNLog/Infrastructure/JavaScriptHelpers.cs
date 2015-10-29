using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

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
        /// Generates the JavaScript for a JSON object.
        /// </summary>
        /// <param name="optionValues"></param>
        /// <returns>
        /// JS code with the JSON object.
        /// </returns>
        public static string GenerateJson(AttributeValueCollection optionValues)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("{");

            bool firstItem = true;
            foreach (KeyValuePair<string, Value> option in optionValues)
            {
                string jsValue = null;

                // Do not test for IsNullOrEmpty. For example, appenders="" is legitimate (use 0 appenders)
                if (option.Value.Text != null)
                {
                    jsValue = option.Value.ValueInfo.ToJavaScript(option.Value.Text);
                }
                else if (option.Value.TextCollection != null)
                {
                    jsValue = "[" + String.Join(",", option.Value.TextCollection.Select(t => option.Value.ValueInfo.ToJavaScript(t))) + "]";
                }
                else
                {
                    continue;
                }

                sb.AppendFormat("{0}\"{1}\": {2}", firstItem ? "" : ", ", option.Key, jsValue);
                firstItem = false;
            }

            sb.Append("}");

            return sb.ToString();
        }

        /// <summary>
        /// Generates the JavaScript to set options on an object
        /// </summary>
        /// <param name="parentName">
        /// Name of the JavaScript variable that holds the object.
        /// </param>
        /// <param name="optionValues">
        /// The names and values of the options.
        /// </param>
        /// <param name="sb">
        /// The JavaScript code is added to this StringBuilder.
        /// </param>
        public static void GenerateSetOptions(string parentName, AttributeValueCollection optionValues, StringBuilder sb)
        {
            string optionsJson = GenerateJson(optionValues);
            sb.AppendLine(string.Format("{0}.setOptions({1});", parentName, optionsJson));
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
