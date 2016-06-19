#if !NET40

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using JSNLog.Infrastructure.AspNet5;
using Microsoft.Extensions.Primitives;
using JSNLog.Infrastructure;
using JSNLog.LogHandling;

// Be sure to leave the namespace at JSNLog.
namespace JSNLog
{
    /// <summary>
    /// Note that the OWIN counterpart of this (also in namespace JSNLog) 
    /// already has name JSNLogMiddlewareComponent
    /// </summary>
    public class JSNLogMiddleware
    {
        private readonly RequestDelegate next;

        public JSNLogMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            // If this is a logging request (based on its url), do the logging and don't pass on the request
            // to the rest of the pipeline.
            string url = context.Request.GetDisplayUrl();
            if (LoggingUrlHelpers.IsLoggingUrl(url))
            {
                ProcessRequest(context);
                return;
            }

            // It was not a logging request
            await next(context);
        }

        private void ProcessRequest(HttpContext context)
        {
            var headers = ToDictionary(context.Request.Headers);
            string urlReferrer = headers.SafeGet("Referer");
            string url = context.Request.GetDisplayUrl();

            var logRequestBase = new LogRequestBase(
                userAgent: headers.SafeGet("User-Agent"),
                userHostAddress: context.GetUserIp(),
                requestId: context.GetLogRequestId(),
                url: (urlReferrer ?? url).ToString(),
                queryParameters: ToDictionary(context.Request.Query),
                cookies: ToDictionary(context.Request.Cookies),
                headers: headers);

            DateTime serverSideTimeUtc = DateTime.UtcNow;
            string httpMethod = context.Request.Method;
            string origin = headers.SafeGet("Origin");

            Encoding encoding = HttpHelpers.GetEncoding(headers.SafeGet("Content-Type"));

            string json;
            using (var reader = new StreamReader(context.Request.Body, encoding))
            {
                json = reader.ReadToEnd();
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

        private void ToAspNet5Response(LogResponse logResponse, HttpResponse owinResponse)
        {
            owinResponse.StatusCode = logResponse.StatusCode;

            foreach (KeyValuePair<string, string> kvp in logResponse.Headers)
            {
                owinResponse.Headers[kvp.Key] = kvp.Value;
            }
        }

        private Dictionary<string, string> ToDictionary(IEnumerable<KeyValuePair<string, string>> values)
        {
            var result = new Dictionary<string, string>();
            foreach (var kvp in values)
            {
                result[kvp.Key] = kvp.Value.ToString();
            }

            return result;
        }

        private Dictionary<string, string> ToDictionary(IEnumerable<KeyValuePair<string, StringValues>> values)
        {
            var result = new Dictionary<string, string>();
            foreach (var kvp in values)
            {
                result[kvp.Key] = kvp.Value.ToString();
            }

            return result;
        }



        
    }
}

#endif
