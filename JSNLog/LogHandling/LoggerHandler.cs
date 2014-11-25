using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Collections.Specialized;
using System.Xml;
using JSNLog.LogHandling;
using System.IO;

// Be sure to leave the namespace at JSNLog. Web.config relies on this.
namespace JSNLog
{
    public class LoggerHandler : IHttpHandler
    {
        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            string userAgent = context.Request.UserAgent;
            string userHostAddress = context.Request.UserHostAddress;
            DateTime serverSideTimeUtc = DateTime.UtcNow;
            string url = context.Request.Url.AbsolutePath;
            string requestId = JSNLog.Infrastructure.RequestId.GetFromRequest();

            string json;
            using (var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding))
            {
                json = reader.ReadToEnd();
            }

            LoggerProcessor.ProcessLogRequest(json, userAgent, userHostAddress,
                serverSideTimeUtc, url, requestId);

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

            HttpResponse Response = context.Response;
            Response.ContentType = "text/plain";
            Response.Write("");
        }
    }
}
