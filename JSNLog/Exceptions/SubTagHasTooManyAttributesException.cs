using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JSNLog.Exceptions
{
    public class SubTagHasTooManyAttributesException: BaseException
    {
        public SubTagHasTooManyAttributesException(string subTagName, string allowedAttributeName) : 
            base(string.Format("Too many attributes - In web.config, you can have only attribute for the {0} tag, and it must be {1}", 
                                    subTagName, allowedAttributeName))
        {
        }
    }
}
