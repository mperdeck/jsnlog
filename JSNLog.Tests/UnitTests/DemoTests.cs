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
using System.IO;

namespace JSNLog.Tests.UnitTests
{
    [TestClass]
    public class DemoTests
    {
        // All tests here write a partial with the complete demo html.
        // The partials are written to directory:
        private const string _demosDirectory = @"D:\Dev\JSNLog\jsnlog.website\WebSite\Views\Shared\Demos";

        // >>>>>>>>>>>>>>>>>>>>>>>
        // All files are removed from the Demos directory before the tests are run.
        // If you run 1 test, you'll get only 1 file.
        // Be sure to run ALL tests before publishing the web site.
        // <<<<<<<<<<<<<<<<<<<<<<<<

        [ClassInitialize]
        public static void ClassInitialize(TestContext a)
        {
            // Delete all files in Demos directory

            System.IO.DirectoryInfo demosDirectory = new DirectoryInfo(_demosDirectory);

            foreach (FileInfo file in demosDirectory.GetFiles("*.cshtml"))
            {
                file.Delete();
            }
        }

        [TestMethod]
        public void DemoTest1()
        {
            TestDemo(
                @"
<jsnlog serverSideMessageFormat=""Sent: % date, Brower: % userAgent - % message"" >
</jsnlog>",
                @"
new JsnlogConfiguration {
    serverSideMessageFormat=""Sent: % date, Brower: % userAgent - % message""
}",
                "jsnlog1");
        }

        [TestMethod]
        public void jsnlog2()
        {
            TestDemo(
                @"
<jsnlog serverSideLogger=""jslogger"">
</jsnlog>",
                @"
new JsnlogConfiguration {
    serverSideLogger=""jslogger""
}",
                "jsnlog2");
        }

        [TestMethod]
        public void jsnlog3()
        {
            TestDemo(
                @"
<jsnlog enabled=""false"">
</jsnlog>",
                @"
new JsnlogConfiguration {
    enabled=false
}",
                "jsnlog3");
        }

        // ---------------------------------------------------------------------------------

        [TestMethod]
        public void ajaxappender1()
        {
            TestDemo(
                @"
<jsnlog>
	<ajaxAppender 
		name=""appender1"" 
		storeInBufferLevel=""TRACE"" 
		level=""WARN"" 
		sendWithBufferLevel=""FATAL"" 
		bufferSize=""20""/>
	<logger appenders=""appender1""/>
</jsnlog>
",
                @"
new JsnlogConfiguration {
    ajaxAppenders=new List<AjaxAppender> {
        new AjaxAppender {
		    name=""appender1"", 
		    storeInBufferLevel=""TRACE"", 
		    level=""WARN"", 
		    sendWithBufferLevel=""FATAL"", 
		    bufferSize=20
        }
    },
    loggers=new List<Logger> {
        new Logger {
            appenders=""appender1""
        }
    }
}",
                "ajaxappender1");
        }

        // ---------------------------------------------------------------------------------

        [TestMethod]
        public void consoleappender1()
        {
            TestDemo(
                @"
<jsnlog>
	<!-- ""mylogger"" logs to just the console -->
	<consoleAppender name=""consoleAppender"" />
	<logger name=""mylogger"" appenders=""consoleAppender"" />
</jsnlog>
",
                @"
// ""mylogger"" logs to just the console
new JsnlogConfiguration {
    consoleAppenders=new List<ConsoleAppender> {
        new ConsoleAppender {
		    name=""consoleAppender""
        }
    },
    loggers=new List<Logger> {
        new Logger {
            name=""mylogger"",
            appenders=""consoleAppender""
        }
    }
}",
                "consoleappender1");
        }

        [TestMethod]
        public void consoleappender2()
        {
            TestDemo(
                @"
<jsnlog>
	<!-- ""mylogger"" logs to both the server and the console -->
    <consoleAppender name=""consoleAppender"" />
    <ajaxAppender name=""ajaxAppender"" />
	<logger name=""mylogger"" appenders=""ajaxAppender;consoleAppender"" />
</jsnlog>
",
                @"
// ""mylogger"" logs to both the server and the console
new JsnlogConfiguration {
    consoleAppenders=new List<ConsoleAppender> {
        new ConsoleAppender {
		    name=""consoleAppender""
        }
    },
    ajaxAppenders=new List<AjaxAppender> {
        new AjaxAppender {
		    name=""ajaxAppender""
        }
    },
    loggers=new List<Logger> {
        new Logger {
            name=""mylogger"",
            appenders=""ajaxAppender;consoleAppender""
        }
    }
}",
                "consoleappender2");
        }

        [TestMethod]
        public void consoleappender3()
        {
            TestDemo(
                @"
<jsnlog>
	<!-- Debugging: all loggers log to both the server and the console -->
    <consoleAppender name=""consoleAppender"" />
    <ajaxAppender name=""ajaxAppender"" />
	<logger appenders=""ajaxAppender;consoleAppender"" />
</jsnlog>
",
                @"
// Debugging: all loggers log to both the server and the console
new JsnlogConfiguration {
    consoleAppenders=new List<ConsoleAppender> {
        new ConsoleAppender {
		    name=""consoleAppender""
        }
    },
    ajaxAppenders=new List<AjaxAppender> {
        new AjaxAppender {
		    name=""ajaxAppender""
        }
    },
    loggers=new List<Logger> {
        new Logger {
            appenders=""ajaxAppender;consoleAppender""
        }
    }
}",
                "consoleappender3");
        }

        [TestMethod]
        public void consoleappender4()
        {
            TestDemo(
                @"
<jsnlog>
	<!-- Production: loggers log to the server only -->
    <ajaxAppender name=""ajaxAppender"" />
	<logger appenders=""ajaxAppender;consoleAppender"" />
</jsnlog>
",
                @"
// Production: loggers log to the server only
new JsnlogConfiguration {
    ajaxAppenders=new List<AjaxAppender> {
        new AjaxAppender {
		    name=""ajaxAppender""
        }
    },
    loggers=new List<Logger> {
        new Logger {
            appenders=""ajaxAppender;consoleAppender""
        }
    }
}",
                "consoleappender4");
        }

        // ---------------------------------------------------------------------------------

        /// <summary>
        /// Ensures that the xml will be serialised by JSNLog to the code in csharp.
        /// Also writes HTML to d:\temp\demos.html with premade html for example tabs.
        /// </summary>
        /// <param name="configXml"></param>
        /// <param name="csharp"></param>
        public void TestDemo(string configXml, string csharp, string demoId)
        {
            // Testing to ensure xml and code are the same

            XmlElement xe = TestUtils.ConfigToXe(configXml);
            var jsnlogConfigurationFromXml = XmlHelpers.DeserialiseXml<JsnlogConfiguration>(xe);

            JsnlogConfiguration jsnlogConfigurationFromCode = (JsnlogConfiguration)TestUtils.Eval(csharp);

            TestUtils.EnsureEqualJsnlogConfiguration(jsnlogConfigurationFromXml, jsnlogConfigurationFromCode);

            // Write partial

            var sb = new StringBuilder();
            sb.AppendLine(string.Format("@* GENERATED CODE - by class DemoTests in JSNLog.Tests project. Demo {0}. *@", demoId));

            sb.AppendLine(@"<div class=""commontabs""><div data-tab=""Web.config"">");
            sb.AppendLine(@"");
            sb.AppendLine(string.Format(@"<pre>{0}</pre>", ScrubbedCode(configXml)));
            sb.AppendLine(@"");
            sb.AppendLine(@"</div><div data-tab=""JsnlogConfiguration"">");
            sb.AppendLine(@"");
            sb.AppendLine(string.Format(@"<pre>{0}</pre>", ScrubbedCode(csharp)));
            sb.AppendLine(@"");
            sb.AppendLine(@"</div></div>");

            string path = Path.Combine(_demosDirectory, string.Format("_{0}.cshtml", demoId));
            string content = sb.ToString();

            bool fileExists = File.Exists(path);
            Assert.IsFalse(fileExists, string.Format("{0} already exists", path));

            System.IO.File.WriteAllText(path, content);
        }

        private string ScrubbedCode(string code)
        {
            return HttpUtility.HtmlEncode(code.Trim().Replace("\t", "    "));
        }

    }
}

