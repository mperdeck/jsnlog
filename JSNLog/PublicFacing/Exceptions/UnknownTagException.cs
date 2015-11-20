using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JSNLog.Exceptions
{
    public class UnknownTagException: JSNLogException
    {
        public UnknownTagException(string unknownTag): 
            base(string.Format("Unknown tag {0} - In web.config, this tag is either unknown, or is being used with the wrong parent tag", 
                    unknownTag))
        {
        }
    }
}
