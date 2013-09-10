using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using JSNLog.Infrastructure;

namespace JSNLog.ValueInfos
{
    public class LevelValue : IValueInfo
    {
        private static string _regexLevels = null;

        public string ValidValueRegex
        {
            get 
            {
                if (_regexLevels == null)
                {
                    _regexLevels = LevelUtils.LevelRegex();
                }

                return _regexLevels; 
            }
        }

        public string ToJavaScript(string text)
        {
            string js = LevelUtils.LevelNumber(text).ToString();
            return js;
        }
    }
}
