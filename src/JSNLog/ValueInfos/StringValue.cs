using System.Web;

namespace JSNLog.ValueInfos
{
    internal class StringValue : IValueInfo
    {
        public string ToJavaScript(string text)
        {
            return HttpUtility.JavaScriptStringEncode(text, true);
        }
    }
}
