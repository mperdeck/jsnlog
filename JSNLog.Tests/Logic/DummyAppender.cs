using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JSNLog.Infrastructure;
using JSNLog.Elements;
using JSNLog;
using System.Xml;
using System.Text;

namespace JSNLog.Tests.Logic
{
    public class DummyAppender : AppenderElementBase, IElement 
    {
        public void Init(out XmlHelpers.TagInfo tagInfo)
        {
            tagInfo = new XmlHelpers.TagInfo("dummyAppender", ProcessDummyAppender, Constants.AppenderAttributes, 0);
        }

        // --------------------------------

        public void ProcessDummyAppender(XmlElement xe, string parentName, Dictionary<string, string> appenderNames, Sequence sequence,
            IEnumerable<AttributeInfo> appenderAttributes, StringBuilder sb)
        {
            // Ensure no child xml elements have been used with the appender element
            // by passing in an empty list.
            var childTagInfos = new List<XmlHelpers.TagInfo>();

            ProcessAppender(xe, parentName, appenderNames, sequence,
                appenderAttributes, "createDummyAppender", childTagInfos, sb);
        }
    }
}
