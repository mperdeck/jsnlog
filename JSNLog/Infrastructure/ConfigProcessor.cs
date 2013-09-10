using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Web.Configuration;
using JSNLog.Exceptions;
using JSNLog.Infrastructure;
using System.Text.RegularExpressions;
using System.Web;
using System.Reflection;
using JSNLog.ValueInfos;

namespace JSNLog.Infrastructure
{
    public class ConfigProcessor
    {
        private List<XmlHelpers.TagInfo> topLeveltagInfos = null;

        /// <summary>
        /// Processes a configuration (such as the contents of the jsnlog element in web.config).
        /// 
        /// The configuration is processed into JavaScript, which configures the jsnlog client side library.
        /// </summary>
        /// <param name="requestId">
        /// requestId to be passed to the JSNLog library when setting its options.
        /// Could be null (when user didn't provide a request id).
        /// In that case, this method creates a request id itself.
        /// </param>
        /// <param name="xe">
        /// XmlElement to be processed
        /// </param>
        /// <param name="sb">
        /// All JavaScript needs to be written to this string builder.
        /// </param>
        public void ProcessRoot(XmlElement xe, string requestId, StringBuilder sb)
        {
            string userIp = HttpContext.Current.Request.UserHostAddress;
            ProcessRootExec(xe, sb, VirtualPathUtility.ToAbsolute, userIp, requestId ?? Utils.CreateRequestId(), true);
        }

        // This version is not reliant on sitting in a web site, so can be unit tested.
        // generateClosure - if false, no function closure is generated around the generated JS code. Only set to false when unit testing.
        public void ProcessRootExec(XmlElement xe, StringBuilder sb, Func<string, string> virtualToAbsoluteFunc, string userIp, string requestId, bool generateClosure)
        {
            string loggerProductionLibraryVirtualPath = XmlHelpers.OptionalAttribute(xe, "productionLibraryPath", "");
            bool loggerEnabled = bool.Parse(XmlHelpers.OptionalAttribute(xe, "enabled", "true", Constants.RegexBool));

            string loggerProductionLibraryPath = null;
            if (!string.IsNullOrEmpty(loggerProductionLibraryVirtualPath))
            {
                loggerProductionLibraryPath = virtualToAbsoluteFunc(loggerProductionLibraryVirtualPath);
            }

            if (!loggerEnabled)
            {
                if (!string.IsNullOrWhiteSpace(loggerProductionLibraryPath))
                {
                    JavaScriptHelpers.WriteScriptTag(loggerProductionLibraryPath, sb);
                }

                JavaScriptHelpers.WriteJavaScriptBeginTag(sb);
                Utils.ProcessOptionAttributes(Constants.JsLogObjectName, xe, Constants.JSNLogAttributes, null, sb);
                JavaScriptHelpers.WriteJavaScriptEndTag(sb);

                return;
            }

            if (!string.IsNullOrWhiteSpace(loggerProductionLibraryPath))
            {
                JavaScriptHelpers.WriteScriptTag(loggerProductionLibraryPath, sb);
            }

            JavaScriptHelpers.WriteJavaScriptBeginTag(sb);
            if (generateClosure) { JavaScriptHelpers.WriteLine("(function () {", sb); }

            // Generate setOptions for JSNLog object itself

            AttributeValueCollection attributeValues = new AttributeValueCollection();

            attributeValues[Constants.JsLogObjectClientIpOption] = new Value(userIp, new StringValue());
            attributeValues[Constants.JsLogObjectRequestIdOption] = new Value(requestId, new StringValue());

            Utils.ProcessOptionAttributes(Constants.JsLogObjectName, xe, Constants.JSNLogAttributes,
                attributeValues, sb);

            // Process all loggers and appenders

            Dictionary<string, string> appenderNames = new Dictionary<string, string>();
            Sequence sequence = new Sequence();

            // -----------------
            // First process all assembly tags

            topLeveltagInfos =
                new List<XmlHelpers.TagInfo>(
                    new[] {
                            new XmlHelpers.TagInfo(Constants.TagAssembly, ProcessAssembly, Constants.AssemblyAttributes, (int)Constants.OrderNbr.Assembly)
                        });

            XmlHelpers.ProcessNodeList(
                xe.ChildNodes,
                topLeveltagInfos.Where(t => t.Tag == Constants.TagAssembly).ToList(),
                null, appenderNames, sequence, sb,
                string.Format("^{0}*", Constants.TagAssembly));

            // -----------------
            // The elements (if any) from external assemblies have now been loaded (with the assembly elements).
            // Now add the elements from the executing assembly after the external ones. 
            // This way, logger elements are processed last - after any new appenders.

            AddAssemblyTagInfos(Assembly.GetExecutingAssembly());

            // Now process the external and internal elements, but not the assembly elements.

            XmlHelpers.ProcessNodeList(
                xe.ChildNodes,
                topLeveltagInfos,
                null, appenderNames, sequence, sb,
                string.Format("^((?!{0}).)*$", Constants.TagAssembly));

            // -------------

            if (generateClosure) { JavaScriptHelpers.WriteLine("}());", sb); }
            JavaScriptHelpers.WriteJavaScriptEndTag(sb);
        }

        private void ProcessAssembly(XmlElement xe, string parentName, Dictionary<string, string> appenderNames, Sequence sequence,
            IEnumerable<AttributeInfo> assemblyAttributes, StringBuilder sb)
        {
            if (xe == null) { return; }

            string assemblyName = XmlHelpers.RequiredAttribute(xe, "name");
            AddAssemblyTagInfos(Assembly.Load(assemblyName));
        }

        /// <summary>
        /// Calls Init on all classes in the given assembly that implement IElement.
        /// Adds their TagInfos to the end of topLeveltagInfos.
        /// </summary>
        /// <param name="assembly"></param>
        private void AddAssemblyTagInfos(Assembly assembly)
        {
            List<IElement> types = new List<IElement>(
                from t in assembly.GetTypes()
                where t.IsClass && t.GetInterfaces().Contains(typeof(IElement))
                select Activator.CreateInstance(t) as IElement
            );

            var tagInfos = new List<XmlHelpers.TagInfo>();
            foreach(IElement type in types)
            {
                XmlHelpers.TagInfo tagInfo;
                type.Init(out tagInfo);
                tagInfos.Add(tagInfo);
            }

            var sortedTagInfos = tagInfos.OrderBy(t=>(int)t.OrderNbr);

            topLeveltagInfos.AddRange(sortedTagInfos);
        }
    }
}
