using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JSNLog
{
    public enum Level
    {
        ALL = -2147483648,
        TRACE = 1000,
        DEBUG = 2000,
        INFO = 3000,
        WARN = 4000,
        ERROR = 5000,
        FATAL = 6000,
        OFF = 2147483647
    }
}
