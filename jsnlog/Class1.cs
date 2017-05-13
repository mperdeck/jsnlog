using System;

namespace jsnlog
{
    public class Class1
    {
        public static string Get(string s)
        {
#if NET452
            string version = "NET452";
#else
            string version = "NETSTANDARD";
#endif
            return s + " " + version;
        }
    }
}
