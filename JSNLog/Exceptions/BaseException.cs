using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JSNLog.Exceptions
{
    public class BaseException : Exception
    {
        public BaseException(string message): 
            base(string.Format("{0} - {1}", Constants.PackageName, message))
        {
        }
    }
}
