using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace JSNLog.ValueInfos
{
    internal class UrlValue : IValueInfo
    {
        public string ValidValueRegex
        {
            get { return Constants.RegexUrl; }
        }

        public string ToJavaScript(string text)
        {
            return HttpUtility.JavaScriptStringEncode(text, true);
        }
    }
}
