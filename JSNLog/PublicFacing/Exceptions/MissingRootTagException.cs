using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JSNLog.Exceptions
{
    public class MissingRootTagException : JSNLogException
    {
        public MissingRootTagException(): 
            base(string.Format(
                "Missing root tag - In web.config, there is no {0} tag.", Constants.ConfigRootName))
        {
        }
    }
}
