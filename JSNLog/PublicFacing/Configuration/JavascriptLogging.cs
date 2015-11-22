using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using JSNLog.Exceptions;
using JSNLog.Infrastructure;
using System.Text.RegularExpressions;

namespace JSNLog
{
    public class JavascriptLogging
    {
        /// <summary>
        /// Call this method for every request to generate a script tag with JavaScript
        /// that configures all loggers and appenders, based on the jsnlog element in the web.config.
        /// </summary>
        /// <param name="requestId">
        /// Request Id to be included in all logging requests sent by jsnlog.js from the client.
        /// If null, a new request id will be generated (the same one that will be returned from RequestId method).
        /// </param>
        /// <returns></returns>
        public static string Configure(string requestId = null)
        {
            StringBuilder sb = new StringBuilder();

            var jsnlogConfiguration = JavascriptLogging.JsnlogConfiguration;

            var configProcessor = new ConfigProcessor();
            configProcessor.ProcessRoot(jsnlogConfiguration, requestId, sb);

            return sb.ToString();
        }

        /// <summary>
        /// Returns a request id that is unique to this request.
        /// 
        /// However, if the request is a log request from jsnlog.js, than this method returns the requestId travelling 
        /// in the request.
        /// 
        /// The site can call this method to get the request id for use in server side logging.
        /// </summary>
        /// <returns></returns>
        public static string RequestId()
        {
            string requestId = JSNLog.Infrastructure.RequestId.GetFromRequest();

            // If requestId is empty string, regard that as a valid requestId.
            // jsnlog.js will send such request ids when the request id has not been
            // set. In that case, you don't want to generate a new request id for
            // a log request, because that would be confusing.
            if (requestId == null)
            {
                requestId = JSNLog.Infrastructure.RequestId.Get();
            }

            return requestId;
        }

        // Definitions for the OnLogging event. Search for OnLogging to see how it is used.
        public static event LoggingHandler OnLogging;

        internal static void RaiseLoggingEvent(LoggingEventArgs loggingEventArgs)
        {
            if (OnLogging != null)
            {
                OnLogging(loggingEventArgs);
            }
        }

        private static JsnlogConfiguration _jsnlogConfiguration = null;

        public static JsnlogConfiguration JsnlogConfiguration
        {
            get
            {
                if (_jsnlogConfiguration == null)
                {
                    XmlElement xe = XmlHelpers.RootElement();
                    if (xe != null)
                    {
                        var _jsnlogConfiguration = XmlHelpers.DeserialiseXml<JsnlogConfiguration>(xe);
                    }
                }

                // If there is no configuration, return the default configuration
                return _jsnlogConfiguration ?? new JsnlogConfiguration();
            }

            set
            {
                XmlElement xe = XmlHelpers.RootElement();
                if (xe != null)
                {
                    throw new ConflictingConfigException();
                }

                _jsnlogConfiguration = value;
            }
        }
    }
}
