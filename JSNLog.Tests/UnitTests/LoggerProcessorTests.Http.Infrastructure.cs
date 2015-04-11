using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JSNLog.LogHandling;
using JSNLog.Tests.Logic;
using System.Xml;
using System.IO;
using System.Collections.Specialized;

namespace JSNLog.Tests.UnitTests
{
    public partial class LoggerProcessorTests
    {
        private class LogEntry
        {
            public Constants.Level Level {get;set;}
            public string LoggerName  {get;set;}
            public string Message { get; set; }

            public LogEntry(Constants.Level level, string loggerName, string message)
            {
                Level = level;
                LoggerName = loggerName;
                Message = message;
            }
        }

        private class TestLogger : ILogger
        {
            public List<LogEntry> LogEntries { get; set; }

            public TestLogger()
            {
                LogEntries = new List<LogEntry>();
            }

            public void Log(Constants.Level level, string loggerName, string message)
            {
                LogEntries.Add(new LogEntry(level,loggerName, message));
            }
        }

        private class TestHttpResponse : HttpResponseBase
        {
            public override int StatusCode { get; set; }

            private NameValueCollection _headers = new NameValueCollection();
            public override NameValueCollection Headers { get { return _headers; } }

            public override void AppendHeader(string name, string value)
            {
                _headers.Add(name, value);
            }

            public TestHttpResponse()
            {
            }
        }

        private void RunTestHttp(
            string httpMethod, string origin,
            string configXml, string json, string requestId, string userAgent, string userHostAddress,
            DateTime serverSideTimeUtc, string url,
            int expectedResponseCode, NameValueCollection expectedResponseHeaders, List<LogEntry> expectedLogEntries)
        {
            // Arrange

            TestHttpResponse response = new TestHttpResponse();
            TestLogger logger = new TestLogger();
            XmlElement xe = Utils.ConfigToXe(configXml);

            // Act

            LoggerProcessor.ProcessLogRequest(json, userAgent, userHostAddress,
                serverSideTimeUtc, url, requestId,
                httpMethod, origin, response, logger, xe);

            // Assert

            Assert.AreEqual(expectedResponseCode, response.StatusCode);
            TestLogEntries(expectedLogEntries, logger.LogEntries);
            TestResponseHeaders(expectedResponseHeaders, response.Headers);
        }

        private void TestLogEntries(List<LogEntry> expectedLogEntries, List<LogEntry> actualLogEntries)
        {
            Assert.AreEqual(expectedLogEntries.Count(), actualLogEntries.Count(), "Log counts not equal");

            for (int i = 0; i < expectedLogEntries.Count(); i++)
            {
                Assert.AreEqual(expectedLogEntries.ElementAt(i).Message, actualLogEntries.ElementAt(i).Message);
                Assert.AreEqual(expectedLogEntries.ElementAt(i).LoggerName, actualLogEntries.ElementAt(i).LoggerName);
                Assert.AreEqual(expectedLogEntries.ElementAt(i).Level, actualLogEntries.ElementAt(i).Level);
            }
        }

        private void TestResponseHeaders(NameValueCollection expectedHeaders, NameValueCollection actualHeaders)
        {
            Assert.IsTrue(expectedHeaders.Count == actualHeaders.Count);

            foreach(string key in expectedHeaders.Keys)
            {
                Assert.AreEqual(expectedHeaders[key], actualHeaders[key]);
            }
        }
    }
}

