using System;
using System.Text;
#if NET45 || DNX451
using System.Xml;
#endif
using JSNLog.Exceptions;
using JSNLog.Infrastructure;
using JSNLog.LogHandling;
#if NET45
using System.Web;
#else
using Microsoft.AspNet.Http;
#endif

namespace JSNLog
{
    public static class JavascriptLogging
    {
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
#if NET45
        public static string Configure(string requestId = null)
        {
            return HttpContext.Current.Configure(requestId);
        }
#endif

#if NET45
        public static string Configure(this HttpContext httpContext, string requestId = null)
        {
            return httpContext.ToContextBase().Configure(requestId);
        }

        public static string Configure(this HttpContextBase httpContext, string requestId = null)
#else
        public static string Configure(this HttpContext httpContext, string requestId = null)
#endif
        {
            StringBuilder sb = new StringBuilder();

            string userIp = httpContext.GetUserIp();
            var configProcessor = new ConfigProcessor();

            // If someone passes in a null requestId via a ViewBag, then it may be converted to empty string 
            configProcessor.ProcessRoot(
                string.IsNullOrEmpty(requestId) ? httpContext.GetRequestId() : requestId, 
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
#if NET45
        public static string RequestId()
        {
            return HttpContext.Current.ToContextBase().RequestId();
        }
#endif

#if NET45
        public static string RequestId(this HttpContextBase httpContext)
#else
        public static string RequestId(this HttpContext httpContext)
#endif
        {
            string requestId = httpContext.GetLogRequestId();

            // If requestId is empty string, regard that as a valid requestId.
            // jsnlog.js will send such request ids when the request id has not been
            // set. In that case, you don't want to generate a new request id for
            // a log request, because that would be confusing.
            if (requestId == null)
            {
                requestId = httpContext.GetRequestId();
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

#region JsnlogConfiguration

        private static JsnlogConfiguration _jsnlogConfiguration = null;

#if NET45
        private static ILoggingAdapter _logger = new CommonLoggingAdapter();
#else
        private static ILoggingAdapter _logger = null;
#endif


        internal static JsnlogConfiguration GetJsnlogConfigurationWithoutWebConfig()
        {
            // If there is no configuration, return the default configuration
            return _jsnlogConfiguration ?? new JsnlogConfiguration();
        }

        // All unit tests run under DNX451
#if NET45 || DNX451

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
#if NET45
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

        // All unit tests run under DNX451
#if NET45 || DNX451
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
#if NET45
            SetJsnlogConfiguration(() => XmlHelpers.RootElement(), jsnlogConfiguration, loggingAdapter);
#else
            SetJsnlogConfigurationWithoutWebConfig(jsnlogConfiguration, loggingAdapter);
#endif
        }

#endregion
    }
}
