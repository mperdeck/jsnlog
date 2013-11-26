using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JSNLog.Infrastructure;
using Common.Logging;
using System.Web.Script.Serialization;
using System.Xml;

namespace JSNLog.LogHandling
{
    public class LoggerProcessor
    {
        /// <summary>
        /// The log data sent in a single log request from the client.
        /// It is expected that this list has 2 items:
        /// * the requestId (key: r)
        /// * the array with log items (key: lg)
        /// </summary>
        private class LogRequestData : Dictionary<string, Object>
        {
        }

        public class LogData
        {
            // ----- Processed data -----

            public string Message { get; private set; }

            // Client LoggerName and Level may have been overridden by JSNLog config in web.config.
            // Final result of this is stored in properties below. 
            public string LoggerName { get; private set; }
            public Constants.Level Level { get; private set; }

            // The level number. May not correspond directly with Level
            // To get Level from LevelInt, first Level at or above LevelInt is used.
            public int LevelInt { get; private set; }

            // ----- Raw data -----

            public string ClientLogMessage { get; private set; }
            public int ClientLogLevel { get; private set; }
            public string ClientLogLoggerName { get; private set; }
            public string ClientLogRequestId { get; private set; }

            public DateTime LogDateUtc { get; private set; }
            public DateTime LogDateServerUtc { get; private set; }
            public DateTime LogDate { get; private set; }
            public DateTime LogDateServer { get; private set; }

            public string UserAgent { get; private set; }
            public string UserHostAddress { get; private set; }
            public string LogRequestUrl { get; private set; }

            public LogData(string message, string loggerName, Constants.Level level, int levelInt,
                    string clientLogMessage, int clientLogLevel, string clientLogLoggerName, string clientLogRequestId,
                    DateTime logDateUtc, DateTime logDateServerUtc, DateTime logDate, DateTime logDateServer,
                    string userAgent, string userHostAddress, string logRequestUrl)
            {
                Message = message;
                LoggerName = loggerName;
                Level = level;
                LevelInt = levelInt;

                ClientLogMessage = clientLogMessage;
                ClientLogLevel = clientLogLevel;
                ClientLogLoggerName = clientLogLoggerName;
                ClientLogRequestId = clientLogRequestId;
                LogDateUtc = logDateUtc;
                LogDateServerUtc = logDateServerUtc;
                LogDate = logDate;
                LogDateServer = logDateServer;
                UserAgent = userAgent;
                UserHostAddress = userHostAddress;
                LogRequestUrl = logRequestUrl;
            }
        }

        public static void ProcessLogRequest(string json, string userAgent, string userHostAddress,
            DateTime serverSideTimeUtc, string url)
        {
            XmlElement xe = XmlHelpers.RootElement();

            List<LogData> logDatas =
                ProcessLogRequestExec(json, userAgent, userHostAddress, serverSideTimeUtc, url, xe);

            // ---------------------------------
            // Pass log data to Common Logging

            foreach (LogData logData in logDatas)
            {
                ILog log = LogManager.GetLogger(logData.LoggerName);

                switch (logData.Level)
                {
                    case Constants.Level.TRACE:
                        log.Trace(logData.Message);
                        break;

                    case Constants.Level.DEBUG:
                        log.Debug(logData.Message);
                        break;

                    case Constants.Level.INFO:
                        log.Info(logData.Message);
                        break;

                    case Constants.Level.WARN:
                        log.Warn(logData.Message);
                        break;

                    case Constants.Level.ERROR:
                        log.Error(logData.Message);
                        break;

                    case Constants.Level.FATAL:
                        log.Fatal(logData.Message);
                        break;

                    default:
                        throw new Exception(string.Format("ProcessLogRequest - finalLevel={0}", logData.Level));
                }
            }
        }

        /// <summary>
        /// Processes a request with logging info. Unit testable.
        /// 
        /// Returns log info in easily digestable format.
        /// </summary>
        /// <param name="json">JSON sent from client by AjaxAppender</param>
        /// <param name="serverSideTimeUtc">Current time in UTC</param>
        /// <param name="url">Url of the log request</param>
        /// <param name="xe">The JSNLog element in web.config</param>
        public static List<LogData> ProcessLogRequestExec(string json, string userAgent, string userHostAddress,
            DateTime serverSideTimeUtc, string url, XmlElement xe)
        {
            List<LogData> logDatas = new List<LogData>();

            try
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                LogRequestData logRequestData = js.Deserialize<LogRequestData>(json);

                // Request Id will be missing from the message if the user never set it. 
                // This will be common when jsnlog.js is used stand alone.
                string requestId = SafeGet(logRequestData, "r", "");

                Object[] logItems = (Object[])(logRequestData["lg"]);

                foreach (Object logItem in logItems)
                {
                    LogData logData = ProcessLogItem((Dictionary<string, Object>)logItem, userAgent, userHostAddress, requestId, serverSideTimeUtc, url, xe);
                    logDatas.Add(logData);
                }
            }
            catch (Exception e)
            {
                try
                {
                    ILog log = LogManager.GetLogger(Constants.JSNLogInternalErrorLoggerName);

                    string message =
                        string.Format(
                            "json: {0}, userAgent: {1}, userHostAddress: {2}, serverSideTimeUtc: {3}, url: {4}, exception: {5}",
                            json, userAgent, userHostAddress, serverSideTimeUtc, url, e);

                    var logData = new LogData(message, Constants.JSNLogInternalErrorLoggerName, Constants.Level.ERROR,
                        5000, "", 5000, "", "", serverSideTimeUtc, serverSideTimeUtc,
                        Utils.UtcToLocalDateTime(serverSideTimeUtc), Utils.UtcToLocalDateTime(serverSideTimeUtc), 
                        userAgent, userHostAddress, url);

                    logDatas.Add(logData);
                }
                catch
                {
                }
            }

            return logDatas;
        }

        private static LogData ProcessLogItem(Dictionary<string, Object> logItem, string userAgent, string userHostAddress,
            string requestId, DateTime serverSideTimeUtc, string url, XmlElement xe)
        {
            string serversideLoggerNameOverride = XmlHelpers.OptionalAttribute(xe, "serverSideLogger", null);
            string messageFormat = XmlHelpers.OptionalAttribute(xe, "serverSideMessageFormat", "%message");
            string levelOverride = XmlHelpers.OptionalAttribute(xe, "serverSideLevel", null, LevelUtils.LevelRegex());
            string dateFormat = XmlHelpers.OptionalAttribute(xe, "dateFormat", "yyyy-MM-dd HH:mm:ss,fff");

            // ----------------

            string message = logItem["m"].ToString();
            string logger = logItem["n"].ToString();
            string level = logItem["l"].ToString(); // note that level as sent by the javascript is a number

            DateTime utcTimestamp = DateTime.UtcNow;
            string timestampMs = logItem["t"].ToString();
            try
            {
                double ms = double.Parse(timestampMs);
                utcTimestamp = (new DateTime(1970, 1, 1)).AddMilliseconds(ms);
            }
            catch
            {
            }

            // ----------------

            if (string.IsNullOrWhiteSpace(logger)) { logger = Constants.RootLoggerNameServerSide; }
            string finalLoggerName = serversideLoggerNameOverride ?? logger;

            string finalLevel = levelOverride ?? level;

            // ----------------

            string finalMessage = messageFormat
                .Replace("%message", message)
                .Replace("%utcDateServer", serverSideTimeUtc.ToString(dateFormat))
                .Replace("%utcDate", utcTimestamp.ToString(dateFormat))
                .Replace("%dateServer", Utils.UtcToLocalDateTime(serverSideTimeUtc).ToString(dateFormat))
                .Replace("%date", Utils.UtcToLocalDateTime(utcTimestamp).ToString(dateFormat))
                .Replace("%level", level)
                .Replace("%newline", System.Environment.NewLine)
                .Replace("%userAgent", userAgent)
                .Replace("%userHostAddress", userHostAddress)
                .Replace("%requestId", requestId)
                .Replace("%url", url)
                .Replace("%logger", logger);

            // ---------------

            LogData logData = new LogData(
                finalMessage, finalLoggerName, LevelUtils.ParseLevel(finalLevel).Value, LevelUtils.LevelNumber(finalLevel),
                message, int.Parse(level), logger, requestId,
                utcTimestamp, serverSideTimeUtc, Utils.UtcToLocalDateTime(utcTimestamp), Utils.UtcToLocalDateTime(serverSideTimeUtc),
                userAgent, userHostAddress, url);

            return logData;
        }

        /// <summary>
        /// Returns the value associated with a key in a dictionary.
        /// If the key is not present, returns the default value - rather than throwing an exception.
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        private static string SafeGet(Dictionary<string, Object> dictionary, string key, string defaultValue)
        {
            if (dictionary.ContainsKey(key))
            {
                return dictionary[key].ToString();
            }

            return defaultValue;
        }
    }
}
