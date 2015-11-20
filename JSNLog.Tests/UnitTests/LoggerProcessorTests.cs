using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JSNLog.LogHandling;
using JSNLog.Tests.Logic;
using System.Xml;
using System.Web.Script.Serialization;
using JSNLog.Infrastructure;

namespace JSNLog.Tests.UnitTests
{
    [TestClass]
    public partial class LoggerProcessorTests
    {
        private string _json1 = null;
        private string _json1root = null;
        private string _json2 = null;
        private string _json4 = null;
        private string _json5 = null;

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
'lg': [
{ 'm': 'first ""message""', 'n': 'a.b.c', 'l': 1500, 't': " + TestUtils.MsSince1970(_dtFirstLogUtc).ToString() + @"}
] }";

            _json1root = @"{
'lg': [
{ 'm': 'first message', 'n': '', 'l': 1500, 't': " + TestUtils.MsSince1970(_dtFirstLogUtc).ToString() + @"}
] }";

            _json2 = @"{
'lg': [
{ 'm': 'first message', 'n': 'a.b.c', 'l': 1500, 't': " + TestUtils.MsSince1970(_dtFirstLogUtc).ToString() + @"},
{ 'm': 'second message', 'n': 'a2.b3.c4', 'l': 3000, 't': " + TestUtils.MsSince1970(_dtSecondLogUtc).ToString() + @"}
] }";
            // Same as _json1, but without 'm' field. This is invalid and should cause an internal error.
            _json4 = @"{
'lg': [
{ 'n': 'a.b.c', 'l': 1500, 't': " + TestUtils.MsSince1970(_dtFirstLogUtc).ToString() + @"}
] }";

            _json5 = @"{
""lg"": [
{ ""m"": ""{\""x\"":5,\""y\"":88}"", ""n"": ""a.b.c"", ""l"": 1500, ""t"": " + TestUtils.MsSince1970(_dtFirstLogUtc).ToString() + @"}
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
                new LoggerProcessor.LogData(@"first ""message""", "a.b.c",Level.DEBUG, 1500,
                    @"first ""message""", 1500, "a.b.c", "therequestid1", 
                    _dtFirstLogUtc, _dtServerUtc, _dtFirstLog,_dtServer,
                    "my browser", "12.345.98.7", "http://mydomain.com/main")
            };

            // Act and Assert

            RunTest(configXml, _json1, "therequestid1", "my browser", "12.345.98.7",
                        _dtServerUtc, "http://mydomain.com/main", expected);
        }

        [TestMethod]
        public void DefaultFormatOneObjectLogItem()
        {
            // Arrange

            string configXml = @"
                <jsnlog></jsnlog>
";

            var expected = new[] {
                new LoggerProcessor.LogData(@"{""x"":5,""y"":88}", "a.b.c",Level.DEBUG, 1500,
                    @"{""x"":5,""y"":88}", 1500, "a.b.c", "therequestid1", 
                    _dtFirstLogUtc, _dtServerUtc, _dtFirstLog,_dtServer,
                    "my browser", "12.345.98.7", "http://mydomain.com/main")
            };

            // Act and Assert

            RunTest(configXml, _json5, "therequestid1", "my browser", "12.345.98.7",
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
                new LoggerProcessor.LogData("first message", Constants.RootLoggerNameServerSide,Level.DEBUG, 1500,
                    "first message", 1500, Constants.RootLoggerNameServerSide, "therequestid", 
                    _dtFirstLogUtc, _dtServerUtc, _dtFirstLog,_dtServer,
                    "my browser", "12.345.98.7", "http://mydomain.com/main")
            };

            // Act and Assert

            RunTest(configXml, _json1root, "therequestid", "my browser", "12.345.98.7",
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
                    "a.b.c",Level.DEBUG, 1500,
                    "first message", 1500, "a.b.c", "therequestid2", 
                    _dtFirstLogUtc, _dtServerUtc, _dtFirstLog,_dtServer,
                    "my browser", "12.345.98.7", "http://mydomain.com/main"),
                new LoggerProcessor.LogData(
                    "second message",
                    "a2.b3.c4",Level.INFO, 3000,
                    "second message", 3000, "a2.b3.c4", "therequestid2", 
                    _dtSecondLogUtc, _dtServerUtc, _dtSecondLog,_dtServer,
                    "my browser", "12.345.98.7", "http://mydomain.com/main")
            };

            // Act and Assert

            RunTest(configXml, _json2, "therequestid2", "my browser", "12.345.98.7",
                        _dtServerUtc, "http://mydomain.com/main", expected);
        }

        [TestMethod]
        public void FullFormatOneLogItem()
        {
            // Arrange

            string dateFormat = "dd-MM-yyyy HH:mm:ss";

            string configXml = @"
                <jsnlog 
serverSideMessageFormat=""msg: %message, json: %jsonmessage, utcDate: %utcDate, utcDateServer: %utcDateServer, date: %date, dateServer: %dateServer, level: %level, userAgent: %userAgent, userHostAddress: %userHostAddress, requestId: %requestId, url: %url, logger: %logger""
dateFormat="""+dateFormat+@"""
></jsnlog>
";
            var expected = new[] {
                new LoggerProcessor.LogData(
                    string.Format(
                    @"msg: first ""message"", json: ""first \""message\"""", utcDate: {0}, utcDateServer: {1}, date: {2}, dateServer: {3}, level: 1500, userAgent: my browser, userHostAddress: 12.345.98.7, requestId: therequestid1, url: http://mydomain.com/main, logger: a.b.c",
                            _dtFirstLogUtc.ToString(dateFormat), _dtServerUtc.ToString(dateFormat), 
                            _dtFirstLog.ToString(dateFormat), _dtServer.ToString(dateFormat)),
                    "a.b.c",Level.DEBUG, 1500,
                    @"first ""message""", 1500, "a.b.c", "therequestid1", 
                    _dtFirstLogUtc, _dtServerUtc, _dtFirstLog,_dtServer,
                    "my browser", "12.345.98.7", "http://mydomain.com/main")
            };

            // Act and Assert

            RunTest(configXml, _json1, "therequestid1", "my browser", "12.345.98.7",
                        _dtServerUtc, "http://mydomain.com/main", expected);
        }

        [TestMethod]
        public void FullFormatOneObjectLogItem()
        {
            // Arrange

            string dateFormat = "dd-MM-yyyy HH:mm:ss";

            string configXml = @"
                <jsnlog 
serverSideMessageFormat=""msg: %message, json: %jsonmessage, utcDate: %utcDate, utcDateServer: %utcDateServer, date: %date, dateServer: %dateServer, level: %level, userAgent: %userAgent, userHostAddress: %userHostAddress, requestId: %requestId, url: %url, logger: %logger""
dateFormat=""" + dateFormat + @"""
></jsnlog>
";
            var expected = new[] {
                new LoggerProcessor.LogData(
                    string.Format(
                    @"msg: {{""x"":5,""y"":88}}, json: {{""x"":5,""y"":88}}, utcDate: {0}, utcDateServer: {1}, date: {2}, dateServer: {3}, level: 1500, userAgent: my browser, userHostAddress: 12.345.98.7, requestId: therequestid1, url: http://mydomain.com/main, logger: a.b.c",
                            _dtFirstLogUtc.ToString(dateFormat), _dtServerUtc.ToString(dateFormat), 
                            _dtFirstLog.ToString(dateFormat), _dtServer.ToString(dateFormat)),
                    "a.b.c",Level.DEBUG, 1500,
                    @"{""x"":5,""y"":88}", 1500, "a.b.c", "therequestid1", 
                    _dtFirstLogUtc, _dtServerUtc, _dtFirstLog,_dtServer,
                    "my browser", "12.345.98.7", "http://mydomain.com/main")
            };

            // Act and Assert

            RunTest(configXml, _json5, "therequestid1", "my browser", "12.345.98.7",
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
                new LoggerProcessor.LogData(@"first ""message""", "server.logger",Level.FATAL, 6000,
                    @"first ""message""", 1500, "a.b.c", "therequestid1", 
                    _dtFirstLogUtc, _dtServerUtc, _dtFirstLog,_dtServer,
                    "my browser", "12.345.98.7", "http://mydomain.com/main")
            };

            // Act and Assert

            RunTest(configXml, _json1, "therequestid1", "my browser", "12.345.98.7",
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
                    string.Format(@"first ""message"" | {0}", _dtFirstLogUtc.ToString("o")), 
                    "a.b.c",Level.DEBUG, 1500,
                    @"first ""message""", 1500, "a.b.c", "therequestid1", 
                    _dtFirstLogUtc, _dtServerUtc, _dtFirstLog,_dtServer,
                    "my browser", "12.345.98.7", "http://mydomain.com/main")
            };

            // Act and Assert

            RunTest(configXml, _json1, "therequestid1", "my browser", "12.345.98.7",
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
                new LoggerProcessor.LogData(@"first ""message""", "a.b.c",Level.DEBUG, 1500,
                    @"first ""message""", 1500, "a.b.c", null, 
                    _dtFirstLogUtc, _dtServerUtc, _dtFirstLog,_dtServer,
                    "my browser", "12.345.98.7", "http://mydomain.com/main")
            };

            // Act and Assert

            RunTest(configXml, _json1, null, "my browser", "12.345.98.7",
                        _dtServerUtc, "http://mydomain.com/main", expected);
        }

        [TestMethod]
        public void OverrideMessageDataWithHandler()
        {
            string configXml = @"
                <jsnlog></jsnlog>
";
            string appendedMsg = "Extra Message";

            var expected = new[] {
                new LoggerProcessor.LogData(@"first ""message""" + appendedMsg , "d.e.f", Level.FATAL, 1500,
                    @"first ""message""", 1500, "a.b.c",  "therequestid1",
                    _dtFirstLogUtc, _dtServerUtc, _dtFirstLog,_dtServer,
                    "my browser", "12.345.98.7", "http://mydomain.com/main")
            };

            JSNLog.LoggingHandler loggingHandler = (LoggingEventArgs loggingEventArgs) =>
            {
                loggingEventArgs.FinalMessage += appendedMsg;
                loggingEventArgs.FinalLevel = Level.FATAL;
                loggingEventArgs.FinalLogger = "d.e.f";
            };

            JSNLog.JavascriptLogging.OnLogging += loggingHandler;

            // Act and Assert

            RunTest(configXml, _json1, "therequestid1", "my browser", "12.345.98.7",
                        _dtServerUtc, "http://mydomain.com/main", expected);

            // Clean up

            JSNLog.JavascriptLogging.OnLogging -= loggingHandler;
        }

        [TestMethod]
        public void FilterOutLogMessages()
        {
            // Arrange

            string configXml = @"
                <jsnlog></jsnlog>
";

            var expected = new[] {
                new LoggerProcessor.LogData(
                    "first message",
                    "a.b.c",Level.DEBUG, 1500,
                    "first message", 1500, "a.b.c", "therequestid2",
                    _dtFirstLogUtc, _dtServerUtc, _dtFirstLog,_dtServer,
                    "my browser", "12.345.98.7", "http://mydomain.com/main")
            };


            JSNLog.LoggingHandler loggingHandler = (LoggingEventArgs loggingEventArgs) =>
            {
                if (loggingEventArgs.FinalMessage.Contains("second message"))
                {
                    loggingEventArgs.Cancel = true;
                }
            };


            JSNLog.JavascriptLogging.OnLogging += loggingHandler;

            // Act and Assert

            RunTest(configXml, _json2, "therequestid2", "my browser", "12.345.98.7",
                        _dtServerUtc, "http://mydomain.com/main", expected);

            // Clean up

            JSNLog.JavascriptLogging.OnLogging -= loggingHandler;
        }
        
        private class DatesBag
        {
            public DateTime utcDate { get; set; }
            public DateTime utcDateServer { get; set; }
            public DateTime date { get; set; }
            public DateTime dateServer { get; set; }
        }

        [TestMethod]
        public void EnsureDefaultDateFormatAllowsJsonDeserialization()
        {
            // Arrange

            string configXml = @"
                <jsnlog serverSideMessageFormat=""{ 
                    'utcDate': '%utcDate', 'utcDateServer': '%utcDateServer', 'date': '%date', 'dateServer': '%dateServer' 
                    }""></jsnlog>";

            XmlElement xe = TestUtils.ConfigToXe(configXml);

            // Act

            var jsnlogConfiguration = XmlHelpers.DeserialiseXml<JsnlogConfiguration>(xe);
            List<LoggerProcessor.LogData> actual =
                LoggerProcessor.ProcessLogRequestExec(
                    _json1, 
                    new LogRequestBase("my browser", "12.345.98.7", "http://mydomain.com/main", "",null, null, null),
                    _dtServerUtc, jsnlogConfiguration);

            string messageToBeLogged = actual.FirstOrDefault().Message;

            var javaScriptSerializer = new JavaScriptSerializer();
            var datesBag1 = javaScriptSerializer.Deserialize<DatesBag>(messageToBeLogged);
            TestDatesEqual(datesBag1.utcDate, _dtFirstLogUtc);
            TestDatesEqual(datesBag1.utcDateServer, _dtServerUtc);
            TestDatesEqual(datesBag1.date, _dtFirstLog);
            TestDatesEqual(datesBag1.dateServer, _dtServer);

            var datesBag2 = Newtonsoft.Json.JsonConvert.DeserializeObject<DatesBag>(messageToBeLogged);
            TestDatesEqual(datesBag2.utcDate, _dtFirstLogUtc);
            TestDatesEqual(datesBag2.utcDateServer, _dtServerUtc);
            TestDatesEqual(datesBag2.date, _dtFirstLog);
            TestDatesEqual(datesBag2.dateServer, _dtServer);
        }

        [TestMethod]
        public void InternalError()
        {
            string configXml = @"
                <jsnlog></jsnlog>
";

            XmlElement xe = TestUtils.ConfigToXe(configXml);

            // Act

            var jsnlogConfiguration = XmlHelpers.DeserialiseXml<JsnlogConfiguration>(xe);
            List<LoggerProcessor.LogData> actual =
                LoggerProcessor.ProcessLogRequestExec(
                    _json4,
                    new LogRequestBase("my browser", "12.345.98.7", "http://mydomain.com/main", "",null, null, null),
                    _dtServerUtc, jsnlogConfiguration);

            // Assert

            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual(Constants.JSNLogInternalErrorLoggerName, actual.ElementAt(0).LoggerName);
        }

        private void RunTest(string configXml, string json, string requestId, string userAgent, string userHostAddress,
            DateTime serverSideTimeUtc, string url, IEnumerable<LoggerProcessor.LogData> expected)
        {
            XmlElement xe = TestUtils.ConfigToXe(configXml);

            // Act

            var jsnlogConfiguration = XmlHelpers.DeserialiseXml<JsnlogConfiguration>(xe);
            List<LoggerProcessor.LogData> actual =
                LoggerProcessor.ProcessLogRequestExec(
                    json,
                    new LogRequestBase(userAgent, userHostAddress, requestId, url, null, null, null),
                    serverSideTimeUtc, jsnlogConfiguration);

            TestLogDatasEqual(expected, actual);
        }

        private void TestDatesEqual(DateTime dt1, DateTime dt2)
        {
            Assert.IsTrue(new DateTimeOffset(dt1) == new DateTimeOffset(dt2));
        }

        private void TestLogDatasEqual(IEnumerable<LoggerProcessor.LogData> expected, IEnumerable<LoggerProcessor.LogData> actual)
        {
            Assert.AreEqual(expected.Count(), actual.Count(), "Counts not equal");

            for (int i = 0; i < expected.Count(); i++)
            {
                Assert.AreEqual(expected.ElementAt(i).Message, actual.ElementAt(i).Message);
                Assert.AreEqual(expected.ElementAt(i).LoggerName, actual.ElementAt(i).LoggerName);
                Assert.AreEqual(expected.ElementAt(i).Level, actual.ElementAt(i).Level);
                Assert.AreEqual(expected.ElementAt(i).LevelInt, actual.ElementAt(i).LevelInt);

                Assert.AreEqual(expected.ElementAt(i).ClientLogMessage, actual.ElementAt(i).ClientLogMessage);
                Assert.AreEqual(expected.ElementAt(i).ClientLogLevel, actual.ElementAt(i).ClientLogLevel);
                Assert.AreEqual(expected.ElementAt(i).ClientLogLoggerName, actual.ElementAt(i).ClientLogLoggerName);
                Assert.AreEqual(expected.ElementAt(i).ClientLogRequestId, actual.ElementAt(i).ClientLogRequestId);
                Assert.AreEqual(expected.ElementAt(i).LogDateUtc, actual.ElementAt(i).LogDateUtc);
                Assert.AreEqual(expected.ElementAt(i).LogDateServerUtc, actual.ElementAt(i).LogDateServerUtc);
                Assert.AreEqual(expected.ElementAt(i).LogDate, actual.ElementAt(i).LogDate);
                Assert.AreEqual(expected.ElementAt(i).LogDateServer, actual.ElementAt(i).LogDateServer);
                Assert.AreEqual(expected.ElementAt(i).UserAgent, actual.ElementAt(i).UserAgent);
                Assert.AreEqual(expected.ElementAt(i).UserHostAddress, actual.ElementAt(i).UserHostAddress);
                Assert.AreEqual(expected.ElementAt(i).LogRequestUrl, actual.ElementAt(i).LogRequestUrl);
            }
        }
    }
}

