using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Xunit;
using JSNLog.LogHandling;
using System.Xml;
using System.IO;
using System.Collections.Specialized;
using JSNLog.Infrastructure;
using JSNLog.Tests.Common;

namespace JSNLog.Tests.UnitTests
{
    public partial class LoggerProcessorTests
    {
        private class LogEntry
        {
            public Level Level {get;set;}
            public string LoggerName  {get;set;}
            public string Message { get; set; }

            public LogEntry(Level level, string loggerName, string message)
            {
                Level = level;
                LoggerName = loggerName;
                Message = message;
            }
        }

        private class TestLogger : ILoggingAdapter
        {
            public List<LogEntry> LogEntries { get; set; }

            public TestLogger()
            {
                LogEntries = new List<LogEntry>();
            }

            public void Log(FinalLogData finalLogData)
            {
                LogEntries.Add(new LogEntry(finalLogData.FinalLevel, finalLogData.FinalLogger, finalLogData.FinalMessage));
            }
        }

        private void RunTestHttp(
            string httpMethod, string origin,
            string configXml, string json, string requestId, string userAgent, string userHostAddress,
            DateTime serverSideTimeUtc, string url,
            int expectedResponseCode, Dictionary<string, string> expectedResponseHeaders, List<LogEntry> expectedLogEntries)
        {
            // Arrange

            LogResponse response = new LogResponse();
            TestLogger logger = new TestLogger();

            CommonTestHelpers.SetConfigCache(configXml, logger);

            // Act

            LoggerProcessor.ProcessLogRequest(
                json, 
                new LogRequestBase(userAgent, userHostAddress, requestId,url, null, null, null),
                serverSideTimeUtc,
                httpMethod, origin, response);

            // Assert

            Assert.Equal(expectedResponseCode, response.StatusCode);
            TestLogEntries(expectedLogEntries, logger.LogEntries);
            TestResponseHeaders(expectedResponseHeaders, response.Headers);
        }

        private void TestLogEntries(List<LogEntry> expectedLogEntries, List<LogEntry> actualLogEntries)
        {
            Assert.Equal(expectedLogEntries.Count(), actualLogEntries.Count());

            for (int i = 0; i < expectedLogEntries.Count(); i++)
            {
                Assert.Equal(expectedLogEntries.ElementAt(i).Message, actualLogEntries.ElementAt(i).Message);
                Assert.Equal(expectedLogEntries.ElementAt(i).LoggerName, actualLogEntries.ElementAt(i).LoggerName);
                Assert.Equal(expectedLogEntries.ElementAt(i).Level, actualLogEntries.ElementAt(i).Level);
            }
        }

        private void TestResponseHeaders(Dictionary<string, string> expectedHeaders, 
            Dictionary<string, string> actualHeaders)
        {
            Assert.Equal(expectedHeaders.Count, actualHeaders.Count);

            foreach(string key in expectedHeaders.Keys)
            {
                Assert.Equal(expectedHeaders[key], actualHeaders[key]);
            }
        }
    }
}

