using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;

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
        /// <param name="jsnlogConfiguration">
        /// Note that if jsnlogConfiguration is set to null, the script tag and JavaScript config code will 
        /// automatically be inserted in html responses.
        /// </param>
        public static void UseJSNLog(this IApplicationBuilder builder,
            ILoggingAdapter loggingAdapter, JsnlogConfiguration jsnlogConfiguration = null)
        {
            JavascriptLogging.SetJsnlogConfiguration(jsnlogConfiguration, loggingAdapter);
            builder.UseMiddleware<JSNLogMiddleware>();
        }

        public static void UseJSNLog(this IApplicationBuilder builder,
            ILoggerFactory loggerFactory, JsnlogConfiguration jsnlogConfiguration = null)
        {
            var loggingAdapter = new LoggingAdapter(loggerFactory);
            UseJSNLog(builder, loggingAdapter, jsnlogConfiguration);
        }
    }
}
