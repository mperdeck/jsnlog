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
    }
}
