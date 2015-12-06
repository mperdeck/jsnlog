using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using JSNLog.Infrastructure;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JSNLog.Tests.Logic
{
    public class TestUtils
    {
        /// <summary>
        /// Generates JS to store the timestamp for a log action
        /// </summary>
        /// <param name="seq">
        /// The index into tests of the log action for which the timestamp is stored.
        /// </param>
        /// <returns></returns>
        private static string StoreTimestampJs(int seq)
        {
            string js = string.Format("__timestamp{0} = (new Date).getTime();", seq);
            return js;
        }

        /// <summary>
        /// Generates JS expression to get the timestamp for a log action
        /// </summary>
        /// <param name="seq"></param>
        /// <returns></returns>
        private static string GetTimestampJs(int seq)
        {
            string js = string.Format("__timestamp{0}", seq);
            return js;
        }

        private static string Msg(int seq, int level, string logger)
        {
            return string.Format("msg{0} level: {1}, logger: {2}", seq, level, logger);
        }

        /// <summary>
        /// Creates JSON with all log items expected for a given check number.
        /// </summary>
        /// <param name="checkNbr"></param>
        /// <param name="tests"></param>
        /// <returns></returns>
        private static string Expected(int checkNbr, IEnumerable<T> tests)
        {
            var sb = new StringBuilder();

            sb.AppendLine(@"[");

            int seq = 0;
            bool first = true;
            foreach (T t in tests)
            {
                if (t.CheckExpected == checkNbr)
                {
                    string msg = t.ExpectedMsg ?? Msg(seq, t.Level, t.Logger);
                    string timestamp = GetTimestampJs(seq);

                    sb.AppendLine(string.Format("  {0}{{", first ? "" : ","));
                    sb.AppendLine(string.Format("    l: {0},", t.Level));
                    sb.AppendLine(string.Format("    m: '{0}',", msg));
                    sb.AppendLine(string.Format("    n: '{0}',", t.Logger));
                    sb.AppendLine(string.Format("    t: {0}", timestamp));
                    sb.AppendLine("  }");

                    first = false;
                }

                seq++;
            }

            sb.AppendLine("]");
            string js = sb.ToString();

            return js;
        }

        /// <summary>
        /// Returns all javascript to set up a test.
        /// The generated javascript is within an immediately executing function, so it sits in its own namespace.
        /// 
        /// </summary>
        /// <param name="configXml">
        /// String with xml with the JSNLog root element, as would be used in a web.config file.
        /// </param>
        /// <param name="userIp">
        /// Simulated IP address of the client.
        /// </param>
        /// <param name="requestId">
        /// Simulated request id.
        /// </param>
        /// <returns></returns>
        public static string SetupTest(string userIp, string requestId, string configXml, IEnumerable<T> tests)
        {
            var sb = new StringBuilder();

            // Set config cache in JavascriptLogging to contents of xe
            SetConfigCache(configXml);

            var configProcessor = new ConfigProcessor();
            configProcessor.ProcessRootExec(sb, s => s, userIp, requestId, false);

            sb.AppendLine(@"<script type=""text/javascript"">");
            sb.AppendLine("(function () {");

            sb.AppendLine("JL.setOptions({ 'defaultBeforeSend': TestUtils.beforeSend });");

            int seq = 0;
            foreach (T t in tests)
            {
                if (t.Level > -1)
                {
                    // Level given, so generate call to logger.

                    string msg = t.LogObject ?? @"""" + Msg(seq, t.Level, t.Logger) + @"""";
                    string logCallJs = string.Format(@"JL(""{0}"").log({1}, {2});", t.Logger, t.Level, msg);
                    string storeTimestampJs = StoreTimestampJs(seq);
                    sb.AppendLine(logCallJs + " " + storeTimestampJs);
                }

                if (t.CheckNbr > -1)
                {
                    // CheckNbr given, so do a check

                    // Create JSON object with all expected log entries
                    string expected = Expected(t.CheckNbr, tests);

                    // Generate check js
                    string checkJs = string.Format("TestUtils.Check({0}, {1}, {2});", t.CheckAppender, t.CheckNbr, expected);
                    sb.AppendLine("");
                    sb.AppendLine(checkJs);
                    sb.AppendLine("// ----------------------");
                    sb.AppendLine("");
                }

                if (!string.IsNullOrEmpty(t.Header))
                {
                    sb.AppendLine(string.Format("$('body').append('<h3>{0}</h3>');", t.Header));
                }

                seq++;
            }

            // Remove the "running" heading. If the tests somehow crash, we won't get here and the running header will remain,
            // showing something is wrong.
            sb.AppendLine("$('#running').remove();");

            sb.AppendLine("})();");
            sb.AppendLine("</script>");
            string js = sb.ToString();

            return js;
        }

        /// <summary>
        /// Returns number of milli seconds from 1/1/1970 to the given UTC timestamp. 
        /// </summary>
        /// <param name="dtUtc"></param>
        /// <returns></returns>
        public static Double MsSince1970(DateTime dtUtc)
        {
            TimeSpan ts = dtUtc - new DateTime(1970, 1, 1);
            Double result = ts.TotalMilliseconds;
            return result;
        }

        public static XmlElement ConfigToXe(string configXml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(configXml);
            XmlElement xe = (XmlElement)doc.DocumentElement;

            return xe;
        }

        internal static void SetConfigCache(string configXml, ILogger logger = null)
        {
            // Set config cache in JavascriptLogging to contents of xe
            XmlElement xe = TestUtils.ConfigToXe(configXml);
            JavascriptLogging.SetJsnlogConfiguration(null, logger);
            JavascriptLogging.GetJsnlogConfiguration(() => xe);
        }

        public static string SetupRequestIdTest(string requestId, string configXml)
        {
            var sb = new StringBuilder();
            SetConfigCache(configXml);

            var configProcessor = new ConfigProcessor();
            configProcessor.ProcessRoot(requestId, sb);
            string js = sb.ToString();

            return js;
        }

        /// <summary>
        /// Evalutes the passed in string as C# code.
        /// Returns the resulting value.
        /// </summary>
        /// <param name="sCSCode"></param>
        /// <returns></returns>
        public static object Eval(string sCSCode)
        {
            CSharpCodeProvider c = new CSharpCodeProvider();
            ICodeCompiler icc = c.CreateCompiler();
            CompilerParameters cp = new CompilerParameters();

            cp.ReferencedAssemblies.Add("system.dll");
            cp.ReferencedAssemblies.Add("system.xml.dll");
            cp.ReferencedAssemblies.Add("JSNLog.dll");

            cp.CompilerOptions = "/t:library";
            cp.GenerateInMemory = true;

            StringBuilder sb = new StringBuilder("");
            sb.Append("using System;\n");
            sb.Append("using System.Xml;\n");
            sb.Append("using System.Collections.Generic;\n");
            sb.Append("using JSNLog;\n");

            sb.Append("namespace CSCodeEvaler{ \n");
            sb.Append("public class CSCodeEvaler{ \n");
            sb.Append("public object EvalCode(){\n");
            sb.Append("return " + sCSCode + "; \n");
            sb.Append("} \n");
            sb.Append("} \n");
            sb.Append("}\n");

            CompilerResults cr = icc.CompileAssemblyFromSource(cp, sb.ToString());
            if (cr.Errors.Count > 0)
            {
                throw new Exception("Error evaluating cs code: " + cr.Errors[0].ErrorText);
            }

            System.Reflection.Assembly a = cr.CompiledAssembly;
            object o = a.CreateInstance("CSCodeEvaler.CSCodeEvaler");

            Type t = o.GetType();
            MethodInfo mi = t.GetMethod("EvalCode");

            object s = mi.Invoke(o, null);
            return s;
        }

        public static void EnsureEqualJsnlogConfiguration(JsnlogConfiguration jc1, JsnlogConfiguration jc2)
        {
            Assert.AreEqual(jc1.enabled, jc2.enabled);
            Assert.AreEqual(jc1.maxMessages, jc2.maxMessages);
            Assert.AreEqual(jc1.defaultAjaxUrl, jc2.defaultAjaxUrl);
            Assert.AreEqual(jc1.corsAllowedOriginsRegex, jc2.corsAllowedOriginsRegex);
            Assert.AreEqual(jc1.serverSideLogger, jc2.serverSideLogger);
            Assert.AreEqual(jc1.serverSideLevel, jc2.serverSideLevel);
            Assert.AreEqual(jc1.serverSideMessageFormat, jc2.serverSideMessageFormat);
            Assert.AreEqual(jc1.dateFormat, jc2.dateFormat);
            Assert.AreEqual(jc1.productionLibraryPath, jc2.productionLibraryPath);

            EnsureListsEqual(jc1.ajaxAppenders, jc2.ajaxAppenders, EnsureEqualAjaxAppender);
            EnsureListsEqual(jc1.consoleAppenders, jc2.consoleAppenders, EnsureEqualConsoleAppender);
            EnsureListsEqual(jc1.loggers, jc2.loggers, EnsureEqualLogger);
        }

        public static void EnsureEqualAjaxAppender(AjaxAppender aa1, AjaxAppender aa2)
        {
            Assert.AreEqual(aa1.url, aa2.url);
            EnsureEqualAppender(aa1, aa2);
        }

        public static void EnsureEqualConsoleAppender(ConsoleAppender ca1, ConsoleAppender ca2)
        {
            EnsureEqualAppender(ca1, ca2);
        }

        public static void EnsureEqualAppender(Appender a1, Appender a2)
        {
            Assert.AreEqual(a1.name, a2.name);
            Assert.AreEqual(a1.sendWithBufferLevel, a2.sendWithBufferLevel);
            Assert.AreEqual(a1.storeInBufferLevel, a2.storeInBufferLevel);
            Assert.AreEqual(a1.bufferSize, a2.bufferSize);
            Assert.AreEqual(a1.batchSize, a2.batchSize);

            EnsureEqualFilterOptions(a1, a2);
        }

        public static void EnsureEqualLogger(Logger l1, Logger l2)
        {
            Assert.AreEqual(l1.appenders, l2.appenders);
            Assert.AreEqual(l1.name, l2.name);

            EnsureListsEqual(l1.onceOnlies, l2.onceOnlies, EnsureEqualOnceOnlyOptions);

            EnsureEqualFilterOptions(l1, l2);
        }

        internal static void EnsureEqualFilterOptions(FilterOptions fc1, FilterOptions fc2)
        {
            Assert.AreEqual(fc1.level, fc2.level);
            Assert.AreEqual(fc1.ipRegex, fc2.ipRegex);
            Assert.AreEqual(fc1.userAgentRegex, fc2.userAgentRegex);
            Assert.AreEqual(fc1.disallow, fc2.disallow);
        }

        public static void EnsureEqualOnceOnlyOptions(OnceOnlyOptions oo1, OnceOnlyOptions oo2)
        {
            Assert.AreEqual(oo1.regex, oo2.regex);
        }

        public static void EnsureListsEqual<T>(List<T> list1i, List<T> list2i, Action<T, T> ensurer)
        {
            var list1 = list1i ?? new List<T>();
            var list2 = list2i ?? new List<T>();

            int nbrElements = list1.Count;
            Assert.AreEqual(nbrElements, list2.Count);

            for(int i = 0; i < nbrElements; i++)
            {
                ensurer(list1[i], list2[i]);
            }
        }

    }
}