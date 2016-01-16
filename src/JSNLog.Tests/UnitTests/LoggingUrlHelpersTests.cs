using System;
using Xunit;
using JSNLog.Infrastructure;
using JSNLog.Tests.Common;

namespace JSNLog.Tests.UnitTests
{
    [Collection("JSNLog")]
    public class LoggingUrlHelpersTests
    {
        [Fact]
        public void IsLoggingUrl_NoUrlsConfigured()
        {
            string configXml = @"
                <jsnlog></jsnlog>
";

            CommonTestHelpers.SetConfigCache(configXml, null);

            Assert.True(LoggingUrlHelpers.IsLoggingUrl("/jsnlog.logger"));
            Assert.True(LoggingUrlHelpers.IsLoggingUrl("http://abc.com/jsnlog.logger"));
            Assert.False(LoggingUrlHelpers.IsLoggingUrl("http://abc.com/jsnlog.css"));
        }

        [Fact]
        public void IsLoggingUrl_NoUrlsConfigured_CodeConfig()
        {
            JavascriptLogging.SetJsnlogConfiguration(null, null);

            Assert.True(LoggingUrlHelpers.IsLoggingUrl("/jsnlog.logger"));
            Assert.True(LoggingUrlHelpers.IsLoggingUrl("http://abc.com/jsnlog.logger"));
            Assert.False(LoggingUrlHelpers.IsLoggingUrl("http://abc.com/jsnlog.css"));
        }

        [Fact]
        public void IsLoggingUrl_NoUrl()
        {
            string configXml = @"
                <jsnlog></jsnlog>
";

            CommonTestHelpers.SetConfigCache(configXml, null);

            Exception ex = Assert.Throws<ArgumentNullException>(() => LoggingUrlHelpers.IsLoggingUrl(null));
        }

        [Fact]
        public void IsLoggingUrl_DefaultConfigured_NoAppenderUrlsConfigured()
        {
            string configXml = @"
                <jsnlog defaultAjaxUrl=""/jsnlogger""></jsnlog>
";

            CommonTestHelpers.SetConfigCache(configXml, null);

            Assert.True(LoggingUrlHelpers.IsLoggingUrl("/jsnlogger"));
            Assert.True(LoggingUrlHelpers.IsLoggingUrl("/abc/def/jsnlogger"));
            Assert.False(LoggingUrlHelpers.IsLoggingUrl("/jsnlog.logger"));
            Assert.False(LoggingUrlHelpers.IsLoggingUrl("http://abc.com/jsnlog.logger"));
            Assert.False(LoggingUrlHelpers.IsLoggingUrl("http://abc.com/jsnlog.css"));
        }

        [Fact]
        public void IsLoggingUrl_NoDefaultConfigured_AppenderUrlsConfigured()
        {
            string configXml = @"
                <jsnlog>
<ajaxAppender url=""/jsn2logger"" />

</jsnlog>
";

            CommonTestHelpers.SetConfigCache(configXml, null);

            Assert.True(LoggingUrlHelpers.IsLoggingUrl("/jsn2logger"));
            Assert.True(LoggingUrlHelpers.IsLoggingUrl("//abc.com/jsn2logger?a=b;c=d"));

            // Should also the url of the default appender
            Assert.True(LoggingUrlHelpers.IsLoggingUrl("/jsnlog.logger"));
            Assert.True(LoggingUrlHelpers.IsLoggingUrl("http://abc.com/jsnlog.logger"));
            Assert.False(LoggingUrlHelpers.IsLoggingUrl("http://abc.com/jsnlog.css"));
        }

        [Fact]
        public void IsLoggingUrl_NoDefaultConfigured_AppenderUrlsConfiguredWithTilde()
        {
            string configXml = @"
                <jsnlog>
<ajaxAppender url=""~/jsn2logger"" />

</jsnlog>
";

            CommonTestHelpers.SetConfigCache(configXml, null);

            Assert.True(LoggingUrlHelpers.IsLoggingUrl("/jsn2logger"));
            Assert.True(LoggingUrlHelpers.IsLoggingUrl("//abc.com/jsn2logger?a=b;c=d"));

            // url of the default appender should also be regarded as a logging url
            Assert.True(LoggingUrlHelpers.IsLoggingUrl("/jsnlog.logger"));
            Assert.True(LoggingUrlHelpers.IsLoggingUrl("http://abc.com/jsnlog.logger"));
            Assert.False(LoggingUrlHelpers.IsLoggingUrl("http://abc.com/jsnlog.css"));
        }

        [Fact]
        public void IsLoggingUrl_DefaultConfigured_AppenderUrlsConfigured()
        {
            string configXml = @"
                <jsnlog defaultAjaxUrl=""/jsnlogger"">
    <ajaxAppender url=""/jsn2logger"" />
</jsnlog>
";

            CommonTestHelpers.SetConfigCache(configXml, null);

            Assert.True(LoggingUrlHelpers.IsLoggingUrl("/jsn2logger"));
            Assert.True(LoggingUrlHelpers.IsLoggingUrl("//abc.com/jsn2logger?a=b;c=d"));

            Assert.True(LoggingUrlHelpers.IsLoggingUrl("/jsnlogger"));
            Assert.True(LoggingUrlHelpers.IsLoggingUrl("/abc/def/jsnlogger"));
            Assert.False(LoggingUrlHelpers.IsLoggingUrl("/jsnlog.logger"));
            Assert.False(LoggingUrlHelpers.IsLoggingUrl("http://abc.com/jsnlog.logger"));
            Assert.False(LoggingUrlHelpers.IsLoggingUrl("http://abc.com/jsnlog.css"));
        }

        [Fact]
        public void IsLoggingUrl_DefaultConfigured_MultipleAppenderUrlsConfigured()
        {
            string configXml = @"
                <jsnlog defaultAjaxUrl=""/jsnlogger"">
    <ajaxAppender url=""/jsn2logger"" />
    <ajaxAppender url=""/jsn3logger"" />
</jsnlog>
";

            CommonTestHelpers.SetConfigCache(configXml, null);

            Assert.True(LoggingUrlHelpers.IsLoggingUrl("/jsn2logger"));
            Assert.True(LoggingUrlHelpers.IsLoggingUrl("//abc.com/jsn2logger?a=b;c=d"));

            Assert.True(LoggingUrlHelpers.IsLoggingUrl("/jsn3logger"));
            Assert.True(LoggingUrlHelpers.IsLoggingUrl("//abc.com/jsn3logger?a=b;c=d"));

            Assert.True(LoggingUrlHelpers.IsLoggingUrl("/jsnlogger"));
            Assert.True(LoggingUrlHelpers.IsLoggingUrl("/abc/def/jsnlogger"));
            Assert.False(LoggingUrlHelpers.IsLoggingUrl("/jsnlog.logger"));
            Assert.False(LoggingUrlHelpers.IsLoggingUrl("http://abc.com/jsnlog.logger"));
            Assert.False(LoggingUrlHelpers.IsLoggingUrl("http://abc.com/jsnlog.css"));
        }
    }
}