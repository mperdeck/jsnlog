using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using JSNLog.Infrastructure;
using JSNLog.Exceptions;
using JSNLog.ValueInfos;

namespace JSNLog.Elements
{
    public class LoggerElement : ElementBase, IElement 
    {
        public void Init(out XmlHelpers.TagInfo tagInfo)
        {
            tagInfo = new XmlHelpers.TagInfo(Constants.TagLogger, ProcessLogger, Constants.LoggerAttributes, (int)Constants.OrderNbr.Logger);
        }

        // --------------------------------

        public void ProcessLogger(XmlElement xe, string parentName, Dictionary<string, string> appenderNames, Sequence sequence,
            IEnumerable<AttributeInfo> loggerAttributes, StringBuilder sb)
        {
            if (xe == null) { return; }

            var appendersValue = new AppendersValue(appenderNames);
            string appenders = XmlHelpers.OptionalAttribute(xe, "appenders", null, appendersValue.ValidValueRegex);
            string loggerName = XmlHelpers.OptionalAttribute(xe, "name", "");

            JavaScriptHelpers.GenerateLogger(Constants.JsLoggerVariable, loggerName, sb);

            AttributeValueCollection attributeValues = new AttributeValueCollection();
            if (appenders != null)
            {
                attributeValues[Constants.JsLoggerAppendersOption] = new Value(appenders, appendersValue);
            }

            Utils.ProcessOptionAttributes(Constants.JsLoggerVariable, xe, loggerAttributes, attributeValues, sb);
        }
    }
}
