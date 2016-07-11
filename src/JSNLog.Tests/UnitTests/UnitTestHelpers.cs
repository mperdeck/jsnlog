using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using System.Xml;
using System.Text;
using JSNLog.Infrastructure;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Reflection;
using JSNLog.Tests.Common;

namespace JSNLog.Tests.UnitTests
{
    public class UnitTestHelpers
    {
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

        /// <summary>
        /// Evalutes the passed in string as C# code.
        /// Returns the resulting value.
        /// </summary>
        /// <param name="sCSCode"></param>
        /// <returns></returns>
        public static object Eval(string sCSCode)
        {
            CSharpCodeProvider c = new CSharpCodeProvider();
#pragma warning disable CS0618
            ICodeCompiler icc = c.CreateCompiler();
#pragma warning restore CS0618
            CompilerParameters cp = new CompilerParameters();

            cp.ReferencedAssemblies.Add("system.dll");
            cp.ReferencedAssemblies.Add("system.xml.dll");

            // If it can't find JSNLog.dll, make sure that "Produce outputs on build" property of JSNLog project is checked.
            cp.ReferencedAssemblies.Add(TestConstants._jsnlogDllDirectory);

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
            Assert.Equal(jc1.enabled, jc2.enabled);
            Assert.Equal(jc1.maxMessages, jc2.maxMessages);
            Assert.Equal(jc1.defaultAjaxUrl, jc2.defaultAjaxUrl);
            Assert.Equal(jc1.corsAllowedOriginsRegex, jc2.corsAllowedOriginsRegex);
            Assert.Equal(jc1.serverSideLogger, jc2.serverSideLogger);
            Assert.Equal(jc1.serverSideLevel, jc2.serverSideLevel);
            Assert.Equal(jc1.serverSideMessageFormat, jc2.serverSideMessageFormat);
            Assert.Equal(jc1.dateFormat, jc2.dateFormat);
            Assert.Equal(jc1.productionLibraryPath, jc2.productionLibraryPath);

            EnsureListsEqual(jc1.ajaxAppenders, jc2.ajaxAppenders, EnsureEqualAjaxAppender);
            EnsureListsEqual(jc1.consoleAppenders, jc2.consoleAppenders, EnsureEqualConsoleAppender);
            EnsureListsEqual(jc1.loggers, jc2.loggers, EnsureEqualLogger);
        }

        public static void EnsureEqualAjaxAppender(AjaxAppender aa1, AjaxAppender aa2)
        {
            Assert.Equal(aa1.url, aa2.url);
            EnsureEqualAppender(aa1, aa2);
        }

        public static void EnsureEqualConsoleAppender(ConsoleAppender ca1, ConsoleAppender ca2)
        {
            EnsureEqualAppender(ca1, ca2);
        }

        public static void EnsureEqualAppender(Appender a1, Appender a2)
        {
            Assert.Equal(a1.name, a2.name);
            Assert.Equal(a1.sendWithBufferLevel, a2.sendWithBufferLevel);
            Assert.Equal(a1.storeInBufferLevel, a2.storeInBufferLevel);
            Assert.Equal(a1.bufferSize, a2.bufferSize);
            Assert.Equal(a1.batchSize, a2.batchSize);

            EnsureEqualFilterOptions(a1, a2);
        }

        public static void EnsureEqualLogger(Logger l1, Logger l2)
        {
            Assert.Equal(l1.appenders, l2.appenders);
            Assert.Equal(l1.name, l2.name);

            EnsureListsEqual(l1.onceOnlies, l2.onceOnlies, EnsureEqualOnceOnlyOptions);

            EnsureEqualFilterOptions(l1, l2);
        }

        internal static void EnsureEqualFilterOptions(FilterOptions fc1, FilterOptions fc2)
        {
            Assert.Equal(fc1.level, fc2.level);
            Assert.Equal(fc1.ipRegex, fc2.ipRegex);
            Assert.Equal(fc1.userAgentRegex, fc2.userAgentRegex);
            Assert.Equal(fc1.disallow, fc2.disallow);
        }

        public static void EnsureEqualOnceOnlyOptions(OnceOnlyOptions oo1, OnceOnlyOptions oo2)
        {
            Assert.Equal(oo1.regex, oo2.regex);
        }

        public static void EnsureListsEqual<T>(List<T> list1i, List<T> list2i, Action<T, T> ensurer)
        {
            var list1 = list1i ?? new List<T>();
            var list2 = list2i ?? new List<T>();

            int nbrElements = list1.Count;
            Assert.Equal(nbrElements, list2.Count);

            for (int i = 0; i < nbrElements; i++)
            {
                ensurer(list1[i], list2[i]);
            }
        }
    }
}
