using System.Collections.Generic;
using System.Linq;
using JSNLog.Exceptions;

namespace JSNLog.ValueInfos
{
    internal class AppendersValue : IValueInfo
    {
        private Dictionary<string, string> _appenderNames = null;

        public AppendersValue(Dictionary<string, string> appenderNames)
        {
            _appenderNames = appenderNames;
        }

        public string ToJavaScript(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return "[]";
            }

            string[] appenderNames = text.Split(new[] { Constants.AppenderNameSeparator });
            string[] appenderVariableNames = appenderNames.Select(a => {
                string trimmed = a.Trim();
                if (!_appenderNames.ContainsKey(trimmed)) {throw new UnknownAppenderException(trimmed);}
                string appenderVariable = _appenderNames[trimmed];
                return appenderVariable;
            }).ToArray();

            string js = "[" + string.Join(",", appenderVariableNames) + "]";
            return js;
        }
    }
}
