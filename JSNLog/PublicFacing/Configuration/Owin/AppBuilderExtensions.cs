using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Owin;

namespace JSNLog
{
    public static class AppBuilderExtensions
    {
        public static void UseJSNLog(this IAppBuilder app, string loggerUrlRegex)
        {
            app.Use<JsnlogMiddlewareComponent>(loggerUrlRegex);
        }
    }
}
