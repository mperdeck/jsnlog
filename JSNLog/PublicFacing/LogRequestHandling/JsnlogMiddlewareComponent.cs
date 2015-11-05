using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Xml;
using JSNLog.LogHandling;
using System.IO;
using JSNLog.Infrastructure;
using Microsoft.Owin;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

// Be sure to leave the namespace at JSNLog.
namespace JSNLog
{
    // use an alias for the OWIN AppFunc:
    using AppFunc = Func<IDictionary<string, object>, System.Threading.Tasks.Task>;

    public class JsnlogMiddlewareComponent
    {
        AppFunc _next;
        Regex _loggerUrlRegex;

        public JsnlogMiddlewareComponent(AppFunc next, string loggerUrlRegex)
        {
            _next = next;
            _loggerUrlRegex = new Regex(loggerUrlRegex);
        }
        public async Task Invoke(IDictionary<string, object> environment)
        {
            IOwinContext context = new OwinContext(environment);

            // If this is a logging request (based on its url), do the logging and don't pass on the request
            // to the rest of the pipeline.
            if (_loggerUrlRegex.IsMatch(context.Request.Uri.OriginalString))
            {
                ProcessRequest(context);
                return;
            }

            // It was not a logging request
            await _next.Invoke(environment);
        }

        private void ProcessRequest(IOwinContext context)
        {
            var headers = ToDictionary(context.Request.Headers);
            string urlReferrer = headers.SafeGet("Referer");
            string url = context.Request.Uri.OriginalString;

            var logRequestBase = new LogRequestBase(
                userAgent: headers.SafeGet("User-Agent"),
                userHostAddress: context.Request.RemoteIpAddress,
                requestId: JSNLog.Infrastructure.RequestId.GetFromRequest(),
                url: (urlReferrer ?? url).ToString(),
                queryParameters: ToDictionary(context.Request.Query),
                cookies: Utils.ToDictionary(context.Request.Cookies),
                headers: headers);

            DateTime serverSideTimeUtc = DateTime.UtcNow;
            string httpMethod = context.Request.Method;
            string origin = headers.SafeGet("Origin");

            string json;
            using (var reader = new StreamReader(context.Request.Body, context.Request.ContentEncoding))
            {
                json = reader.ReadToEnd();
            }

            var response = new LogResponse();
            ILogger logger = new Logger();
            XmlElement xe = XmlHelpers.RootElement();

            LoggerProcessor.ProcessLogRequest(json, logRequestBase,
                serverSideTimeUtc,
                httpMethod, origin, response, logger, xe);

        }

        private Dictionary<string, string> ToDictionary(IReadableStringCollection nameValueCollection)
        {
            var result = new Dictionary<string, string>();

            foreach (var kvp in nameValueCollection)
            {
                result[kvp.Key] = nameValueCollection.Get(kvp.Key);
            }

            return result;
        }



        //public void ProcessRequest(HttpContext context)
        //{
        //    var logRequestBase = new LogRequestBase(
        //        userAgent: context.Request.UserAgent,
        //        userHostAddress: context.Request.UserHostAddress,
        //        requestId: JSNLog.Infrastructure.RequestId.GetFromRequest(),
        //        url: (context.Request.UrlReferrer ?? context.Request.Url).ToString(),
        //        queryParameters: Utils.ToDictionary(context.Request.QueryString),
        //        cookies: ToDictionary(context.Request.Cookies),
        //        headers: Utils.ToDictionary(context.Request.Headers));

        //    DateTime serverSideTimeUtc = DateTime.UtcNow;
        //    string httpMethod = context.Request.HttpMethod;
        //    string origin = context.Request.Headers["Origin"];

        //    string json;
        //    using (var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding))
        //    {
        //        json = reader.ReadToEnd();
        //    }

        //    HttpResponse response = context.Response;
        //    ILogger logger = new Logger();
        //    XmlElement xe = XmlHelpers.RootElement();

        //    LoggerProcessor.ProcessLogRequest(json, logRequestBase,
        //        serverSideTimeUtc,
        //        httpMethod, origin, new HttpResponseWrapper(response), logger, xe);

        //    // Send dummy response. That way, the log request will not remain "pending"
        //    // in eg. Chrome dev tools.
        //    //
        //    // This must be given a MIME type of "text/plain"
        //    // Otherwise, the browser may try to interpret the empty string as XML.
        //    // When the user uses Firefox, and right clicks on the page and chooses "Inspect Element",
        //    // then in that debugger's console it will say "no element found".
        //    // See
        //    // http://www.acnenomor.com/307387p1/how-do-i-setup-my-ajax-post-request-to-prevent-no-element-found-on-empty-response
        //    // http://stackoverflow.com/questions/975929/firefox-error-no-element-found/976200#976200

        //    response.ContentType = "text/plain";
        //    response.ClearContent();
        //    response.Write("");
        //}

        //private Dictionary<string, string> ToDictionary(HttpCookieCollection httpCookieCollection)
        //{
        //    // HttpCookieCollection requires System.Web, so has been kept in this file.

        //    var result = new Dictionary<string, string>();

        //    foreach (string key in httpCookieCollection.AllKeys)
        //    {
        //        result[key] = httpCookieCollection[key].Value;
        //    }

        //    return result;
        //}
    }
}
