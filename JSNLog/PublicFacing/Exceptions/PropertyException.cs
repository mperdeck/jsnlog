using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JSNLog.Exceptions
{
    public class PropertyException : JSNLogException
    {
        public PropertyException(string propertyName, Exception innerException) :
            base(string.Format("Bad value for property {0}. See inner exception", propertyName), innerException)
        {
        }
    }
}
