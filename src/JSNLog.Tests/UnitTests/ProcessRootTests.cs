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
using JSNLog.Tests.Common;

namespace JSNLog.Tests.UnitTests
{
    /// <summary>
    /// These tests ensure that errorneous config is handled with an exception, rather than outright crashing.
    /// </summary>

    [Collection("JSNLog")]
    public class ProcessRootTests
    {
        [Fact]
        public void CorrectXml()
        {
            // Use this test to adhoc debug config processor

            // Arrange

            string configXml = @"
                <jsnlog>
</jsnlog>
";

            // Act and Assert
            RunTest(configXml);
        }

        [Fact]
        public void InvalidLevel()
        {
            // Arrange

            string configXml = @"
                <jsnlog>
    <logger name=""l2"" level=""xyz"" />
</jsnlog>
";

            // Act and Assert
            Exception ex = Assert.Throws<ConfigurationException>(() => RunTest(configXml));
        }

        [Fact]
        public void InvalidLevel2()
        {
            // Arrange

            string configXml = @"
                <jsnlog>
    <ajaxAppender name=""da1"" level=""abc"" />
</jsnlog>
";

            // Act and Assert
                        Exception ex = Assert.Throws<ConfigurationException>(() => RunTest(configXml));
        }

        [Fact]
        public void InvalidAppender()
        {
            // Arrange

            string configXml = @"
                <jsnlog>
    <ajaxAppender name=""da1"" level=""2300"" />
    <logger name=""l2"" appenders=""da2"" />
</jsnlog>
";

            // Act and Assert
            Exception ex = Assert.Throws<ConfigurationException>(() => RunTest(configXml));
        }

        [Fact]
        public void InvalidAppender2()
        {
            // Arrange

            string configXml = @"
                <jsnlog>
    <logger name=""l2"" appenders=""da2"" />
</jsnlog>
";

            // Act and Assert
            Exception ex = Assert.Throws<ConfigurationException>(() => RunTest(configXml));
        }

        [Fact]
        public void NoAppenderName()
        {
            // Arrange

            string configXml = @"
                <jsnlog>
    <ajaxAppender level=""2300"" />
</jsnlog>
";

            // Act and Assert
            Exception ex = Assert.Throws<ConfigurationException>(() => RunTest(configXml));
        }

        [Fact]
        public void InvalidBatchSize()
        {
            // Arrange

            string configXml = @"
                <jsnlog>
    <ajaxAppender name=""aa"" batchSize=""abc"" level=""2300"" />
</jsnlog>
";

            // Act and Assert
            Exception ex = Assert.Throws<WebConfigException>(() => RunTest(configXml));
        }

        [Fact]
        public void InvalidBufferSize()
        {
            // Arrange

            string configXml = @"
                <jsnlog>
    <ajaxAppender name=""aa"" bufferSize=""abc"" sendWithBufferLevel=""3000"" level=""2300"" storeInBufferLevel=""2000"" />
</jsnlog>
";

            // Act and Assert
            Exception ex = Assert.Throws<WebConfigException>(() => RunTest(configXml));
        }

        [Fact]
        public void MissingBufferParameter()
        {
            // Arrange

            string configXml = @"
                <jsnlog>
    <ajaxAppender name=""aa"" bufferSize=""2"" level=""2300"" storeInBufferLevel=""2000"" />
</jsnlog>
";

            // Act and Assert
            Exception ex = Assert.Throws<ConfigurationException>(() => RunTest(configXml));
        }

        [Fact]
        public void MissingBufferParameter2()
        {
            // Arrange

            string configXml = @"
                <jsnlog>
    <ajaxAppender name=""aa"" sendWithBufferLevel=""3000"" level=""2300"" storeInBufferLevel=""2000"" />
</jsnlog>
";

            // Act and Assert
            Exception ex = Assert.Throws<ConfigurationException>(() => RunTest(configXml));
        }

        [Fact]
        public void MissingBufferParameter3()
        {
            // Arrange

            string configXml = @"
                <jsnlog>
    <ajaxAppender name=""aa"" bufferSize=""2"" level=""2300"" sendWithBufferLevel=""3000"" />
</jsnlog>
";

            // Act and Assert
            Exception ex = Assert.Throws<ConfigurationException>(() => RunTest(configXml));
        }

        [Fact]
        public void WrongBufferParameter()
        {
            // Arrange

            string configXml = @"
                <jsnlog>
    <ajaxAppender name=""aa"" bufferSize=""2"" storeInBufferLevel=""2400"" level=""2300"" sendWithBufferLevel=""3000"" />
</jsnlog>
";

            // Act and Assert
            Exception ex = Assert.Throws<ConfigurationException>(() => RunTest(configXml));
        }

        [Fact]
        public void WrongBufferParameter2()
        {
            // Arrange

            string configXml = @"
                <jsnlog>
    <ajaxAppender name=""aa"" bufferSize=""2"" storeInBufferLevel=""2000"" level=""3300"" sendWithBufferLevel=""3000"" />
</jsnlog>
";

            // Act and Assert
            Exception ex = Assert.Throws<ConfigurationException>(() => RunTest(configXml));
        }

        [Fact]
        public void InvalidEnabled()
        {
            // Arrange

            // Note that the XML deserializer regards "1" as a valid input for booleans
            // and correctly translates that to true.
            string configXml = @"
                <jsnlog enabled=""xyz"">
</jsnlog>
";

            // Act and Assert
            Exception ex = Assert.Throws<WebConfigException>(() => RunTest(configXml));
        }

        [Fact]
        public void InvalidMaxMessages()
        {
            // Arrange

            string configXml = @"
                <jsnlog maxMessages=""xyz"">
</jsnlog>
";

            // Act and Assert
            Exception ex = Assert.Throws<WebConfigException>(() => RunTest(configXml));
        }

        [Fact]
        public void InvalidMaxMessages2()
        {
            // Arrange

            string configXml = @"
                <jsnlog maxMessages=""true"">
</jsnlog>
";

            // Act and Assert
            Exception ex = Assert.Throws<WebConfigException>(() => RunTest(configXml));
        }

        private void RunTest(string configXml)
        {
            var sb = new StringBuilder();

            CommonTestHelpers.SetConfigCache(configXml);

            var configProcessor = new ConfigProcessor();
            configProcessor.ProcessRootExec(sb, s => s, "23.89.450.1", "req", true);
        }
    }
}

