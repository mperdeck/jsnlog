using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using JSNLog;
using Microsoft.AspNetCore.Http;
using JSNLog.Infrastructure.AspNet5;
using Microsoft.Extensions.Primitives;
using JSNLog.Infrastructure;
using JSNLog.LogHandling;
using Microsoft.Extensions.Logging;

namespace jsnlog.Infrastructure
{
    internal class LoggerRequestHelpers
    {
        public static async Task ProcessLoggerRequestAsync(HttpContext context, ILogger logger)
        {
            // If there is an exception whilst processing the log request (for example when the connection with the
            // Internet disappears), try to log that exception. If that goes wrong too, fail silently.

            try
            {
                await ProcessRequestAsync(context);
            }
            catch (Exception e)
            {
                try
                {
                    logger.LogInformation($"JSNLog: Exception while processing log request -  {e}");
                }
                catch
                {
                }
            }

            return;
        }

        private static async Task ProcessRequestAsync(HttpContext context)
        {
            var headers = context.Request.Headers.ToDictionary();
            string urlReferrer = headers.SafeGet("Referer");
            string url = context.Request.GetDisplayUrl();

            var logRequestBase = new LogRequestBase(
                userAgent: headers.SafeGet("User-Agent"),
                userHostAddress: context.Wrapper().GetUserIp(),
                requestId: context.Wrapper().GetLogRequestId(),
                url: (urlReferrer ?? url).ToString(),
                queryParameters: context.Request.Query.ToDictionary(),
                cookies: context.Request.Cookies.ToDictionary(),
                headers: headers);

            DateTime serverSideTimeUtc = DateTime.UtcNow;
            string httpMethod = context.Request.Method;
            string origin = headers.SafeGet("Origin");

            Encoding encoding = HttpHelpers.GetEncoding(headers.SafeGet("Content-Type"));

            string json;
            using (var reader = new StreamReader(context.Request.Body, encoding))
            {
                json = await reader.ReadToEndAsync();
            }

            var response = new LogResponse();

            LoggerProcessor.ProcessLogRequest(json, logRequestBase,
                serverSideTimeUtc,
                httpMethod, origin, response);

            // Send dummy response. That way, the log request will not remain "pending"
            // in eg. Chrome dev tools.
            //
            // This must be given a MIME type of "text/plain"
            // Otherwise, the browser may try to interpret the empty string as XML.
            // When the user uses Firefox, and right clicks on the page and chooses "Inspect Element",
            // then in that debugger's console it will say "no element found".
            // See
            // http://www.acnenomor.com/307387p1/how-do-i-setup-my-ajax-post-request-to-prevent-no-element-found-on-empty-response
            // http://stackoverflow.com/questions/975929/firefox-error-no-element-found/976200#976200

            ToAspNet5Response(response, context.Response);
            context.Response.ContentType = "text/plain";
            context.Response.ContentLength = 0;
        }

        private static void ToAspNet5Response(LogResponse logResponse, HttpResponse owinResponse)
        {
            owinResponse.StatusCode = logResponse.StatusCode;

            foreach (KeyValuePair<string, string> kvp in logResponse.Headers)
            {
                owinResponse.Headers[kvp.Key] = kvp.Value;
            }
        }
    }
}
