using JSNLog.Infrastructure;

namespace JSNLog.ValueInfos
{
    internal class LevelValue : IValueInfo
    {
        public string ToJavaScript(string text)
        {
            string js = LevelUtils.LevelNumber(text).ToString();
            return js;
        }
    }
}
