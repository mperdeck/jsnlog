using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JSNLog
{
    public class Constants
    {
        public const string PackageName = "JSNLog";
        public const string ConfigRootName = "jsnlog";
        public const string RegexLevels = "^(TRACE|DEBUG|INFO|WARN|ERROR|FATAL)$";
        public const string RegexBool = "^(true|false)$";
        public const string RegexPositiveInteger = "^[0-9]+$";
        public const string RegexIntegerGreaterZero = "^[1-9][0-9]*$";
    }
}
