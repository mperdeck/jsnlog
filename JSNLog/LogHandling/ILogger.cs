using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JSNLog;

namespace JSNLog.LogHandling
{
    internal interface ILogger
    {
        void Log(Level level, string loggerName, string message);
    }
}
