using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if !DNXCORE50
using System.Text.RegularExpressions;
#endif

#if NET45
using System.Web;
#else
using Microsoft.AspNet.Http;
#endif

namespace JSNLog.Infrastructure
{
    internal static class HttpHelpers
    {
#if !DNXCORE50
        private static Regex _regex = new Regex(@";\s*charset=(?<charset>[^\s;]+)");
#endif

#if NET45
        public static HttpContextBase ToContextBase(this HttpContext httpContext)
        {
            var contextBase = new HttpContextWrapper(httpContext);
            return contextBase;
        }
#endif

        public static Encoding GetEncoding(string contentType)
        {
            if (string.IsNullOrEmpty(contentType))
            {
                return Encoding.UTF8;
            }

            // Seeing that log entries are sent as JSON, the browser should always sending them
            // in Unicode, probably UTF-8.
            string charset = "utf-8";

#if !DNXCORE50
            // Processing any charset anyway, just to be sure.
            // But not for DNXCORE50, because that doesn't support regular expressions in RC2.
            var match = _regex.Match(contentType);
            if (match.Success)
            {
                charset = match.Groups["charset"].Value;
            }
#endif

            try
            {
                return Encoding.GetEncoding(charset);
            }
            catch
            {
                return Encoding.UTF8;
            }
        }

#if NET45
        public static string GetUserIp(this HttpContextBase httpContext)
        {
            string userIp = httpContext.Request.UserHostAddress;
            return userIp;
        }
#else
        public static string GetUserIp(this HttpContext httpContext)
        {
            string userIp = Utils.SafeToString(httpContext.Connection.RemoteIpAddress);
            return userIp;
        }
#endif
    }
}
