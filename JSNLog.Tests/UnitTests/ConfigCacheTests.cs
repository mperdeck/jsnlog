using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JSNLog.LogHandling;
using JSNLog.Tests.Logic;
using System.Xml;
using JSNLog.Infrastructure;
using System.Text;
using JSNLog.Exceptions;

namespace JSNLog.Tests.UnitTests
{
    [TestClass]
    public class ConfigCacheTests
    {
        [TestMethod]
        [ExpectedException(typeof(ConflictingConfigException))]
        public void SetConfigWithJsnlogInWebConfig()
        {
            // Arrange

            string configXml = @"
                <jsnlog maxMessages=""5"">
</jsnlog>
";

            XmlElement xe = TestUtils.ConfigToXe(configXml);
            JsnlogConfiguration jsnlogConfiguration = new JsnlogConfiguration();

            // Act

            JavascriptLogging.SetJsnlogConfiguration(() => xe, jsnlogConfiguration);
        }

        [TestMethod]
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
            Assert.AreEqual(jsnlogConfiguration, retrievedJsnlogConfiguration);
            Assert.AreEqual(jsnlogConfiguration.maxMessages, retrievedJsnlogConfiguration.maxMessages);
        }

        [TestMethod]
        public void GetConfigFromWebConfig()
        {
            // Arrange

            string configXml = @"
                <jsnlog maxMessages=""5"">
</jsnlog>
";
            XmlElement xe = TestUtils.ConfigToXe(configXml);
            JavascriptLogging.SetJsnlogConfiguration(null);
            JavascriptLogging.GetJsnlogConfiguration(() => xe);

            // Act

            JsnlogConfiguration retrievedJsnlogConfiguration = JavascriptLogging.GetJsnlogConfiguration();

            // Assert

            // Retrieved object is expected to be the exact same object that was put in
            Assert.AreEqual((uint)5, retrievedJsnlogConfiguration.maxMessages);
        }
    }
}

