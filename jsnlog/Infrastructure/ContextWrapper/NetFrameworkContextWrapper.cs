#if NETFRAMEWORK

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

        public override string GetRequestIdFromContext()
        {
            return (string)(HttpContext.Current.Items[Constants.ContextItemRequestIdName]);
        }

        public override void SetRequestIdInContext(string requestId)
        {
            HttpContext.Current.Items[Constants.ContextItemRequestIdName] = requestId;
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
            var provider = (IServiceProvider)HttpContext.Current;
            if (provider != null)
            {
                var workerRequest = (HttpWorkerRequest)provider.GetService(typeof(HttpWorkerRequest));
                if (workerRequest != null)
                {
                    Guid requestIdGuid = workerRequest.RequestTraceIdentifier;
                    if (requestIdGuid != Guid.Empty)
                    {
                        return requestIdGuid.ToString();
                    }
                }
            }

            return null;
        }
    }
}

#endif

