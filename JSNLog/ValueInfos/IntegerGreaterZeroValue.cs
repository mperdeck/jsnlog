using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace JSNLog.ValueInfos
{
    public class IntegerGreaterZeroValue : IValueInfo
    {
        public string ValidValueRegex
        {
            get { return Constants.RegexIntegerGreaterZero; }
        }

        public string ToJavaScript(string text)
        {
            return text;
        }
    }
}
