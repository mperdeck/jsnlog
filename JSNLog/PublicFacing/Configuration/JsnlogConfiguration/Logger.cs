using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml.Serialization;
using JSNLog.Infrastructure;
using JSNLog.Exceptions;
using JSNLog.ValueInfos;

namespace JSNLog
{
    public class Logger : FilterOptions, ICanCreateJsonFields, ICanCreateElement
    {
        [XmlAttribute]
        public string appenders { get; set; }

        [XmlAttribute]
        public string name { get; set; }

        [XmlElement("onceOnly")]
        public List<OnceOnlyOptions> onceOnlies = new List<OnceOnlyOptions>();

        // --------------------------------------------------------

        protected string ElementOnceOnly { get { return "onceOnly"; } }
        protected string FieldRegex { get { return "regex"; } }

        protected string FieldAppenders { get { return "appenders"; } }
        protected string FieldOnceOnly { get { return "onceOnly"; } }

        // Implement ICanCreateElement
        public void CreateElement(StringBuilder sb, Dictionary<string, string> appenderNames, int sequence, 
            Func<string, string> virtualToAbsoluteFunc)
        {
            try
            {
                string jsVariableName = string.Format("{0}{1}", Constants.JsLoggerVariablePrefix, sequence);
                JavaScriptHelpers.GenerateLogger(jsVariableName, name, sb);

                JavaScriptHelpers.GenerateSetOptions(jsVariableName, this, appenderNames, virtualToAbsoluteFunc, 
                    sb, null);
            }
            catch (Exception e)
            {
                string displayName = String.IsNullOrEmpty(name) ? "<nameless root logger>" : name; ;
                throw new ConfigurationException(displayName, e);
            }
        }

        // Implement ICanCreateJsonFields
        public override void AddJsonFields(IList<string> jsonFields, Dictionary<string, string> appenderNames, Func<string, string> virtualToAbsoluteFunc)
        {
            JavaScriptHelpers.AddJsonField(jsonFields, FieldAppenders, appenders, new AppendersValue(appenderNames));

            if (onceOnlies != null)
            {
                if (onceOnlies.Any(o=>o.regex == null))
                {
                    throw new MissingAttributeException(ElementOnceOnly, FieldRegex);
                }

                JavaScriptHelpers.AddJsonField(jsonFields, FieldOnceOnly,
                    onceOnlies.Select(o=>o.regex), new StringValue());
            }

            base.AddJsonFields(jsonFields, appenderNames, virtualToAbsoluteFunc);
        }
    }
}
