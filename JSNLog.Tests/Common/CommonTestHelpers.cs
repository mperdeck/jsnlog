using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace JSNLog.Tests.Common
{
    public class CommonTestHelpers
    {
        public static void SetConfigCache(string configXml, ILoggingAdapter logger = null)
        {
            // Set config cache in JavascriptLogging to contents of xe
            XmlElement xe = ConfigToXe(configXml);
            JavascriptLogging.SetJsnlogConfiguration(null, logger);
            JavascriptLogging.GetJsnlogConfiguration(() => xe);
        }

        public static XmlElement ConfigToXe(string configXml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(configXml);
            XmlElement xe = (XmlElement)doc.DocumentElement;

            return xe;
        }
    }
}
