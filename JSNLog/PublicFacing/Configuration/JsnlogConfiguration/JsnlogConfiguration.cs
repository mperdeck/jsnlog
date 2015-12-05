using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml.Serialization;
using JSNLog.Infrastructure;
using JSNLog.ValueInfos;
using JSNLog.Exceptions;

namespace JSNLog
{
    [XmlRoot("jsnlog")]
    public class JsnlogConfiguration : ICanCreateJsonFields
    {
        [XmlAttribute]
        public bool enabled { get; set; }

        [XmlAttribute]
        public uint maxMessages { get; set; }

        [XmlAttribute]
        public string defaultAjaxUrl { get; set; }

        [XmlAttribute]
        public string corsAllowedOriginsRegex { get; set; }

        [XmlAttribute]
        public string serverSideLogger { get; set; }

        [XmlAttribute]
        public string serverSideLevel { get; set; }

        [XmlAttribute]
        public string serverSideMessageFormat { get; set; }

        [XmlAttribute]
        public string dateFormat { get; set; }

        [XmlAttribute]
        public string productionLibraryPath { get; set; }

        // Be sure to make everything Properties. While the XML serializer handles fields ok,
        // the JSON serializer used in ASP.NET 5 doesn't.

        [XmlElement("logger")]
        public List<Logger> loggers { get; set; }

        [XmlElement("ajaxAppender")]
        public List<AjaxAppender> ajaxAppenders { get; set; }

        [XmlElement("consoleAppender")]
        public List<ConsoleAppender> consoleAppenders { get; set; }

        public JsnlogConfiguration()
        {
            // Set default values. If an element is not given in the XML or JSON,
            // the deserializer will simply not set it.
            enabled = true;
            maxMessages = int.MaxValue;
            serverSideMessageFormat = "%message";
            dateFormat = "o";

            // Do not set default for defaultAjaxUrl here. Its default is set in jsnlog.js.
        }

        // --------------------------------------------------------

        protected string FieldEnabled { get { return "enabled"; } }
        protected string FieldMaxMessages { get { return "maxMessages"; } }
        protected string FieldDefaultAjaxUrl { get { return "defaultAjaxUrl"; } }

        // Implement ICanCreateJsonFields
        public void AddJsonFields(IList<string> jsonFields, Dictionary<string, string> appenderNames, Func<string, string> virtualToAbsoluteFunc)
        {
            try
            {
                JavaScriptHelpers.AddJsonField(jsonFields, FieldEnabled, enabled);
                JavaScriptHelpers.AddJsonField(jsonFields, FieldMaxMessages, maxMessages);
                JavaScriptHelpers.AddJsonField(jsonFields, FieldDefaultAjaxUrl, defaultAjaxUrl, new UrlValue(virtualToAbsoluteFunc));
            }
            catch (Exception e)
            {
                string displayName = "jsnlog library";
                throw new ConfigurationException(displayName, e);
            }
        }
    }
}
