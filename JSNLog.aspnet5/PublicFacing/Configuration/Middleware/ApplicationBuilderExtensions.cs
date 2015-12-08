using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNet.Builder;

namespace JSNLog
{
    public static class ApplicationBuilderExtensions
    {
        public static void UseJSNLog(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<JSNLogMiddlewareComponent>();
        }
    }
}
