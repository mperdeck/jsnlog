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
#if NETFRAMEWORK
    [XmlRoot("jsnlog")]
#endif
    public class JsnlogConfiguration : ICanCreateJsonFields
    {
#if NETFRAMEWORK
        [XmlAttribute]
#endif
        public bool enabled { get; set; }

#if NETFRAMEWORK
        [XmlAttribute]
#endif
        public uint maxMessages { get; set; }

#if NETFRAMEWORK
        [XmlAttribute]
#endif
        public string defaultAjaxUrl { get; set; }

#if NETFRAMEWORK
        [XmlAttribute]
#endif
        public string corsAllowedOriginsRegex { get; set; }

#if NETFRAMEWORK
        [XmlAttribute]
#endif
        public string serverSideLogger { get; set; }

#if NETFRAMEWORK
        [XmlAttribute]
#endif
        public string serverSideLevel { get; set; }

#if NETFRAMEWORK
        [XmlAttribute]
#endif
        public string serverSideMessageFormat { get; set; }

#if NETFRAMEWORK
        [XmlAttribute]
#endif
        public string dateFormat { get; set; }

#if NETFRAMEWORK
        [XmlAttribute]
#endif
        public string productionLibraryPath { get; set; }

#if NETFRAMEWORK
        [XmlAttribute]
#endif

        private bool _insertJsnlogInHtmlResponses = false;

        public bool insertJsnlogInHtmlResponses 
        {
            get
            {
                return _insertJsnlogInHtmlResponses;
            }
            set
            {
#if NETFRAMEWORK
                if (value)
                {
                    throw new Exception(
                        "The JsnlogConfiguration.insertJsnlogInHtmlResponses property cannot be set to true in netstandard2.0. " +
                        $"Upgrade to netstandard2.1 or for other options see {SiteConstants.InstallPageUrl}");
                }
#endif

                _insertJsnlogInHtmlResponses = value;
            }
        }

        // Be sure to make everything Properties. While the XML serializer handles fields ok,
        // the JSON serializer used in ASP.NET 5 doesn't.

#if NETFRAMEWORK
        [XmlElement("logger")]
#endif
        public List<Logger> loggers { get; set; }

#if NETFRAMEWORK
        [XmlElement("ajaxAppender")]
#endif
        public List<AjaxAppender> ajaxAppenders { get; set; }

#if NETFRAMEWORK
        [XmlElement("consoleAppender")]
#endif
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

        public void Validate()
        {
            var appenders = new List<Appender>();

            if (ajaxAppenders != null)
            {
                appenders.AddRange(ajaxAppenders);
            }

            if (consoleAppenders != null)
            {
                appenders.AddRange(consoleAppenders);
            }

            if (appenders.Any(a=>string.IsNullOrWhiteSpace(a.name)))
            {
                throw new GeneralAppenderException(@"""""", "Appenders have to have a name, and it must not be empty.");
            }

            var appendersNames = appenders.Select(a => a.name);

            string duplicateName = appendersNames.GroupBy(a => a).Where(g => g.Skip(1).Any()).SelectMany(a => a).FirstOrDefault();
            if (duplicateName != null)
            {
                throw new GeneralAppenderException(duplicateName, "There are multiple appenders with this name. However, appender names must be unique.");
            }
        }
    }
}
