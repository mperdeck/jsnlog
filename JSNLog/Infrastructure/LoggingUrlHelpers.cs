using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JSNLog.Infrastructure
{
    public class LoggingUrlHelpers
    {

        /// <summary>
        /// Returns true if a request with the given url is a logging request that should be handled
        /// by JSNLog.
        /// 
        /// Note that the user may have set the defaultUrl or an appender url because they want to use
        /// the standard url /jsnlog.logger for something else. So you don't want to always allow 
        /// /jsnlog.logger.
        /// 
        /// On the other hand, it is impossible to figure out exactly what urls the user has used, because you
        /// don't know which loggers are being used in the user's code.
        /// So you can't only match against the urls specified with the appenders.
        /// 
        /// So this method assumes that in addition to the appenders that have been configured,
        /// the default appender is also used. That is, it also passes the defaultUrl set by the user
        /// (and if that is not set, the defaultDefaultUrl).
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static bool IsLoggingUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException(url);
            }

            JsnlogConfiguration jsnlogConfiguration = JavascriptLogging.GetJsnlogConfiguration();

            if (jsnlogConfiguration == null)
            {
                return UrlMatchesAppenderUrl(Constants.DefaultDefaultAjaxUrl, url);
            }

            string resolvedDefaultUrl = ResolvedAppenderUrl(Constants.DefaultDefaultAjaxUrl, jsnlogConfiguration.defaultAjaxUrl, null);
            if (UrlMatchesAppenderUrl(resolvedDefaultUrl, url))
            {
                return true;
            }

            if (jsnlogConfiguration.ajaxAppenders != null)
            {
                foreach (AjaxAppender ajaxAppender in jsnlogConfiguration.ajaxAppenders)
                {
                    string resolvedAppenderUrl = ResolvedAppenderUrl(
                        Constants.DefaultDefaultAjaxUrl, jsnlogConfiguration.defaultAjaxUrl, ajaxAppender.url);
                    if (UrlMatchesAppenderUrl(resolvedAppenderUrl, url))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Returns the end of the url that an AjaxAppender will send its log requests to.
        /// Note that this is not the complete url, it may not have the domain, etc.
        /// </summary>
        /// <param name="defaultDefaultUrl">
        /// The default url when user does not provide a url at all.
        /// </param>
        /// <param name="defaultUrl">
        /// DefaultUrl given to the library
        /// </param>
        /// <param name="appenderUrl">
        /// Url given to the appender.
        /// </param>
        /// <returns></returns>
        public static string ResolvedAppenderUrl(string defaultDefaultUrl, string defaultUrl, string appenderUrl)
        {
            if (!string.IsNullOrEmpty(appenderUrl)) { return appenderUrl; }
            if (!string.IsNullOrEmpty(defaultUrl)) { return defaultUrl; }

            return defaultDefaultUrl;
        }

        private static string TrimmedUrl(string url)
        {
            return url.TrimStart('~');
        }

        private static bool UrlMatchesAppenderUrl(string appenderUrl, string url)
        {
            // The appender url may be prefixed with ~ (for virtual directory).

            string[] urlParts = url.Split(new char[] { '?' });
            string urlWithoutQuery = urlParts[0];

            bool result = urlWithoutQuery.EndsWith(TrimmedUrl(appenderUrl));
            return result;
        }

    }
}
