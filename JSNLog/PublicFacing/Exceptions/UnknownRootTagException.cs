using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JSNLog.Exceptions
{
    public class UnknownRootTagException : JSNLogException
    {
        public UnknownRootTagException(string unknownTag): 
            base(string.Format(
                "Unknown root tag {0} - In web.config, an element with name {1} should be used to configure {2}. " +
                "Instead, {0} is used.", unknownTag, Constants.ConfigRootName, Constants.PackageName))
        {
        }
    }
}
