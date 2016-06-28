using System;
#if !DNXCORE50
using System.Text.RegularExpressions;
#endif
using JSNLog.Exceptions;
using JSNLog.Infrastructure;

namespace JSNLog.ValueInfos
{
    internal class UrlValue : IValueInfo
    {
#if !DNXCORE50
        private static Regex regexUrl = new Regex(Constants.RegexUrl);
#endif

        Func<string, string> _virtualToAbsoluteFunc = null;
        public UrlValue(Func<string, string> virtualToAbsoluteFunc)
        {
            _virtualToAbsoluteFunc = virtualToAbsoluteFunc;
        }

        public string ToJavaScript(string text)
        {
#if !DNXCORE50
            if (!regexUrl.IsMatch(text))
            {
                throw new InvalidAttributeException(text);
            }
#endif

            string resolvedUrl = Utils.AbsoluteUrl(text, _virtualToAbsoluteFunc);
            return HtmlHelpers.JavaScriptStringEncode(resolvedUrl, true);
        }
    }
}
