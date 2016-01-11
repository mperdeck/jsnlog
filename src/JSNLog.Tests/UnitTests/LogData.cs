using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JSNLog.Infrastructure;

namespace JSNLog.Tests
{
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

        public LogData(FinalLogData finalLogData, DateTime serverUtc)
        {
            Message = finalLogData.FinalMessage;
            LoggerName = finalLogData.FinalLogger;
            Level = finalLogData.FinalLevel;
            LevelInt = -1;

            if (finalLogData.LogRequest != null)
            {
                ClientLogMessage = finalLogData.LogRequest.Message;
                ClientLogLevel = int.Parse(finalLogData.LogRequest.Level);
                ClientLogLoggerName = finalLogData.LogRequest.Logger;
                ClientLogRequestId = finalLogData.LogRequest.RequestId;
                LogDateUtc = finalLogData.LogRequest.UtcDate;
                LogDateServerUtc = serverUtc;
                LogDate = Utils.UtcToLocalDateTime(finalLogData.LogRequest.UtcDate);
                LogDateServer = Utils.UtcToLocalDateTime(serverUtc);
                UserAgent = finalLogData.LogRequest.UserAgent;
                UserHostAddress = finalLogData.LogRequest.UserHostAddress;
                LogRequestUrl = finalLogData.LogRequest.Url;
            }
        }
    }
}

