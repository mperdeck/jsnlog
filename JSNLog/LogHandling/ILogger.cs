using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JSNLog;

namespace JSNLog.LogHandling
{
    public interface ILogger
    {
        void Log(Level level, string loggerName, string message);
    }
}
