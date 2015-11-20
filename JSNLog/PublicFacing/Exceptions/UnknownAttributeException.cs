using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JSNLog.Exceptions
{
    public class UnknownAttributeException : JSNLogException
    {
        public UnknownAttributeException(string tag, string unknownAttribute) :
            base(string.Format("Unknown attribute {0} - In web.config, a {1} tag cannot have a {0} attribute",
                    unknownAttribute, tag))
        {
        }
    }
}
