using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

#if NET40
using System.Web;
#else
using Microsoft.AspNetCore.Http;
#endif

namespace JSNLog.Infrastructure
{
    internal static class HttpHelpers
    {
        private static Regex _regex = new Regex(@";\s*charset=(?<charset>[^\s;]+)");
        
        public static Encoding GetEncoding(string contentType)
        {
            if (string.IsNullOrEmpty(contentType))
            {
                return Encoding.UTF8;
            }

            string charset = "utf-8";
            var match = _regex.Match(contentType);
            if (match.Success)
            {
                charset = match.Groups["charset"].Value;
            }

            try
            {
                return Encoding.GetEncoding(charset);
            }
            catch
            {
                return Encoding.UTF8;
            }
        }

        public static string GetUserIp(this HttpContext httpContext)
        {
#if NET40
            string userIp = httpContext.Request.UserHostAddress;
#else
            string userIp = Utils.SafeToString(httpContext.Connection.RemoteIpAddress);
#endif

            string xForwardedFor = httpContext.GetRequestHeader(Constants.HttpHeaderXForwardedFor);
            if (!string.IsNullOrEmpty(xForwardedFor))
            {
                userIp = xForwardedFor + ", " + userIp;
            }

            return userIp;
        }

        public static string GetRequestHeader(this HttpContext httpContext, string requestHeaderName)
        {
            // Even though the code for NET40 and DNX is the same for getting the headers,
            // the type of the headers variable will be different.
            var headers = httpContext.Request.Headers;

            string requestHeaderValue = headers[requestHeaderName];
            return requestHeaderValue;
        }
    }
}
