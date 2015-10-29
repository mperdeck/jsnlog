using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JSNLog.Exceptions
{
    public class GeneralAppenderException : BaseException
    {
        public GeneralAppenderException(string appenderName, string message) :
            base(string.Format("Appender {0} - {1}",
                    appenderName, message))
        {
        }
    }
}
