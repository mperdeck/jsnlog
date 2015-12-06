using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JSNLog
{
    public class FinalLogData
    {
        public ILogRequest LogRequest { get; private set; }

        public string FinalLogger { get; set; }
        public Level FinalLevel { get; set; }
        public string FinalMessage { get; set; }
        public string ServerSideMessageFormat { get; internal set; }

        public FinalLogData(ILogRequest logRequest)
        {
            LogRequest = logRequest;
        }

        public override string ToString()
        {
            return string.Format(
                "FinalLogger: {0}, FinalLevel: {1}, FinalMessage: {2}, ServerSideMessageFormat: {3}, LogRequest: {{{5}}}",
                FinalLogger, FinalLevel, FinalMessage, ServerSideMessageFormat, LogRequest);
        }
    }
}
