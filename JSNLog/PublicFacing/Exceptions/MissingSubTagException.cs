using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace JSNLog.Exceptions
{
    public class MissingSubTagException : JSNLogException
    {
        public MissingSubTagException(string tagName, string missingSubTagName) : 
            base(string.Format("Missing child element {0} in {1} tag - In web.config, every {1} tag must have a {0} child element",
                    missingSubTagName, tagName))
        {
        }
    }
}
