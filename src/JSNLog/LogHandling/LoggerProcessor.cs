using System;
using System.Collections.Generic;
using JSNLog.Infrastructure;
using System.Net;
using System.Text.RegularExpressions;
using JSNLog.Exceptions;

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
        
        private class LogRequestSingleMsg
        {
            public string m {get;set;}
            public string n {get;set;}
            public string l {get;set;}
            public string t {get;set;}
        }

        private class LogRequestData
        {
            public string r {get;set;}
            public ICollection<LogRequestSingleMsg> lg {get;set;}
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
        internal static void ProcessLogRequest(string json, LogRequestBase logRequestBase,
            DateTime serverSideTimeUtc,
            string httpMethod, string origin, LogResponse response)
        {
            JsnlogConfiguration jsnlogConfiguration = JavascriptLogging.GetJsnlogConfiguration();

            ILoggingAdapter logger = JavascriptLogging.GetLogger();

            if ((httpMethod != "POST") && (httpMethod != "OPTIONS"))
            {
                response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                return;
            }

            string corsAllowedOriginsRegex = jsnlogConfiguration.corsAllowedOriginsRegex;
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

                // Only if the origin is allowed send CORS headers
                if (originIsAllowed)
                {
                    response.AppendHeader("Access-Control-Max-Age", Constants.CorsAccessControlMaxAgeInSeconds.ToString());
                    response.AppendHeader("Access-Control-Allow-Methods", "POST");
                    response.AppendHeader("Access-Control-Allow-Headers", "jsnlog-requestid, content-type");
                }

                return;
            }

            // httpMethod must be POST

            List<FinalLogData> logDatas =
                ProcessLogRequestExec(json, logRequestBase, serverSideTimeUtc, jsnlogConfiguration);

            // ---------------------------------
            // Pass log data to Common Logging

            foreach (FinalLogData logData in logDatas)
            {
                logger.Log(logData);
            }
        }

        /// <summary>
        /// Processes a request with logging info. Unit testable.
        /// 
        /// Returns log info in easily digestable format.
        /// </summary>
        /// <param name="json">JSON sent from client by AjaxAppender</param>
        /// <param name="serverSideTimeUtc">Current time in UTC</param>
        /// <param name="jsnlogConfiguration">Contains all config info</param>
        internal static List<FinalLogData> ProcessLogRequestExec(string json, LogRequestBase logRequestBase,
            DateTime serverSideTimeUtc, JsnlogConfiguration jsnlogConfiguration)
        {
            var logDatas = new List<FinalLogData>();
            FinalLogData logData = null;

            try
            {
                LogRequestData logRequestData = LogMessageHelpers.DeserializeJson<LogRequestData>(json);

                foreach (var logItem in logRequestData.lg)
                {
                    logData = null; // in case ProcessLogItem throws exception
                    logData = ProcessLogItem(logItem,
                        logRequestBase, serverSideTimeUtc, jsnlogConfiguration);

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
                    string message = string.Format("Exception: {0}, json: {1}, FinalLogData: {{{2}}}, logRequestBase: {{{3}}}", e, json, logData, logRequestBase);

                    var internalErrorFinalLogData = new FinalLogData(null)
                    {
                        FinalMessage = message,
                        FinalLogger = Constants.JSNLogInternalErrorLoggerName,
                        FinalLevel = Level.ERROR
                    };

                    logDatas.Add(internalErrorFinalLogData);
                }
                catch
                {
                }
            }

            return logDatas;
        }

        private static FinalLogData ProcessLogItem(LogRequestSingleMsg logItem, LogRequestBase logRequestBase,
            DateTime serverSideTimeUtc, JsnlogConfiguration jsnlogConfiguration)
        {
            string serversideLoggerNameOverride = jsnlogConfiguration.serverSideLogger;
            string messageFormat = jsnlogConfiguration.serverSideMessageFormat;
            string levelOverride = jsnlogConfiguration.serverSideLevel;
            string dateFormat = jsnlogConfiguration.dateFormat;

            try
            {
                LevelUtils.ValidateLevel(levelOverride);
            }
            catch (Exception e)
            {
                throw new PropertyException("levelOverride", e);
            }

            // ----------------

            string message = logItem.m;
            string logger = logItem.n;
            string level = logItem.l; // note that level as sent by the javascript is a number

            DateTime utcDate = DateTime.UtcNow;
            string timestampMs = logItem.t;
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
                Cancel = false,
                ServerSideMessageFormat = messageFormat
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

            return loggingEventArgs;
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
