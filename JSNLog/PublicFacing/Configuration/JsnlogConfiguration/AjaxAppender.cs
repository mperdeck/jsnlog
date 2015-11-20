using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml.Serialization;
using JSNLog.Infrastructure;
using JSNLog.ValueInfos;

namespace JSNLog
{
    public class AjaxAppender : Appender, ICanCreateJsonFields, ICanCreateElement
    {
        [XmlAttribute]
        public string url { get; set; }

        public AjaxAppender()
        {
            // Set default values. If an element is not given in the XML or JSON,
            // the deserializer will simply not set it.
            url = "/jsnlog.logger";
        }

        // --------------------------------------------------------

        protected string FieldUrl { get { return "url"; } }

        public void CreateElement(StringBuilder sb, Dictionary<string, string> appenderNames, int sequence, Func<string, string> virtualToAbsoluteFunc)
        {
            CreateAppender(sb, appenderNames, sequence, virtualToAbsoluteFunc, "createAjaxAppender", "ajaxAppender");
        }

        // Implement ICanCreateJsonFields
        public override void AddJsonFields(IList<string> jsonFields, Dictionary<string, string> appenderNames, Func<string, string> virtualToAbsoluteFunc)
        {
            JavaScriptHelpers.AddJsonField(jsonFields, FieldUrl, url, new UrlValue(virtualToAbsoluteFunc));

            base.AddJsonFields(jsonFields, appenderNames, virtualToAbsoluteFunc);
        }
    }
}
