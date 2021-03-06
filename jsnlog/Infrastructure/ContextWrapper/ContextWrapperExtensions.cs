﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSNLog
{
    public static class ContextWrapperExtensions
    {
#if NETFRAMEWORK
        public static NetFrameworkContextWrapper Wrapper(this System.Web.HttpContext httpContext)
        {
            return new NetFrameworkContextWrapper(httpContext);
        }
#endif

        public static CoreContextWrapper Wrapper(this Microsoft.AspNetCore.Http.HttpContext httpContext)
        {
            return new CoreContextWrapper(httpContext);
        }
    }
}
