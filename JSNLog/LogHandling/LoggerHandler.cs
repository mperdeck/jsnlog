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

            string json;
            using (var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding))
            {
                json = reader.ReadToEnd();
            }

            LoggerProcessor.ProcessLogRequest(json, userAgent, userHostAddress,
                serverSideTimeUtc, url);

            // Send dummy response. That way, the log request will not remain "pending"
            // in eg. Chrome dev tools.
            HttpResponse Response = context.Response;
            Response.Write("");
        }
    }
}
