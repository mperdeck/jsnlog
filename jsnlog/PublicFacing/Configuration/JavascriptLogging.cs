using System;
using System.Text;
using System.Xml;
using JSNLog.Exceptions;
using JSNLog.Infrastructure;
using JSNLog.LogHandling;
#if NETFRAMEWORK
using System.Web;
#endif
using Microsoft.AspNetCore.Http;

namespace JSNLog
{
    public static class JavascriptLogging
    {
#if NETFRAMEWORK
        /// <summary>
        /// Call this method for every request to generate a script tag with JavaScript
        /// that configures all loggers and appenders, based on the jsnlog element in the web.config.
        /// </summary>
        /// <param name="requestId">
        /// Request Id to be included in all logging requests sent by jsnlog.js from the client.
        /// If null, a new request id will be generated (the same one that will be returned from RequestId method).
        /// </param>
        /// <returns>
        /// A script tag with the JavaScript to do all configuration.
        /// </returns>
        public static string Configure(string requestId = null)
        {
            return System.Web.HttpContext.Current.Configure(requestId);
        }

        public static string Configure(this System.Web.HttpContext httpContext, string requestId = null)
        {
            return Configure(
                httpContext.Wrapper().GetUserIp(),
                string.IsNullOrEmpty(requestId) ? httpContext.Wrapper().GetRequestId() : requestId);
        }
#endif

        public static string Configure(this Microsoft.AspNetCore.Http.HttpContext httpContext, string requestId = null)
        {
            return Configure(
                httpContext.Wrapper().GetUserIp(),
                string.IsNullOrEmpty(requestId) ? httpContext.Wrapper().GetRequestId() : requestId);
        }

        public static string Configure(string userIp, string requestId = null)
        {
            StringBuilder sb = new StringBuilder();

            var configProcessor = new ConfigProcessor();

            // If someone passes in a null requestId via a ViewBag, then it may be converted to empty string 
            configProcessor.ProcessRoot(
                requestId,
                sb, userIp);

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
#if NETFRAMEWORK
        public static string RequestId()
        {
            return System.Web.HttpContext.Current.Wrapper().RequestId();
        }

        public static string RequestId(this System.Web.HttpContext httpContext)
        {
            return httpContext.Wrapper().RequestId();
        }
#endif

        public static string RequestId(this Microsoft.AspNetCore.Http.HttpContext httpContext)
        {
            return httpContext.Wrapper().RequestId();
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

#region JsnlogConfiguration

        private static JsnlogConfiguration _jsnlogConfiguration = null;

#if NETFRAMEWORK
        // If targeting Net Framework whilst running in a Core web site, this will at first be 
        // wrongly set to the CommonLogging adapter. But it then gets overwritten by 
        // SetJsnlogConfiguration when that method is called by UseJsnlog.
        private static ILoggingAdapter _logger = new CommonLoggingAdapter();
#else
        private static ILoggingAdapter _logger = null;
#endif


        internal static JsnlogConfiguration GetJsnlogConfigurationWithoutWebConfig()
        {
            // If there is no configuration, return the default configuration.
            // In that case, assume the user is after the minimal configuration, where the JSNLog middleware
            // automatically inserts the jsnlog script tag and config js code in all html responses from the server.

            if (_jsnlogConfiguration == null)
            {
                return new JsnlogConfiguration() 
                { 
                    insertJsnlogHtmlInAllHtmlResponse = true,
                    productionLibraryPath = WebSite.App_Code.SiteConstants.CdnJsDownloadUrl
                };
            }

            _jsnlogConfiguration.Validate();

            return _jsnlogConfiguration;
        }

        // All unit tests run under DOTNETCLI
#if NETFRAMEWORK

        // Seam used for unit testing. During unit testing, gets an xml element created by the test. 
        // During production get the jsnlog element from web.config.
        //
        // >>>>>>
        // Note that calling this method with a given xe is a way to cache that xe's config
        // for the next call to GetJsnlogConfiguration().
        internal static JsnlogConfiguration GetJsnlogConfiguration(Func<XmlElement> lxe)
        {
            if (_jsnlogConfiguration == null)
            {
                XmlElement xe = lxe();
                if (xe != null)
                {
                    _jsnlogConfiguration = XmlHelpers.DeserialiseXml<JsnlogConfiguration>(xe);
                }
            }

            return GetJsnlogConfigurationWithoutWebConfig();
        }

#endif

        public static JsnlogConfiguration GetJsnlogConfiguration()
        {
#if NETFRAMEWORK
            // When using ASP Net CORE with a Net Framework target (such as net47), 
            // assume XmlHelpers.RootElement() will always be null.
            return GetJsnlogConfiguration(() => XmlHelpers.RootElement());
#else
            return GetJsnlogConfigurationWithoutWebConfig();
#endif
        }

        internal static ILoggingAdapter GetLogger()
        {
            return _logger;
        }

        internal static void SetJsnlogConfigurationWithoutWebConfig(
            JsnlogConfiguration jsnlogConfiguration, ILoggingAdapter loggingAdapter = null)
        {
            _jsnlogConfiguration = jsnlogConfiguration;

            // Never allow setting the logger to null.
            // If user only set the configuration (leaving logger at null), don't change the logger.

            if (loggingAdapter != null)
            {
                _logger = loggingAdapter;
            }
        }

        // All unit tests run under DOTNETCLI
#if NETFRAMEWORK
        internal static void SetJsnlogConfiguration(
            Func<XmlElement> lxe, JsnlogConfiguration jsnlogConfiguration, ILoggingAdapter logger = null)
        {
            // Always allow setting the config to null, because GetJsnlogConfiguration retrieves web.config when config is null.
            if (jsnlogConfiguration != null)
            {
                XmlElement xe = lxe();
                if (xe != null)
                {
                    throw new ConflictingConfigException();
                }
            }

            SetJsnlogConfigurationWithoutWebConfig(jsnlogConfiguration, logger);
        }
#endif

        public static void SetJsnlogConfiguration(
            JsnlogConfiguration jsnlogConfiguration, ILoggingAdapter loggingAdapter = null)
        {
#if NETFRAMEWORK
            // When using ASP Net CORE with a Net Framework target (such as net47), 
            // assume XmlHelpers.RootElement() will always be null.
            SetJsnlogConfiguration(() => XmlHelpers.RootElement(), jsnlogConfiguration, loggingAdapter);
#else
            SetJsnlogConfigurationWithoutWebConfig(jsnlogConfiguration, loggingAdapter);
#endif
        }

#endregion
    }
}
