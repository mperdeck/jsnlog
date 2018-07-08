using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace jsnlog.Infrastructure.ContextWrapper
{
    public class NetFrameworkContextWrapper: ContextWrapperCommon
    {
        HttpContext _httpContext;

        public NetFrameworkContextWrapper(HttpContext httpContext)
        {
            _httpContext = httpContext;
        }
    }
}
