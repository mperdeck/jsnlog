using System;
using System.Web;

namespace JSNLog.Infrastructure
{
    // This class only works in ASP.NET 4.x
    // It cannot be used in ASP.NET 5+

    public static class RequestId
    {
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
        private static string CreateNewRequestId()
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

            // Couldn't get the GUID as used in in all ETW outputs by IIS. So, use a random GUID.
            return Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Gets an id, that is unique to this request. 
        /// That is, for the same request, this method always returns the same string.
        /// </summary>
        /// <returns></returns>
        public static string Get()
        {
            string requestId = (string)(HttpContext.Current.Items[Constants.ContextItemRequestIdName]);

            if (requestId == null)
            {
                requestId = CreateNewRequestId();
                HttpContext.Current.Items[Constants.ContextItemRequestIdName] = requestId;
            }

            return requestId;
        }

        /// <summary>
        /// Gets the request id from an HTTP header in the request.
        /// Every log request sent by jsnlog.js should have such a header.
        /// However, requests not sent by jsnlog.js will not have this header obviously.
        /// 
        /// If the request id cannot be found, returns null.
        /// </summary>
        /// <returns></returns>
        public static string GetFromRequest()
        {
            var headers = HttpContext.Current.Request.Headers;
            string requestId = headers[Constants.HttpHeaderRequestIdName];
            return requestId;
        }
    }
}
