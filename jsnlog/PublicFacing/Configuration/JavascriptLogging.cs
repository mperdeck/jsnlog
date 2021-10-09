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

        private static ILoggingAdapter _logger = null;

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
                    productionLibraryPath = SiteConstants.CdnJsDownloadUrl
                };
            }

            _jsnlogConfiguration.Validate();

            return _jsnlogConfiguration;
        }

        // All unit tests run under DOTNETCLI
        public static JsnlogConfiguration GetJsnlogConfiguration()
        {
            return GetJsnlogConfigurationWithoutWebConfig();
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
            SetJsnlogConfigurationWithoutWebConfig(jsnlogConfiguration, loggingAdapter);
        }

#endregion
    }
}
