using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace jsnlog.Infrastructure.ContextWrapper
{
    public class CoreContextWrapper : ContextWrapperCommon
    {
        HttpContext _httpContext;

        public CoreContextWrapper(HttpContext httpContext)
        {
            _httpContext = httpContext;
        }
    }
}
