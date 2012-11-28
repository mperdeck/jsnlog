using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Web.Configuration;
using JSNLog.Exceptions;
using JSNLog.Infrastructure;
using System.Text.RegularExpressions;
using System.Web;

namespace JSNLog
{
    public class JavascriptLogging
    {
        private const string JsLoggerVariable = "logger";
        private const string JsRootLoggerVariable = "root";
        private const string JsAppenderVariablePrefix = "a";
        private const string AppenderDefaultUrl = "jsnlog.logger";
        private const string DefaultAppenderVariable = "defaultAppender";
        private const string AdditivityDefaultValue = "true";


        // enabled, logger js file, stub js file (empty if no need to generate script tag; must be in same domain)
        private static void ProcessRoot(XmlElement xe, StringBuilder sb)
        {
            string loggerProductionLibraryVirtualPath = XmlHelpers.OptionalAttribute(xe, "productionLibraryPath", "");
            string loggerStubLibraryVirtualPath = XmlHelpers.OptionalAttribute(xe, "stubPath", "");
            bool loggerEnabled = bool.Parse(XmlHelpers.OptionalAttribute(xe, "enabled", "true", Constants.RegexBool));

            string loggerProductionLibraryPath = null;
            if (!string.IsNullOrEmpty(loggerProductionLibraryVirtualPath))
            {
                loggerProductionLibraryPath = VirtualPathUtility.ToAbsolute(loggerProductionLibraryVirtualPath);
            }

            string loggerStubLibraryPath = null;
            if (!string.IsNullOrEmpty(loggerStubLibraryVirtualPath))
            {
                loggerStubLibraryPath = VirtualPathUtility.ToAbsolute(loggerStubLibraryVirtualPath);
            }

            if (!loggerEnabled)
            {
                if (!string.IsNullOrWhiteSpace(loggerStubLibraryPath))
                {
                    JavaScriptHelpers.WriteScriptTag(loggerStubLibraryPath, sb);
                }
                else if (!string.IsNullOrWhiteSpace(loggerProductionLibraryPath))
                {
                    JavaScriptHelpers.WriteScriptTag(loggerProductionLibraryPath, sb);
                }

                JavaScriptHelpers.WriteJavaScriptBeginTag(sb);
                JavaScriptHelpers.WriteLine("var jsnlog_disabled = true;", sb);
                JavaScriptHelpers.WriteJavaScriptEndTag(sb);

                return;
            }

            if (!string.IsNullOrWhiteSpace(loggerProductionLibraryPath))
            {
                JavaScriptHelpers.WriteScriptTag(loggerProductionLibraryPath, sb);
            }

            JavaScriptHelpers.WriteJavaScriptBeginTag(sb);
            JavaScriptHelpers.WriteLine("(function () {", sb);

            Dictionary<string, string> appenderNames = new Dictionary<string, string>();
            Sequence sequence = new Sequence();

            XmlHelpers.ProcessNodeList(
                xe.ChildNodes, 
                new [] {
                    new XmlHelpers.TagInfo("appender", ProcessAppender, new Regex("^(name|url)$")),
                    new XmlHelpers.TagInfo("root", ProcessRoot, null, 1),
                    new XmlHelpers.TagInfo("logger", ProcessLogger, new Regex("^(name|additivity)$"))
                }, 
                null, appenderNames, sequence, sb);
            
            JavaScriptHelpers.WriteLine("}());", sb);
            JavaScriptHelpers.WriteJavaScriptEndTag(sb);
        }

        // For the interface descriptions of these methods, see XmlHelpers.XmlElementProcessor delegate.

        // ---- Appender and its children

        private static void ProcessAppender(XmlElement xe, string parentName, Dictionary<string,string> appenderNames, Sequence sequence, StringBuilder sb)
        {
            if (xe == null) { return; }

            string appenderName = XmlHelpers.RequiredAttribute(xe, "name");
            string appenderUrl = XmlHelpers.OptionalAttribute(
                xe, "url", AppenderDefaultUrl, @"^[ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789\-._~:/?#[\]@!$&'()*+,;=]+$");

            string appenderVariableName = string.Format("{0}{1}", JsAppenderVariablePrefix, sequence.Next());
            appenderNames[appenderName] = appenderVariableName;

            JavaScriptHelpers.GenerateAppender(appenderVariableName, appenderUrl, sb);

            XmlHelpers.ProcessNodeList(
                xe.ChildNodes,
                new[] {
                    new XmlHelpers.TagInfo("level", ProcessAppenderLevel, new Regex("^value$"), 1),
                    new XmlHelpers.TagInfo("batchSize", ProcessBatchSize, new Regex("^value$"), 1),
                    new XmlHelpers.TagInfo("timerInterval", ProcessTimerInterval, new Regex("^value$"), 1),
                    new XmlHelpers.TagInfo("sessionId", ProcessSessionId, new Regex("^value$"), 1)
                },
                appenderVariableName, appenderNames, sequence, sb);
        }

        private static void ProcessAppenderLevel(XmlElement xe, string parentName, Dictionary<string,string> appenderNames, Sequence sequence, StringBuilder sb)
        {
            JavaScriptHelpers.ProcessTagWithValue(
                xe, parentName, Constants.RegexLevels, "{0}.setThreshold(jsnlog.Level.{1});", sb);
        }

        private static void ProcessBatchSize(XmlElement xe, string parentName, Dictionary<string,string> appenderNames, Sequence sequence, StringBuilder sb)
        {
            JavaScriptHelpers.ProcessTagWithValue(
                xe, parentName, Constants.RegexIntegerGreaterZero, "{0}.setBatchSize({1});", sb);
        }

        private static void ProcessTimerInterval(XmlElement xe, string parentName, Dictionary<string,string> appenderNames, Sequence sequence, StringBuilder sb)
        {
            JavaScriptHelpers.ProcessTagWithValue(
                xe, parentName, Constants.RegexIntegerGreaterZero, "{0}.setTimerInterval({1}); {0}.setTimed(true);", sb);
        }

        // Normally used by version. But this method will override version.
        private static void ProcessSessionId(XmlElement xe, string parentName, Dictionary<string,string> appenderNames, Sequence sequence, StringBuilder sb)
        {
            JavaScriptHelpers.ProcessTagWithValue(
                xe, parentName, null, "{0}.setSessionId(\"{1}\");", sb);
        }

        // ---- Root, Logger, and their children

        private static void AddDefaultAppenderToLogger(string loggerVariableName, StringBuilder sb)
        {
            JavaScriptHelpers.WriteLine(string.Format("var {0};", DefaultAppenderVariable), sb);
            JavaScriptHelpers.GenerateAppender(DefaultAppenderVariable, AppenderDefaultUrl, sb);

            JavaScriptHelpers.AddAppenderToLogger(loggerVariableName, DefaultAppenderVariable, sb);
        }

        private static void ProcessLoggerChildren(
            XmlNodeList xmlNodeList, string parentName, Dictionary<string, string> appenderNames,
            StringBuilder sb)
        {
            XmlHelpers.ProcessNodeList(
                xmlNodeList,
                new[] {
                    new XmlHelpers.TagInfo("level", ProcessLoggerLevel, new Regex("^value$"), 1),
                    new XmlHelpers.TagInfo("appender-ref", ProcessAppenderRef, new Regex("^ref$"))
                },
                parentName, appenderNames, null, sb);
        }

        private static void ProcessRoot(XmlElement xe, string parentName, Dictionary<string,string> appenderNames, Sequence sequence, StringBuilder sb)
        {
            JavaScriptHelpers.WriteLine(string.Format("var {0}=jsnlog.getRootLogger();", JsRootLoggerVariable), sb);

            if (xe == null)
            {
                // No root has been defined in the web.config.
                // But we must have a root with an AjaxAppender.
                // So create one with a default appender.

                AddDefaultAppenderToLogger(JsRootLoggerVariable, sb);
                return;
            }

            ProcessLoggerChildren(xe.ChildNodes, JsRootLoggerVariable, appenderNames, sb);
        }

        private static void ProcessLogger(XmlElement xe, string parentName, Dictionary<string,string> appenderNames, Sequence sequence, StringBuilder sb)
        {
            if (xe == null) { return; }

            string loggerName = XmlHelpers.RequiredAttribute(xe, "name");
            string additivityEnabled = XmlHelpers.OptionalAttribute(xe, "additivity", AdditivityDefaultValue, Constants.RegexBool);

            JavaScriptHelpers.WriteLine(string.Format("{0}=jsnlog.getLogger('{1}');", JsLoggerVariable, loggerName), sb);
            if (additivityEnabled != AdditivityDefaultValue)
            {
                JavaScriptHelpers.WriteLine(string.Format("{0}.setAdditivity({1});", JsLoggerVariable, additivityEnabled), sb);
            }

            ProcessLoggerChildren(xe.ChildNodes, JsLoggerVariable, appenderNames, sb);
        }

        private static void ProcessLoggerLevel(XmlElement xe, string parentName, Dictionary<string,string> appenderNames, Sequence sequence, StringBuilder sb)
        {
            JavaScriptHelpers.ProcessTagWithValue(
                xe, parentName, Constants.RegexLevels, "{0}.setLevel(jsnlog.Level.{1});", sb);
        }

        private static void ProcessAppenderRef(XmlElement xe, string parentName, Dictionary<string,string> appenderNames, Sequence sequence, StringBuilder sb)
        {
            if (xe == null)
            {
                if (parentName == JsRootLoggerVariable)
                {
                    // A root is being defined without an appender-ref.
                    // So attach a default appender.

                    AddDefaultAppenderToLogger(parentName, sb);
                }

                return;
            }

            string appenderRef = XmlHelpers.RequiredAttribute(xe, "ref");

            if (!appenderNames.ContainsKey(appenderRef))
            {
                throw new UnknownAppenderException(appenderRef);
            }

            string appenderVariableName = appenderNames[appenderRef];

            JavaScriptHelpers.AddAppenderToLogger(parentName, appenderVariableName, sb);
        }


        public static string Configure()
        {
            XmlElement xe = XmlHelpers.RootElement();

            StringBuilder sb = new StringBuilder();

            ProcessRoot(xe, sb);

            return sb.ToString();
        }
    }
}
