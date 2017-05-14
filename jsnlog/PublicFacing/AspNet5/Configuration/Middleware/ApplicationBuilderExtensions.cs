#if !NET40

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Builder;

namespace JSNLog
{
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Normally, an ASP.NET 5 app would simply call this to insert JSNLog middleware into the pipeline.
        /// Note that the loggingAdapter is required, otherwise JSNLog can't hand off log messages.
        /// It can live without a configuration though (it will use default settings).
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="loggingAdapter"></param>
        /// <param name="jsnlogConfiguration"></param>
        public static void UseJSNLog(this IApplicationBuilder builder,
            ILoggingAdapter loggingAdapter, JsnlogConfiguration jsnlogConfiguration = null)
        {
            JavascriptLogging.SetJsnlogConfiguration(jsnlogConfiguration, loggingAdapter);
            builder.UseMiddleware<JSNLogMiddleware>();
        }
    }
}

#endif
