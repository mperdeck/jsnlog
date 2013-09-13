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
            XmlElement xe = XmlHelpers.RootElement();

            StringBuilder sb = new StringBuilder();

            var configProcessor = new ConfigProcessor();
            configProcessor.ProcessRoot(xe, requestId, sb);

            return sb.ToString();
        }

        /// <summary>
        /// Returns a request id that is unique to this request.
        /// The site can call this method to get the request id for use in server side logging.
        /// </summary>
        /// <returns></returns>
        public static string RequestId()
        {
            return JSNLog.Infrastructure.RequestId.Get();
        }
    }
}
