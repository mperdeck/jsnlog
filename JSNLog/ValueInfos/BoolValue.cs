using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace JSNLog.ValueInfos
{
    internal class BoolValue : IValueInfo
    {
        public string ValidValueRegex
        {
            get { return Constants.RegexBool; }
        }

        public string ToJavaScript(string text)
        {
            return text.ToLower();
        }
    }
}
