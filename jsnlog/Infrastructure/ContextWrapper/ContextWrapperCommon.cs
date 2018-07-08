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



    }
}
