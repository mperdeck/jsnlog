
using JSNLog.Infrastructure;

namespace JSNLog.ValueInfos
{
    internal class StringValue : IValueInfo
    {
        public string ToJavaScript(string text)
        {
            return HtmlHelpers.JavaScriptStringEncode(text, true);
        }
    }
}
