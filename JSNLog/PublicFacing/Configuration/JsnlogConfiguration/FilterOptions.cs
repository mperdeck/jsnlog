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
    public class FilterOptions: ICanCreateJsonFields 
    {
        [XmlAttribute]
        public string level { get; set; }

        [XmlAttribute]
        public string ipRegex { get; set; }

        [XmlAttribute]
        public string userAgentRegex { get; set; }

        [XmlAttribute]
        public string disallow { get; set; }

        // --------------------------------------------------------

        protected string FieldLevel { get { return "level"; } }

        // Implement ICanCreateJsonFields
        public virtual void AddJsonFields(IList<string> jsonFields, Dictionary<string, string> appenderNames, 
            Func<string, string> virtualToAbsoluteFunc)
        {
            var stringValue = new StringValue();

            JavaScriptHelpers.AddJsonField(jsonFields, FieldLevel, level, new LevelValue());
            JavaScriptHelpers.AddJsonField(jsonFields, "ipRegex", ipRegex, stringValue);
            JavaScriptHelpers.AddJsonField(jsonFields, "userAgentRegex", userAgentRegex, stringValue);
            JavaScriptHelpers.AddJsonField(jsonFields, "disallow", disallow, stringValue);
        }
    }
}
