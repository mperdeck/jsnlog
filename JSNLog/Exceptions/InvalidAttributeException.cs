using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace JSNLog.Exceptions
{
    public class InvalidAttributeException: BaseException
    {
        public InvalidAttributeException(XmlElement xe, string attributeName, string invalidValue) : 
            base(string.Format("Invalid attribute value {0} - In web.config, the {1} attribute of a {2} tag has invalid value {0}",
                    invalidValue, attributeName, xe.Name))
        {
        }
    }
}
