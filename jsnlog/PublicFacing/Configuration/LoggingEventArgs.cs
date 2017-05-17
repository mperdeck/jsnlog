using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JSNLog
{
    public class LoggingEventArgs : FinalLogData
    {
        public bool Cancel { get; set; }

        public LoggingEventArgs(ILogRequest logRequest)
            : base(logRequest)
        {
        }
    }
}
