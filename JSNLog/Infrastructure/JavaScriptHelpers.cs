using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace JSNLog.Infrastructure
{
    public class JavaScriptHelpers
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
        /// Use this for tags that have 1 attribute called "value"
        /// </summary>
        /// <param name="xe">
        /// XmlElement of the tag
        /// </param>
        /// <param name="parentName">
        /// Name of the JavaScript variable that refers to the parent (such as a logger).
        /// </param>
        /// <param name="validValueRegex">
        /// Used to validate the value
        /// </param>
        /// <param name="js">
        /// Template of the JavaScript to be appended to sb.
        /// {0} gets replaced by the parents variable name.
        /// {1} gets replaced by the value.
        /// </param>
        /// <param name="sb">
        /// The JavaScript gets appended to this.
        /// </param>
        public static void ProcessTagWithValue(
            XmlElement xe, string parentName, string validValueRegex, string js, StringBuilder sb)
        {
            if (xe == null) { return; }

            string value = XmlHelpers.RequiredAttribute(xe, "value", validValueRegex);

            string content = string.Format(js, parentName, value);

            WriteLine(content, sb);
        }

        /// <summary>
        /// Generates the JavaScript for an appender.
        /// </summary>
        /// <param name="appenderVariableName"></param>
        /// <param name="appenderUrl"></param>
        /// <param name="sb"></param>
        public static void GenerateAppender(string appenderVariableName, string appenderUrl, StringBuilder sb)
        {
            JavaScriptHelpers.WriteLine(string.Format("var {0}=new jsnlog.AjaxAppender(\"{1}\")", appenderVariableName, appenderUrl), sb);
            JavaScriptHelpers.WriteLine(string.Format("{0}.setLayout(new jsnlog.JsonLayout(false, false));", appenderVariableName), sb);
        }

        public static void AddAppenderToLogger(string loggerVariableName, string appenderVariableName, StringBuilder sb)
        {
            JavaScriptHelpers.WriteLine(string.Format("{0}.addAppender({1});", loggerVariableName, appenderVariableName), sb);
        }

    }
}
