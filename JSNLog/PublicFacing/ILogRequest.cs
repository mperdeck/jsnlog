using System;
using System.Collections.Generic;

namespace JSNLog
{
    public interface ILogRequest
    {
        string Message { get; }
        string Logger { get; }
        string Level { get; }
        DateTime UtcDate { get; }
        string JsonMessage { get; }

        string UserAgent { get; }
        string UserHostAddress { get; }
        string RequestId { get; }
        string Url { get; }

        Dictionary<string, string> QueryParameters { get; }
        Dictionary<string, string> Cookies { get; }
        Dictionary<string, string> Headers { get; }
    }
}
