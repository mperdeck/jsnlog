using JSNLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSNLog
{
    public abstract class ContextWrapperCommon
    {
        public abstract string GetRequestUserIp();
        public abstract string GetRequestHeader(string requestHeaderName);
        public abstract string GetRequestIdFromContext();
        public abstract void SetRequestIdInContext(string requestId);
        public abstract string IISRequestId();

        public string GetUserIp()
        {
            string userIp = GetRequestUserIp();

            string xForwardedFor = GetRequestHeader(Constants.HttpHeaderXForwardedFor);
            if (!string.IsNullOrEmpty(xForwardedFor))
            {
                userIp = xForwardedFor + ", " + userIp;
            }

            return userIp;
        }

        /// <summary>
        /// Gets the request id from an HTTP header in the request.
        /// Every log request sent by jsnlog.js should have such a header.
        /// However, requests not sent by jsnlog.js will not have this header obviously.
        /// 
        /// If the request id cannot be found, returns null.
        /// </summary>
        /// <returns></returns>
        public string GetLogRequestId()
        {
            return GetRequestHeader(Constants.HttpHeaderRequestIdName);
        }

        /// <summary>
        /// Gets an id, that is unique to this request. 
        /// That is, for the same request, this method always returns the same string.
        /// </summary>
        /// <returns></returns>
        public string GetRequestId()
        {
            string requestId = GetRequestIdFromContext();

            if (requestId == null)
            {
                requestId = IISRequestId();

                if (requestId == null)
                {
                    requestId = CreateNewRequestId();
                }

                SetRequestIdInContext(requestId);
            }

            return requestId;
        }

        public string RequestId()
        {
            string requestId = GetLogRequestId();

            // If requestId is empty string, regard that as a valid requestId.
            // jsnlog.js will send such request ids when the request id has not been
            // set. In that case, you don't want to generate a new request id for
            // a log request, because that would be confusing.
            if (requestId == null)
            {
                requestId = GetRequestId();
            }

            return requestId;
        }

        private string CreateNewRequestId()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
