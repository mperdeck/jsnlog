using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JSNLog.Infrastructure;
using System.Web.Script.Serialization;
using System.Xml;
using System.Web;
using System.Net;
using System.Text.RegularExpressions;

namespace JSNLog.LogHandling
{
    internal class LoggerProcessor
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

        internal class LogData
        {
            // ----- Processed data -----

            public string Message { get; private set; }

            // Client LoggerName and Level may have been overridden by JSNLog config in web.config.
            // Final result of this is stored in properties below. 
            public string LoggerName { get; private set; }
            public Level Level { get; private set; }

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

            public LogData(string message, string loggerName, Level level, int levelInt,
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

        /// <summary>
        /// Processes the incoming request. This method is not depended on the environment and so can be unit tested.
        /// Note that the incoming request may not be the POST request with log data.
        /// 
        /// Effect of this method:
        /// * setting StatusCode on the response parameter
        /// * adding headers to the response parameter
        /// * logging contents of log request
        /// </summary>
        /// <param name="json">
        /// JSON payload in the incoming request
        /// </param>
        /// <param name="logRequestBase">
        /// * Type of browser that sent the request
        /// * IP address that sent the address
        /// * Url that the request was sent to
        /// * Request Id sent as part of the request
        /// * Request data to be given to onLogging event handler
        /// </param>
        /// <param name="serverSideTimeUtc">Current time in UTC</param>
        /// <param name="httpMethod">HTTP method of the request</param>
        /// <param name="origin">Value of the Origin request header</param>
        /// <param name="response">
        /// Empty response object. This method can add headers, etc.
        /// </param>
        /// <param name="logger">
        /// Logger object, used to do the actual logging.
        /// </param>
        /// <param name="xe">The JSNLog element in web.config</param>
        internal static void ProcessLogRequest(string json, LogRequestBase logRequestBase,
            DateTime serverSideTimeUtc,
            string httpMethod, string origin, LogResponse response, ILogger logger, XmlElement xe)
        {
            if ((httpMethod != "POST") && (httpMethod != "OPTIONS"))
            {
                response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                return;
            }

            string corsAllowedOriginsRegex = XmlHelpers.OptionalAttribute(xe, "corsAllowedOriginsRegex", null);
            bool originIsAllowed = ((!string.IsNullOrEmpty(corsAllowedOriginsRegex)) && (!string.IsNullOrEmpty(origin)) && Regex.IsMatch(origin, corsAllowedOriginsRegex));

            if (originIsAllowed)
            {
                response.AppendHeader("Access-Control-Allow-Origin", origin);
            }

            response.StatusCode = (int)HttpStatusCode.OK;

            if (httpMethod == "OPTIONS")
            {
                // Standard HTTP response (not related to CORS)
                response.AppendHeader("Allow", "POST");

                // Only if the origin is allow send CORS headers
                if (originIsAllowed)
                {
                    response.AppendHeader("Access-Control-Max-Age", Constants.CorsAccessControlMaxAgeInSeconds.ToString());
                    response.AppendHeader("Access-Control-Allow-Methods", "POST");
                    response.AppendHeader("Access-Control-Allow-Headers", "jsnlog-requestid, content-type");
                }

                return;
            }

            // httpMethod must be POST

            List<LogData> logDatas =
                ProcessLogRequestExec(json, logRequestBase, serverSideTimeUtc, xe);

            // ---------------------------------
            // Pass log data to Common Logging

            foreach (LogData logData in logDatas)
            {
                logger.Log(logData.Level, logData.LoggerName, logData.Message);
            }
        }

        /// <summary>
        /// Processes a request with logging info. Unit testable.
        /// 
        /// Returns log info in easily digestable format.
        /// </summary>
        /// <param name="json">JSON sent from client by AjaxAppender</param>
        /// <param name="serverSideTimeUtc">Current time in UTC</param>
        /// <param name="xe">The JSNLog element in web.config</param>
        internal static List<LogData> ProcessLogRequestExec(string json, LogRequestBase logRequestBase,
            DateTime serverSideTimeUtc, XmlElement xe)
        {
            List<LogData> logDatas = new List<LogData>();

            try
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                LogRequestData logRequestData = js.Deserialize<LogRequestData>(json);

                Object[] logItems = (Object[])(logRequestData["lg"]);

                foreach (Object logItem in logItems)
                {
                    LogData logData = ProcessLogItem((Dictionary<string, Object>)logItem,
                        logRequestBase, serverSideTimeUtc, xe);

                    if (logData != null)
                    {
                        logDatas.Add(logData);
                    }
                }
            }
            catch (Exception e)
            {
                try
                {
                    string message =
                        string.Format(
                            "json: {0}, userAgent: {1}, userHostAddress: {2}, serverSideTimeUtc: {3}, url: {4}, exception: {5}",
                            json, logRequestBase.UserAgent, logRequestBase.UserHostAddress, serverSideTimeUtc,
                            logRequestBase.Url, e);

                    var logData = new LogData(message, Constants.JSNLogInternalErrorLoggerName, Level.ERROR,
                        5000, "", 5000, "", "", serverSideTimeUtc, serverSideTimeUtc,
                        Utils.UtcToLocalDateTime(serverSideTimeUtc), Utils.UtcToLocalDateTime(serverSideTimeUtc),
                        logRequestBase.UserAgent, logRequestBase.UserHostAddress, logRequestBase.Url);

                    logDatas.Add(logData);
                }
                catch
                {
                }
            }

            return logDatas;
        }

        private static LogData ProcessLogItem(Dictionary<string, Object> logItem, LogRequestBase logRequestBase,
            DateTime serverSideTimeUtc, XmlElement xe)
        {
            string serversideLoggerNameOverride = XmlHelpers.OptionalAttribute(xe, "serverSideLogger", null);
            string messageFormat = XmlHelpers.OptionalAttribute(xe, "serverSideMessageFormat", "%message");
            string levelOverride = XmlHelpers.OptionalAttribute(xe, "serverSideLevel", null, LevelUtils.LevelRegex());
            string dateFormat = XmlHelpers.OptionalAttribute(xe, "dateFormat", "o");

            // ----------------

            string message = logItem["m"].ToString();
            string logger = logItem["n"].ToString();
            string level = logItem["l"].ToString(); // note that level as sent by the javascript is a number

            DateTime utcDate = DateTime.UtcNow;
            string timestampMs = logItem["t"].ToString();
            try
            {
                double ms = double.Parse(timestampMs);
                utcDate = DateTime.SpecifyKind((new DateTime(1970, 1, 1)).AddMilliseconds(ms), DateTimeKind.Utc);
            }
            catch
            {
            }

            // ----------------

            string jsonmessage = "";
            if (messageFormat.Contains("%jsonmessage"))
            {
                jsonmessage = LogMessageHelpers.EnsureValidJson(message);
            }

            // ----------------

            var logRequest = new LogRequest(message, logger, level, utcDate, jsonmessage, logRequestBase);
            var loggingEventArgs = new LoggingEventArgs(logRequest) 
            {
                Cancel = false
            };

            // ----------------

            if (string.IsNullOrWhiteSpace(logger)) { logger = Constants.RootLoggerNameServerSide; }
            loggingEventArgs.FinalLogger = serversideLoggerNameOverride ?? logger;

            string consolidatedLevel = levelOverride ?? level;
            loggingEventArgs.FinalLevel = LevelUtils.ParseLevel(consolidatedLevel).Value;

            // ----------------

            loggingEventArgs.FinalMessage = messageFormat
                .Replace("%message", message)
                .Replace("%jsonmessage", jsonmessage)
                .Replace("%utcDateServer", serverSideTimeUtc.ToString(dateFormat))
                .Replace("%utcDate", utcDate.ToString(dateFormat))
                .Replace("%dateServer", Utils.UtcToLocalDateTime(serverSideTimeUtc).ToString(dateFormat))
                .Replace("%date", Utils.UtcToLocalDateTime(utcDate).ToString(dateFormat))
                .Replace("%level", level)
                .Replace("%newline", System.Environment.NewLine)
                .Replace("%userAgent", logRequestBase.UserAgent)
                .Replace("%userHostAddress", logRequestBase.UserHostAddress)
                .Replace("%requestId", logRequestBase.RequestId ?? "")
                .Replace("%url", logRequestBase.Url)
                .Replace("%logger", logger);

            // ----------------

            JavascriptLogging.RaiseLoggingEvent(loggingEventArgs);

            // If user wrote event handler that decided not to log the message, return null
            if (loggingEventArgs.Cancel) { return null; }

            // ---------------

            LogData logData = new LogData(
                loggingEventArgs.FinalMessage, loggingEventArgs.FinalLogger, loggingEventArgs.FinalLevel, 
                LevelUtils.LevelNumber(consolidatedLevel),
                message, int.Parse(level), logger, logRequestBase.RequestId,
                utcDate, serverSideTimeUtc, Utils.UtcToLocalDateTime(utcDate), Utils.UtcToLocalDateTime(serverSideTimeUtc),
                logRequestBase.UserAgent, logRequestBase.UserHostAddress, logRequestBase.Url);

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
