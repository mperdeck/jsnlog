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
            sb.AppendLine(string.Format(@"<pre>{0}</pre>", HttpUtility.HtmlEncode(configXml.Trim())));
            sb.AppendLine(@"");
            sb.AppendLine(@"</div><div data-tab=""JsnlogConfiguration"">");
            sb.AppendLine(@"");
            sb.AppendLine(string.Format(@"<pre>{0}</pre>", HttpUtility.HtmlEncode(csharp.Trim())));
            sb.AppendLine(@"");
            sb.AppendLine(@"</div></div>");

            string path = Path.Combine(_demosDirectory, string.Format("_{0}.cshtml", demoId));
            string content = sb.ToString();

            bool fileExists = File.Exists(path);
            Assert.IsFalse(fileExists, string.Format("{0} already exists", path));

            System.IO.File.WriteAllText(path, content);
        }

    }
}

