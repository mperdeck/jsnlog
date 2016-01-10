using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JSNLog.Exceptions
{
    public class UnknownAppenderException : JSNLogException
    {
        public UnknownAppenderException(string unknownAppender) : 
            base(string.Format("Unknown appender {0} - In web.config, there is a reference to an appender with name '{0}', but that appender is not defined",
                    unknownAppender))
        {
        }
    }
}
