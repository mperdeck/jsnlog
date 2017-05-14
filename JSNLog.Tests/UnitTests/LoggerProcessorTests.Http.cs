using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Xunit;
using JSNLog.LogHandling;
using System.Xml;
using System.IO;
using System.Collections.Specialized;

namespace JSNLog.Tests.UnitTests
{
    public partial class LoggerProcessorTests
    {
        [Fact]
        public void PutRequest()
        {
            string configXml = @"
                <jsnlog corsAllowedOriginsRegex=""abc""></jsnlog>
";

            var expectedResponseHeaders = new Dictionary<string, string>();
            List<LogEntry> expectedLogEntries = new List<LogEntry>();
            int expectedResponseCode = 405;

            RunTestHttp(
                "PUT", "http://abc.com",
                configXml, "", "", "", "230.45.1.8",
                _dtServerUtc, "http://mydomain.com/main",
                expectedResponseCode, expectedResponseHeaders, expectedLogEntries);
        }

        [Fact]
        public void GetRequest()
        {
            string configXml = @"
                <jsnlog corsAllowedOriginsRegex=""abc""></jsnlog>
";

            var expectedResponseHeaders = new Dictionary<string, string>();
            List<LogEntry> expectedLogEntries = new List<LogEntry>();
            int expectedResponseCode = 405;

            RunTestHttp(
                "GET", "http://abc.com",
                configXml, "", "", "", "230.45.1.8",
                _dtServerUtc, "http://mydomain.com/main",
                expectedResponseCode, expectedResponseHeaders, expectedLogEntries);
        }

        [Fact]
        public void OptionsRequest_NotAcceptedOrigin()
        {
            string configXml = @"
                <jsnlog corsAllowedOriginsRegex=""def""></jsnlog>
";

            string origin = "http://abc.com";

            var expectedResponseHeaders = new Dictionary<string, string> { 
                {"Allow", "POST"}
            };

            List<LogEntry> expectedLogEntries = new List<LogEntry>();
            int expectedResponseCode = 200;

            RunTestHttp(
                "OPTIONS", origin,
                configXml, "", "", "", "230.45.1.8",
                _dtServerUtc, "http://mydomain.com/main",
                expectedResponseCode, expectedResponseHeaders, expectedLogEntries);
        }

        [Fact]
        public void OptionsRequest_AcceptedOrigin()
        {
            string configXml = @"
                <jsnlog corsAllowedOriginsRegex=""abc""></jsnlog>
";

            string origin = "http://abc.com";

            var expectedResponseHeaders = new Dictionary<string, string> { 
                {"Allow", "POST"}, 
                {"Access-Control-Allow-Origin", origin}, 
                {"Access-Control-Max-Age", "3600"},  
                {"Access-Control-Allow-Methods", "POST"}, 
                {"Access-Control-Allow-Headers", "jsnlog-requestid, content-type"}
            };
            
            List<LogEntry> expectedLogEntries = new List<LogEntry>();
            int expectedResponseCode = 200;

            RunTestHttp(
                "OPTIONS", origin,
                configXml, "", "", "", "230.45.1.8",
                _dtServerUtc, "http://mydomain.com/main",
                expectedResponseCode, expectedResponseHeaders, expectedLogEntries);
        }

        [Fact]
        public void PostRequest_NotAcceptedOrigin()
        {
            string configXml = @"
                <jsnlog corsAllowedOriginsRegex=""def""></jsnlog>
";

            string origin = "http://abc.com";

            var expectedResponseHeaders = new Dictionary<string, string>
            {
            };

            // Note that JSNLog doesn't reliably know whether a given request is cross origin or not.
            // So on POST, it always logs the data. It relies on the browser to not send the POST
            // if previously it sent a response to a CORS OPTIONS request without CORS headers.

            List<LogEntry> expectedLogEntries = new List<LogEntry>
            {
                new LogEntry(Level.DEBUG, "a.b.c", @"first ""message""")
            };

            int expectedResponseCode = 200;

            RunTestHttp(
                "POST", origin,
                configXml, _json1, "", "", "230.45.1.8",
                _dtServerUtc, "http://mydomain.com/main",
                expectedResponseCode, expectedResponseHeaders, expectedLogEntries);
        }

        [Fact]
        public void PostRequest_AcceptedOrigin()
        {
            string configXml = @"
                <jsnlog corsAllowedOriginsRegex=""abc""></jsnlog>
";

            string origin = "http://abc.com";

            var expectedResponseHeaders = new Dictionary<string, string>
            {
                {"Access-Control-Allow-Origin", origin}
            };

            // Note that JSNLog doesn't reliably know whether a given request is cross origin or not.
            // So on POST, it always logs the data. It relies on the browser to not send the POST
            // if previously it sent a response to a CORS OPTIONS request without CORS headers.

            List<LogEntry> expectedLogEntries = new List<LogEntry>
            {
                new LogEntry(Level.DEBUG, "a.b.c", @"first ""message""")
            };

            int expectedResponseCode = 200;

            RunTestHttp(
                "POST", origin,
                configXml, _json1, "", "", "230.45.1.8",
                _dtServerUtc, "http://mydomain.com/main",
                expectedResponseCode, expectedResponseHeaders, expectedLogEntries);
        }
    }
}

