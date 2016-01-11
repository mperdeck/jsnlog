using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JSNLog.LogHandling
{
    public class DefaultEmptyLogger : ILoggingAdapter
    {
        public void Log(FinalLogData finalLogData)
        {
            // does nothing at all
        }
    }
}
