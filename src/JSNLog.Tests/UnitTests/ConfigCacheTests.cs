using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Xunit;
using JSNLog.LogHandling;
using System.Xml;
using JSNLog.Infrastructure;
using System.Text;
using JSNLog.Exceptions;

namespace JSNLog.Tests.UnitTests
{
    
    public class ConfigCacheTests
    {
        [Fact]
        public void SetConfigWithJsnlogInWebConfig()
        {
            // Arrange

            string configXml = @"
                <jsnlog maxMessages=""5"">
</jsnlog>
";

            XmlElement xe = UnitTestHelpers.ConfigToXe(configXml);
            JsnlogConfiguration jsnlogConfiguration = new JsnlogConfiguration();

            // Act

            Exception ex = Assert.Throws<ConflictingConfigException>(() => JavascriptLogging.SetJsnlogConfiguration(() => xe, jsnlogConfiguration));
        }

        [Fact]
        public void SetConfigWithoutJsnlogInWebConfig()
        {
            // Arrange

            JsnlogConfiguration jsnlogConfiguration = new JsnlogConfiguration
            {
                maxMessages = 5
            };

            JavascriptLogging.SetJsnlogConfiguration(() => null, jsnlogConfiguration);

            // Act

            JsnlogConfiguration retrievedJsnlogConfiguration = JavascriptLogging.GetJsnlogConfiguration();

            // Assert

            // Retrieved object is expected to be the exact same object that was put in
            Assert.Equal(jsnlogConfiguration, retrievedJsnlogConfiguration);
            Assert.Equal(jsnlogConfiguration.maxMessages, retrievedJsnlogConfiguration.maxMessages);
        }

        [Fact]
        public void GetConfigFromWebConfig()
        {
            // Arrange

            string configXml = @"
                <jsnlog maxMessages=""5"">
</jsnlog>
";
            XmlElement xe = UnitTestHelpers.ConfigToXe(configXml);
            JavascriptLogging.SetJsnlogConfiguration(null);
            JavascriptLogging.GetJsnlogConfiguration(() => xe);

            // Act

            JsnlogConfiguration retrievedJsnlogConfiguration = JavascriptLogging.GetJsnlogConfiguration();

            // Assert

            // Retrieved object is expected to be the exact same object that was put in
            Assert.Equal((uint)5, retrievedJsnlogConfiguration.maxMessages);
        }
    }
}

