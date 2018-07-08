using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace JSNLog
{
    public class NetFrameworkContextWrapper: ContextWrapperCommon
    {
        HttpContext _httpContext;

        public NetFrameworkContextWrapper(HttpContext httpContext)
        {
            _httpContext = httpContext;
        }

        public override string GetRequestUserIp()
        {
            return _httpContext.Request.UserHostAddress;
        }

        public override string GetRequestHeader(string requestHeaderName)
        {
            var headers = _httpContext.Request.Headers;
            return headers[requestHeaderName];
        }
    }
}
