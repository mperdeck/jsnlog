using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JSNLog.Tests.Logic;
using JSNLog.Infrastructure;

namespace JSNLog.Tests.UnitTests
{
    [TestClass]
    public class LoggingUrlHelpersTests
    {
        [TestMethod]
        public void IsLoggingUrl_NoUrlsConfigured()
        {
            string configXml = @"
                <jsnlog></jsnlog>
";

            TestUtils.SetConfigCache(configXml, null);

            Assert.IsTrue(LoggingUrlHelpers.IsLoggingUrl("/jsnlog.logger"));
            Assert.IsTrue(LoggingUrlHelpers.IsLoggingUrl("http://abc.com/jsnlog.logger"));
            Assert.IsFalse(LoggingUrlHelpers.IsLoggingUrl("http://abc.com/jsnlog.css"));
        }

        [TestMethod]
        public void IsLoggingUrl_NoUrlsConfigured_CodeConfig()
        {
            JavascriptLogging.SetJsnlogConfiguration(null, null);

            Assert.IsTrue(LoggingUrlHelpers.IsLoggingUrl("/jsnlog.logger"));
            Assert.IsTrue(LoggingUrlHelpers.IsLoggingUrl("http://abc.com/jsnlog.logger"));
            Assert.IsFalse(LoggingUrlHelpers.IsLoggingUrl("http://abc.com/jsnlog.css"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void IsLoggingUrl_NoUrl()
        {
            string configXml = @"
                <jsnlog></jsnlog>
";

            TestUtils.SetConfigCache(configXml, null);

            LoggingUrlHelpers.IsLoggingUrl(null);
        }

        [TestMethod]
        public void IsLoggingUrl_DefaultConfigured_NoAppenderUrlsConfigured()
        {
            string configXml = @"
                <jsnlog defaultAjaxUrl=""/jsnlogger""></jsnlog>
";

            TestUtils.SetConfigCache(configXml, null);

            Assert.IsTrue(LoggingUrlHelpers.IsLoggingUrl("/jsnlogger"));
            Assert.IsTrue(LoggingUrlHelpers.IsLoggingUrl("/abc/def/jsnlogger"));
            Assert.IsFalse(LoggingUrlHelpers.IsLoggingUrl("/jsnlog.logger"));
            Assert.IsFalse(LoggingUrlHelpers.IsLoggingUrl("http://abc.com/jsnlog.logger"));
            Assert.IsFalse(LoggingUrlHelpers.IsLoggingUrl("http://abc.com/jsnlog.css"));
        }

        [TestMethod]
        public void IsLoggingUrl_NoDefaultConfigured_AppenderUrlsConfigured()
        {
            string configXml = @"
                <jsnlog>
<ajaxAppender url=""/jsn2logger"" />

</jsnlog>
";

            TestUtils.SetConfigCache(configXml, null);

            Assert.IsTrue(LoggingUrlHelpers.IsLoggingUrl("/jsn2logger"));
            Assert.IsTrue(LoggingUrlHelpers.IsLoggingUrl("//abc.com/jsn2logger?a=b;c=d"));

            // Should also the url of the default appender
            Assert.IsTrue(LoggingUrlHelpers.IsLoggingUrl("/jsnlog.logger"));
            Assert.IsTrue(LoggingUrlHelpers.IsLoggingUrl("http://abc.com/jsnlog.logger"));
            Assert.IsFalse(LoggingUrlHelpers.IsLoggingUrl("http://abc.com/jsnlog.css"));
        }

        [TestMethod]
        public void IsLoggingUrl_NoDefaultConfigured_AppenderUrlsConfiguredWithTilde()
        {
            string configXml = @"
                <jsnlog>
<ajaxAppender url=""~/jsn2logger"" />

</jsnlog>
";

            TestUtils.SetConfigCache(configXml, null);

            Assert.IsTrue(LoggingUrlHelpers.IsLoggingUrl("/jsn2logger"));
            Assert.IsTrue(LoggingUrlHelpers.IsLoggingUrl("//abc.com/jsn2logger?a=b;c=d"));

            // Should also the url of the default appender
            Assert.IsTrue(LoggingUrlHelpers.IsLoggingUrl("/jsnlog.logger"));
            Assert.IsTrue(LoggingUrlHelpers.IsLoggingUrl("http://abc.com/jsnlog.logger"));
            Assert.IsFalse(LoggingUrlHelpers.IsLoggingUrl("http://abc.com/jsnlog.css"));
        }

        [TestMethod]
        public void IsLoggingUrl_DefaultConfigured_AppenderUrlsConfigured()
        {
            string configXml = @"
                <jsnlog defaultAjaxUrl=""/jsnlogger"">
    <ajaxAppender url=""/jsn2logger"" />
</jsnlog>
";

            TestUtils.SetConfigCache(configXml, null);

            Assert.IsTrue(LoggingUrlHelpers.IsLoggingUrl("/jsn2logger"));
            Assert.IsTrue(LoggingUrlHelpers.IsLoggingUrl("//abc.com/jsn2logger?a=b;c=d"));

            Assert.IsTrue(LoggingUrlHelpers.IsLoggingUrl("/jsnlogger"));
            Assert.IsTrue(LoggingUrlHelpers.IsLoggingUrl("/abc/def/jsnlogger"));
            Assert.IsFalse(LoggingUrlHelpers.IsLoggingUrl("/jsnlog.logger"));
            Assert.IsFalse(LoggingUrlHelpers.IsLoggingUrl("http://abc.com/jsnlog.logger"));
            Assert.IsFalse(LoggingUrlHelpers.IsLoggingUrl("http://abc.com/jsnlog.css"));
        }

        [TestMethod]
        public void IsLoggingUrl_DefaultConfigured_MultipleAppenderUrlsConfigured()
        {
            string configXml = @"
                <jsnlog defaultAjaxUrl=""/jsnlogger"">
    <ajaxAppender url=""/jsn2logger"" />
    <ajaxAppender url=""/jsn3logger"" />
</jsnlog>
";

            TestUtils.SetConfigCache(configXml, null);

            Assert.IsTrue(LoggingUrlHelpers.IsLoggingUrl("/jsn2logger"));
            Assert.IsTrue(LoggingUrlHelpers.IsLoggingUrl("//abc.com/jsn2logger?a=b;c=d"));

            Assert.IsTrue(LoggingUrlHelpers.IsLoggingUrl("/jsn3logger"));
            Assert.IsTrue(LoggingUrlHelpers.IsLoggingUrl("//abc.com/jsn3logger?a=b;c=d"));

            Assert.IsTrue(LoggingUrlHelpers.IsLoggingUrl("/jsnlogger"));
            Assert.IsTrue(LoggingUrlHelpers.IsLoggingUrl("/abc/def/jsnlogger"));
            Assert.IsFalse(LoggingUrlHelpers.IsLoggingUrl("/jsnlog.logger"));
            Assert.IsFalse(LoggingUrlHelpers.IsLoggingUrl("http://abc.com/jsnlog.logger"));
            Assert.IsFalse(LoggingUrlHelpers.IsLoggingUrl("http://abc.com/jsnlog.css"));
        }
    }
}