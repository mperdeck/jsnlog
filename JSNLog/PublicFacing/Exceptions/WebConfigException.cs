using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JSNLog.Exceptions
{
    public class WebConfigException : JSNLogException
    {
        public WebConfigException(Exception innerException) :
            base(string.Format("Error in {0} element or one of its children in your web.config. See inner exception", Constants.ConfigRootName), innerException)
        {
        }
    }
}
