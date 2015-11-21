using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using JSNLog.Infrastructure;

namespace JSNLog.ValueInfos
{
    internal class LevelValue : IValueInfo
    {
        private static string _regexLevels = null;

        public string ToJavaScript(string text)
        {
            string js = LevelUtils.LevelNumber(text).ToString();
            return js;
        }
    }
}
