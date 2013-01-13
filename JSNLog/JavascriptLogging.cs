using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Web.Configuration;
using JSNLog.Exceptions;
using JSNLog.Infrastructure;
using System.Text.RegularExpressions;
using System.Web;

namespace JSNLog
{
    public class JavascriptLogging
    {

        public static string Configure()
        {
            XmlElement xe = XmlHelpers.RootElement();

            StringBuilder sb = new StringBuilder();

            var configProcessor = new ConfigProcessor();
            configProcessor.ProcessRoot(xe, sb);

            return sb.ToString();
        }
    }
}
