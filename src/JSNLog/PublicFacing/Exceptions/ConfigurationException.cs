using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JSNLog.Exceptions
{
    public class ConfigurationException : JSNLogException
    {
        public ConfigurationException(string elementName, Exception innerException) :
            base(string.Format("Error in configuration for {0}. See inner exception", elementName), innerException)
        {
        }
    }
}
