using System;
using System.Collections.Generic;
#if NET45
using System.Web;
#else
using Microsoft.AspNet.Http;
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
#if NET45
        private static string IISRequestId(HttpContextBase httpContext)
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
#else
        private static string IISRequestId(HttpContext httpContext)
        {
            // DNX versions always return null
            return null;
        }
#endif

#if NET45
        private static string GetRequestIdFromContext(this HttpContextBase httpContext)
        {
            return (string)(HttpContext.Current.Items[Constants.ContextItemRequestIdName]);
        }
#else
        private static string GetRequestIdFromContext(this HttpContext httpContext)
        {
            return httpContext.TraceIdentifier;
        }
#endif

#if NET45
        private static void SetRequestIdInContext(HttpContextBase httpContext, string requestId)
        {
            HttpContext.Current.Items[Constants.ContextItemRequestIdName] = requestId;
        }
#else
        private static void SetRequestIdInContext(HttpContext httpContext, string requestId)
        {
            httpContext.TraceIdentifier = requestId;
        }
#endif

        /// <summary>
        /// Gets the request id from an HTTP header in the request.
        /// Every log request sent by jsnlog.js should have such a header.
        /// However, requests not sent by jsnlog.js will not have this header obviously.
        /// 
        /// If the request id cannot be found, returns null.
        /// </summary>
        /// <returns></returns>
        public static string GetLogRequestId(
#if NET45
                this HttpContextBase httpContext
#else
                this HttpContext httpContext
#endif
            )
        {
            // Even though the code for NET45 and DNX is the same for getting the headers,
            // the type of the headers variable will be different.
            var headers = httpContext.Request.Headers;

            string requestId = headers[Constants.HttpHeaderRequestIdName];
            return requestId;
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
        public static string GetRequestId(
#if NET45
                this HttpContextBase httpContext
#else
                this HttpContext httpContext
#endif
            )
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
