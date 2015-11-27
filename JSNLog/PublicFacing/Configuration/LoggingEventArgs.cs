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

        public string FinalLogger { get; set; }
        public Level FinalLevel { get; set; }
        public string FinalMessage { get; set; }
        public string ServerSideMessageFormat { get; internal set; }

        public LoggingEventArgs(ILogRequest logRequest)
        {
            LogRequest = logRequest;
        }
    }
}
