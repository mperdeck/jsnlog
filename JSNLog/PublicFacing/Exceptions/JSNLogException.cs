using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JSNLog.Exceptions
{
    public class JSNLogException : Exception
    {
        public JSNLogException(string message, Exception innerException = null) :
            base(string.Format("{0} - {1}", Constants.PackageName, message), innerException)
        {
        }
    }
}
