using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using JSNLog.Infrastructure;
using JSNLog.Exceptions;

namespace JSNLog.Elements
{
    public class ConsoleAppenderElement : AppenderElementBase, IElement 
    {
        public void Init(out XmlHelpers.TagInfo tagInfo)
        {
            tagInfo = new XmlHelpers.TagInfo(Constants.TagConsoleAppender, ProcessConsoleAppender, Constants.ConsoleAppenderAttributes, (int)Constants.OrderNbr.ConsoleAppender);
        }

        // --------------------------------

        public void ProcessConsoleAppender(XmlElement xe, string parentName, Dictionary<string, string> appenderNames, Sequence sequence,
            IEnumerable<AttributeInfo> appenderAttributes, StringBuilder sb)
        {
            // Ensure no child xml elements have been used with the appender element
            // by passing in an empty list.
            var childTagInfos = new List<XmlHelpers.TagInfo>();

            ProcessAppender(xe, parentName, appenderNames, sequence,
                appenderAttributes, "createConsoleAppender", childTagInfos, sb);
        }
    }
}
