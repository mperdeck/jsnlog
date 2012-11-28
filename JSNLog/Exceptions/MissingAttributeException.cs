using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace JSNLog.Exceptions
{
    public class MissingAttributeException: BaseException
    {
        public MissingAttributeException(XmlElement xe, string missingAttributeName): 
            base(string.Format("Missing attribute {0} in {1} tag - In web.config, every {1} tag must have a {0} attribute", 
                    missingAttributeName, xe.Name))
        {
        }
    }
}
