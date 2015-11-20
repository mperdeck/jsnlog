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

        [XmlElement("logger")]
        public List<Logger> loggers = new List<Logger>();

        [XmlElement("ajaxAppender")]
        public List<AjaxAppender> ajaxAppenders = new List<AjaxAppender>();

        [XmlElement("consoleAppender")]
        public List<ConsoleAppender> consoleAppenders = new List<ConsoleAppender>();

        public JsnlogConfiguration()
        {
            // Set default values. If an element is not given in the XML or JSON,
            // the deserializer will simply not set it.
            enabled = true;
            maxMessages = int.MaxValue;
            serverSideMessageFormat = "%message";
            dateFormat = "o";
            defaultAjaxUrl = "/jsnlog.logger";
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
