using System;
using System.Collections.Generic;

namespace JSNLog
{
    internal class LogRequestBase
    {
        public string UserAgent { get; private set; }
        public string UserHostAddress { get; private set; }
        public string RequestId { get; private set; }
        public string Url { get; private set; }

        // Not used by JSNLog, only by onLogging 
        public Dictionary<string, string> QueryParameters { get; private set; }
        public Dictionary<string, string> Cookies { get; private set; }
        public Dictionary<string, string> Headers { get; private set; }

        public LogRequestBase(string userAgent, string userHostAddress, string requestId,
                                string url,
                                Dictionary<string, string> queryParameters, Dictionary<string, string> cookies,
                                Dictionary<string, string> headers)
        {
            UserAgent = userAgent;
            UserHostAddress = userHostAddress;
            RequestId = requestId;
            Url = url;
            QueryParameters = queryParameters;
            Cookies = cookies;
            Headers = headers;
        }

        public LogRequestBase(LogRequestBase other)
        {
            UserAgent = other.UserAgent;
            UserHostAddress = other.UserHostAddress;
            RequestId = other.RequestId;
            Url = other.Url;
            QueryParameters = other.QueryParameters;
            Cookies = other.Cookies;
            Headers = other.Headers;
        }

        public override string ToString()
        {
            return string.Format(
                "UserAgent: {0}, UserHostAddress: {1}, RequestId: {2}, Url: {3}, QueryParameters: {4}, Cookies: {5}, Headers: {6}",
                UserAgent, UserHostAddress, RequestId, Url, 
                QueryParameters == null ? "null" : "not null",
                Cookies == null ? "null" : "not null",
                Headers == null ? "null" : "not null");
        }
    }
}
