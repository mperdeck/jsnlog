using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JSNLog.Exceptions
{
    public class ConflictingConfigException : JSNLogException
    {
        public ConflictingConfigException() :
            base("To prevent conflicting configurations, you cannot set the JsnlogConfiguration property if there is a jsnlog element in your web.config.", null)
        {
        }
    }
}
