using System;
using System.Collections.Generic;

namespace JSNLog
{
    public class LogRequest : LogRequestBase, ILogRequest
    {
        public string Message { get; private set; }
        public string Logger { get; private set; }
        public string Level { get; private set; }
        public DateTime UtcDate { get; private set; }
        public string JsonMessage { get; private set; }

        public LogRequest(string message, string logger, string level,
            DateTime utcDate, string jsonMessage, LogRequestBase logRequestBase)
            : base(logRequestBase)
        {
            Message = message;
            Logger = logger;
            Level = level;
            UtcDate = utcDate;
            JsonMessage = jsonMessage;
        }
    }
}
