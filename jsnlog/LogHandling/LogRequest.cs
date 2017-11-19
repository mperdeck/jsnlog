using System;
using System.Collections.Generic;

namespace JSNLog
{
    internal class LogRequest : LogRequestBase, ILogRequest
    {
        public string Message { get; private set; }
        public string Logger { get; private set; }
        public string Level { get; private set; }
        public DateTime UtcDate { get; private set; }
        public string EntryId { get; private set; }
        public string JsonMessage { get; private set; }

        public LogRequest(string message, string logger, string level,
            DateTime utcDate, string entryId, string jsonMessage, LogRequestBase logRequestBase)
            : base(logRequestBase)
        {
            Message = message;
            Logger = logger;
            Level = level;
            UtcDate = utcDate;
            EntryId = entryId;
            JsonMessage = jsonMessage;
        }

        public override string ToString()
        {
            return string.Format(
                "Message: {0}, Logger: {1}, Level: {2}, UtcDate: {3}, EntryId: {4}, JsonMessage: {5}, logRequestBase: {{{6}}}",
                Message, Logger, Level, UtcDate, EntryId, JsonMessage, base.ToString());
        }
    }
}
