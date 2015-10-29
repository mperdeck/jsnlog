using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JSNLog
{
    public class LoggingEventArgs
    {
        public ILogRequest LogRequest { get; private set; }

        public bool Cancel { get; set; }

        public string finalLogger { get; set; }
        public Level finalLevel { get; set; }
        public string finalMessage { get; set; }

        public LoggingEventArgs(ILogRequest logRequest)
        {
            LogRequest = logRequest;
        }
    }
}
