using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JSNLog.LogHandling;
using JSNLog.Tests.Logic;
using System.Xml;

namespace JSNLog.Tests.UnitTests
{
    [TestClass]
    public class LoggerProcessorTests
    {
        private string _json1 = null;
        private string _json1root = null;
        private string _json2 = null;
        private string _json3 = null;

        private DateTime _dtFirstLogUtc;
        private DateTime _dtSecondLogUtc;
        private DateTime _dtServerUtc;

        private DateTime _dtFirstLog;
        private DateTime _dtSecondLog;
        private DateTime _dtServer;

        public LoggerProcessorTests()
        {
            _dtFirstLogUtc = new DateTime(2013, 8, 16, 19, 50, 23, DateTimeKind.Utc);
            _dtSecondLogUtc = _dtFirstLogUtc.AddSeconds(2);
            _dtServerUtc = _dtSecondLogUtc.AddSeconds(10);

            _dtFirstLog = JSNLog.Infrastructure.Utils.UtcToLocalDateTime(_dtFirstLogUtc);
            _dtSecondLog = JSNLog.Infrastructure.Utils.UtcToLocalDateTime(_dtSecondLogUtc);
            _dtServer = JSNLog.Infrastructure.Utils.UtcToLocalDateTime(_dtServerUtc);

            _json1 = @"{
'r': 'therequestid',
'lg': [
{ 'm': 'first message', 'n': 'a.b.c', 'l': 1500, 't': " + Utils.MsSince1970(_dtFirstLogUtc).ToString() + @"}
] }";

            _json1root = @"{
'r': 'therequestid',
'lg': [
{ 'm': 'first message', 'n': '', 'l': 1500, 't': " + Utils.MsSince1970(_dtFirstLogUtc).ToString() + @"}
] }";

            _json2 = @"{
'r': 'therequestid2',
'lg': [
{ 'm': 'first message', 'n': 'a.b.c', 'l': 1500, 't': " + Utils.MsSince1970(_dtFirstLogUtc).ToString() + @"},
{ 'm': 'second message', 'n': 'a2.b3.c4', 'l': 3000, 't': " + Utils.MsSince1970(_dtSecondLogUtc).ToString() + @"}
] }";
            // Same as _json1, but without 'r' field
            _json3 = @"{
'lg': [
{ 'm': 'first message', 'n': 'a.b.c', 'l': 1500, 't': " + Utils.MsSince1970(_dtFirstLogUtc).ToString() + @"}
] }";
        }

        [TestMethod]
        public void DefaultFormatOneLogItem()
        {
            // Arrange

            string configXml = @"
                <jsnlog></jsnlog>
";

            var expected = new [] {
                new LoggerProcessor.LogData("first message", "a.b.c",Constants.Level.ERROR, 1500,
                    "first message", 1500, "a.b.c", "therequestid", 
                    _dtFirstLogUtc, _dtServerUtc, _dtFirstLog,_dtServer,
                    "my browser", "12.345.98.7", "http://mydomain.com/main")
            };

            // Act and Assert

            RunTest(configXml, _json1, "my browser", "12.345.98.7",
                        _dtServerUtc, "http://mydomain.com/main", expected);
        }

        [TestMethod]
        public void DefaultFormatOneLogItemRootLogger()
        {
            // Arrange

            string configXml = @"
                <jsnlog></jsnlog>
";

            var expected = new[] {
                new LoggerProcessor.LogData("first message", Constants.RootLoggerNameServerSide,Constants.Level.ERROR, 1500,
                    "first message", 1500, Constants.RootLoggerNameServerSide, "therequestid", 
                    _dtFirstLogUtc, _dtServerUtc, _dtFirstLog,_dtServer,
                    "my browser", "12.345.98.7", "http://mydomain.com/main")
            };

            // Act and Assert

            RunTest(configXml, _json1root, "my browser", "12.345.98.7",
                        _dtServerUtc, "http://mydomain.com/main", expected);
        }

        [TestMethod]
        public void DefaultFormatTwoLogItems()
        {
            // Arrange

            string configXml = @"
                <jsnlog></jsnlog>
";

            var expected = new[] {
                new LoggerProcessor.LogData(
                    "first message", 
                    "a.b.c",Constants.Level.DEBUG, 1500,
                    "first message", 1500, "a.b.c", "therequestid", 
                    _dtFirstLogUtc, _dtServerUtc, _dtFirstLog,_dtServer,
                    "my browser", "12.345.98.7", "http://mydomain.com/main"),
                new LoggerProcessor.LogData(
                    "second message",
                    "a2.b3.c4",Constants.Level.INFO, 3000,
                    "second message", 3000, "a2.b3.c4", "therequestid2", 
                    _dtFirstLogUtc, _dtServerUtc, _dtFirstLog,_dtServer,
                    "my browser", "12.345.98.7", "http://mydomain.com/main")
            };

            // Act and Assert

            RunTest(configXml, _json2, "my browser", "12.345.98.7",
                        _dtServerUtc, "http://mydomain.com/main", expected);
        }

        [TestMethod]
        public void FullFormatOneLogItem()
        {
            // Arrange

            string dateFormat = "dd-MM-yyyy HH:mm:ss";

            string configXml = @"
                <jsnlog 
serverSideMessageFormat=""msg: %message, utcDate: %utcDate, utcDateServer: %utcDateServer, date: %date, dateServer: %dateServer, level: %level, userAgent: %userAgent, userHostAddress: %userHostAddress, requestId: %requestId, url: %url, logger: %logger""
dateFormat="""+dateFormat+@"""
></jsnlog>
";
            var expected = new[] {
                new LoggerProcessor.LogData(
                    string.Format("msg: first message, utcDate: {0}, utcDateServer: {1}, date: {2}, dateServer: {3}, level: 1500, userAgent: my browser, userHostAddress: 12.345.98.7, requestId: therequestid1, url: http://mydomain.com/main, logger: a.b.c",
                            _dtFirstLogUtc.ToString(dateFormat), _dtServerUtc.ToString(dateFormat), 
                            _dtFirstLog.ToString(dateFormat), _dtServer.ToString(dateFormat)),
                    "a.b.c",Constants.Level.DEBUG, 1500,
                    "first message", 1500, "a.b.c", "therequestid", 
                    _dtFirstLogUtc, _dtServerUtc, _dtFirstLog,_dtServer,
                    "my browser", "12.345.98.7", "http://mydomain.com/main")
            };

            // Act and Assert

            RunTest(configXml, _json1, "my browser", "12.345.98.7",
                        _dtServerUtc, "http://mydomain.com/main", expected);
        }

        [TestMethod]
        public void LoggerLevelOverrideOneLogItem()
        {
            // Arrange

            string configXml = @"
                <jsnlog serverSideLogger=""server.logger"" serverSideLevel=""FATAL""></jsnlog>
";

            var expected = new[] {
                new LoggerProcessor.LogData("first message", "server.logger",Constants.Level.FATAL, 6000,
                    "first message", 1500, "a.b.c", "therequestid", 
                    _dtFirstLogUtc, _dtServerUtc, _dtFirstLog,_dtServer,
                    "my browser", "12.345.98.7", "http://mydomain.com/main")
            };

            // Act and Assert

            RunTest(configXml, _json1, "my browser", "12.345.98.7",
                        _dtServerUtc, "http://mydomain.com/main", expected);
        }

        [TestMethod]
        public void MessageWithLoggerDateInDefaultFormat()
        {
            // Arrange

            string configXml = @"
                <jsnlog serverSideMessageFormat=""%message | %utcDate""></jsnlog>
";

            var expected = new[] {
                new LoggerProcessor.LogData(
                    string.Format("first message | {0}", _dtFirstLogUtc.ToString("yyyy-MM-dd HH:mm:ss,fff")), 
                    "server.logger",Constants.Level.DEBUG, 1500,
                    "first message", 1500, "a.b.c", "therequestid", 
                    _dtFirstLogUtc, _dtServerUtc, _dtFirstLog,_dtServer,
                    "my browser", "12.345.98.7", "http://mydomain.com/main")
            };

            // Act and Assert

            RunTest(configXml, _json1, "my browser", "12.345.98.7",
                        _dtServerUtc, "http://mydomain.com/main", expected);
        }

        [TestMethod]
        public void MissingRequestId()
        {
            // Arrange

            string configXml = @"
                <jsnlog></jsnlog>
";

            var expected = new[] {
                new LoggerProcessor.LogData("first message", "a.b.c",Constants.Level.ERROR, 1500,
                    "first message", 1500, "a.b.c", "", 
                    _dtFirstLogUtc, _dtServerUtc, _dtFirstLog,_dtServer,
                    "my browser", "12.345.98.7", "http://mydomain.com/main")
            };

            // Act and Assert

            RunTest(configXml, _json3, "my browser", "12.345.98.7",
                        _dtServerUtc, "http://mydomain.com/main", expected);
        }

        private void RunTest(string configXml, string json, string userAgent, string userHostAddress,
            DateTime serverSideTimeUtc, string url, IEnumerable<LoggerProcessor.LogData> expected)
        {
            XmlElement xe = Utils.ConfigToXe(configXml);

            // Act

            List<LoggerProcessor.LogData> actual =
                LoggerProcessor.ProcessLogRequestExec(json, userAgent, userHostAddress,
                    serverSideTimeUtc, url, xe);

            TestLogDatasEqual(expected, actual);
        }

        private void TestLogDatasEqual(IEnumerable<LoggerProcessor.LogData> expected, IEnumerable<LoggerProcessor.LogData> actual)
        {
            Assert.AreEqual(expected.Count(), actual.Count(), "Counts not equal");

            for (int i = 0; i < expected.Count(); i++)
            {
                Assert.AreEqual(expected.ElementAt(i).Message, expected.ElementAt(i).Message);
                Assert.AreEqual(expected.ElementAt(i).LoggerName, expected.ElementAt(i).LoggerName);
                Assert.AreEqual(expected.ElementAt(i).Level, expected.ElementAt(i).Level);
                Assert.AreEqual(expected.ElementAt(i).LevelInt, expected.ElementAt(i).LevelInt);

                Assert.AreEqual(expected.ElementAt(i).ClientLogMessage, expected.ElementAt(i).ClientLogMessage);
                Assert.AreEqual(expected.ElementAt(i).ClientLogLevel, expected.ElementAt(i).ClientLogLevel);
                Assert.AreEqual(expected.ElementAt(i).ClientLogLoggerName, expected.ElementAt(i).ClientLogLoggerName);
                Assert.AreEqual(expected.ElementAt(i).ClientLogRequestId, expected.ElementAt(i).ClientLogRequestId);
                Assert.AreEqual(expected.ElementAt(i).LogDateUtc, expected.ElementAt(i).LogDateUtc);
                Assert.AreEqual(expected.ElementAt(i).LogDateServerUtc, expected.ElementAt(i).LogDateServerUtc);
                Assert.AreEqual(expected.ElementAt(i).LogDate, expected.ElementAt(i).LogDate);
                Assert.AreEqual(expected.ElementAt(i).LogDateServer, expected.ElementAt(i).LogDateServer);
                Assert.AreEqual(expected.ElementAt(i).UserAgent, expected.ElementAt(i).UserAgent);
                Assert.AreEqual(expected.ElementAt(i).UserHostAddress, expected.ElementAt(i).UserHostAddress);
                Assert.AreEqual(expected.ElementAt(i).LogRequestUrl, expected.ElementAt(i).LogRequestUrl);
            }
        }
    }
}

