using System;
using System.Collections.Generic;
#if NET40
using System.Web;
#else
using Microsoft.AspNetCore.Http;
#endif

namespace JSNLog.Infrastructure
{
    static class RequestId
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
        private static string IISRequestId(HttpContext httpContext)
        {
#if NET40
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
#endif

            // DNX versions always return null

            return null;
        }

        private static string GetRequestIdFromContext(HttpContext httpContext)
        {
#if NET40
            return (string)(HttpContext.Current.Items[Constants.ContextItemRequestIdName]);
#else
            return httpContext.TraceIdentifier;
#endif
        }

        private static void SetRequestIdInContext(HttpContext httpContext, string requestId)
        {
#if NET40
            HttpContext.Current.Items[Constants.ContextItemRequestIdName] = requestId;
#else
            httpContext.TraceIdentifier = requestId;
#endif
        }

        /// <summary>
        /// Gets the request id from an HTTP header in the request.
        /// Every log request sent by jsnlog.js should have such a header.
        /// However, requests not sent by jsnlog.js will not have this header obviously.
        /// 
        /// If the request id cannot be found, returns null.
        /// </summary>
        /// <returns></returns>
        public static string GetLogRequestId(this HttpContext httpContext)
        {
            return httpContext.GetRequestHeader(Constants.HttpHeaderRequestIdName);
        }

        public static string GetLogRequestId(this Dictionary<string,string> headers)
        {
            string requestId = headers.SafeGet(Constants.HttpHeaderRequestIdName);
            return requestId;
        }
        
        private static string CreateNewRequestId()
        {
            return Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Gets an id, that is unique to this request. 
        /// That is, for the same request, this method always returns the same string.
        /// </summary>
        /// <returns></returns>
        public static string GetRequestId(this HttpContext httpContext)
        {
            string requestId = GetRequestIdFromContext(httpContext);

            if (requestId == null)
            {
                requestId = IISRequestId(httpContext);

                if (requestId == null)
                {
                    requestId = CreateNewRequestId();
                }

                SetRequestIdInContext(httpContext, requestId);
            }

            return requestId;
        }
    }
}
