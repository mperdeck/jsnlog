using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Collections.Specialized;
using System.Xml;
using JSNLog.Infrastructure;
using Common.Logging;
using System.Web.Script.Serialization;

namespace JSNLog
{
    public class LoggerHandler : IHttpHandler
    {
        private class LogItems : List<Dictionary<string, Object>>
        {
        }

        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            string userAgent = context.Request.UserAgent;
            string userHostAddress = context.Request.UserHostAddress;

            NameValueCollection formCollection = context.Request.Form;
            string json = formCollection["data"];

            JavaScriptSerializer js = new JavaScriptSerializer();
            LogItems logItems = js.Deserialize<LogItems>(json);

            foreach (Dictionary<string, Object> logItem in logItems)
            {
                ProcessLogItem(logItem, userAgent, userHostAddress);
            }
        }

        private void ProcessLogItem(Dictionary<string, Object> logItem, string userAgent, string userHostAddress)
        {
            XmlElement xe = XmlHelpers.RootElement();
            string serversideLoggerNameOverride = XmlHelpers.OptionalAttribute(xe, "serverSideLogger", null);
            string messageFormat = XmlHelpers.OptionalAttribute(xe, "serverSideMessageFormat", "%message");
            string levelOverride = XmlHelpers.OptionalAttribute(xe, "serverSideLevel", null, Constants.RegexLevels);

            // ----------------

            string logger = logItem["logger"].ToString();
            string level = logItem["level"].ToString();
            string url = logItem["url"].ToString();
            string version = "";

            string sessionId = "";
            if (logItem.ContainsKey("sessionid"))
            {
                sessionId = logItem["sessionid"].ToString();
            }

            DateTime utcTimestamp = DateTime.UtcNow;
            string timestampMs = logItem["timestamp"].ToString();
            try
            {
                double ms = double.Parse(timestampMs);
                utcTimestamp = (new DateTime(1970, 1, 1)).AddMilliseconds(ms);
            }
            catch
            {
            }

            // ----------------

            string finalLoggerName = serversideLoggerNameOverride ?? logger;
            string finalLevel = levelOverride ?? level;

            // ----------------

            ILog log = LogManager.GetLogger(finalLoggerName);

            // ----------------

            // jsnlog.js sends an array of messages in each log item.
            Object[] messageObjects = (Object[])logItem["message"];
            foreach (Object messageObject in messageObjects)
            {
                string message = messageObject.ToString();
                
                // ----------------

                string finalMessage = messageFormat
                    .Replace("%message", message)
                    .Replace("%utcdate", utcTimestamp.ToString("yyyy-MM-dd HH:mm:ss,fff"))
                    .Replace("%level", level)
                    .Replace("%newline", System.Environment.NewLine)
                    .Replace("%userAgent", userAgent)
                    .Replace("%userHostAddress", userHostAddress)
                    .Replace("%version", version)
                    .Replace("%sessionid", sessionId)
                    .Replace("%url", url)
                    .Replace("%logger", logger);

                // ---------------

                switch (finalLevel)
                {
                    case "TRACE":
                        log.Trace(finalMessage);
                        break;

                    case "DEBUG":
                        log.Debug(finalMessage);
                        break;

                    case "INFO":
                        log.Info(finalMessage);
                        break;

                    case "WARN":
                        log.Warn(finalMessage);
                        break;

                    case "ERROR":
                        log.Error(finalMessage);
                        break;

                    case "FATAL":
                        log.Fatal(finalMessage);
                        break;

                    default:
                        throw new Exception(string.Format("ProcessRequest - finalLevel={0}", finalLevel));
                }
            }
        }
    }
}
