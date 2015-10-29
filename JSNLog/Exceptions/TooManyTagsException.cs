using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JSNLog.Exceptions
{
    public class TooManyTagsException : BaseException
    {
        public TooManyTagsException(string tag, int maxNbr, int nbrUsed): 
            base(string.Format("Too many {0} tags - In web.config, you can have up to {1} {0} tags under a parent, but you used {2}", 
                                    tag, maxNbr, nbrUsed))
        {
        }
    }
}
