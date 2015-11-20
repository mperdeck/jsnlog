using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace JSNLog.Exceptions
{
    public class MissingAttributeException : JSNLogException
    {
        public MissingAttributeException(string className, string missingPropertyName): 
            base(string.Format("Missing attribute {0} in {1}",
                    missingPropertyName, className))
        {
        }
    }
}
