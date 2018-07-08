using System;
using System.Collections.Generic;

namespace JSNLog.Infrastructure
{
    static class RequestId
    {

        public static string GetLogRequestId(this Dictionary<string,string> headers)
        {
            string requestId = headers.SafeGet(Constants.HttpHeaderRequestIdName);
            return requestId;
        }
    }
}
