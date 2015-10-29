using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using JSNLog.Exceptions;
using JSNLog.Infrastructure;

namespace JSNLog.ValueInfos
{
    internal class AppendersValue : IValueInfo
    {
        private Dictionary<string, string> _appenderNames = null;

        public AppendersValue(Dictionary<string, string> appenderNames)
        {
            _appenderNames = appenderNames;
        }

        private string _validValueRegex = null;
        public string ValidValueRegex
        {
            get 
            {
                if (_validValueRegex == null)
                {
                    // If no appenders are defined, return a regex that only matches the empty string
                    if (!_appenderNames.Any())
                    {
                        _validValueRegex = "^$";
                    }
                    else
                    {
                        string[] appenderNames = _appenderNames.Keys.Select(a => Regex.Escape(a)).ToArray();
                        string regexAppenderNames = "(" + string.Join("|", appenderNames) + ")";
                        _validValueRegex = string.Format("^({0}(;{0})*)?$", regexAppenderNames);
                    }
                }
                return _validValueRegex; 
            }
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
