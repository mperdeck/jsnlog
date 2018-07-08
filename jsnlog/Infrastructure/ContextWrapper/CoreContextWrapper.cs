using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JSNLog.Infrastructure;
using Microsoft.AspNetCore.Http;

namespace JSNLog
{
    public class CoreContextWrapper : ContextWrapperCommon
    {
        HttpContext _httpContext;

        public CoreContextWrapper(HttpContext httpContext)
        {
            _httpContext = httpContext;
        }

        public override string GetRequestUserIp()
        {
            return Utils.SafeToString(_httpContext.Connection.RemoteIpAddress);
        }

        public override string GetRequestHeader(string requestHeaderName)
        {
            var headers = _httpContext.Request.Headers;
            return headers[requestHeaderName];
        }

        public override string GetRequestIdFromContext()
        {
            return _httpContext.TraceIdentifier;
        }

        public override void SetRequestIdInContext(string requestId)
        {
            _httpContext.TraceIdentifier = requestId;
        }


        /// <summary>
        /// Creates an ID that is unique hopefully.
        /// 
        /// This method initially tries to use the request id that IIS already uses internally. This allows us to correlate across even more log files.
        /// If this fails, for example if this is not part of a web request, than it uses a random GUID.
        /// 
        /// See
        /// http://blog.tatham.oddie.com.au/2012/02/07/code-request-correlation-in-asp-net/
        /// </summary>
        /// <returns></returns>
        public override string IISRequestId()
        {
            // Core versions always return null
            return null;
        }
    }
}
