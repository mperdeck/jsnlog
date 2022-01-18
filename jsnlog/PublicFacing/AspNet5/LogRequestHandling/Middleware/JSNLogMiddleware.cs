using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using JSNLog.Infrastructure.AspNet5;
using Microsoft.Extensions.Primitives;
using JSNLog.Infrastructure;
using JSNLog.LogHandling;
using Microsoft.Extensions.Logging;
using jsnlog.Infrastructure;

#if !NETCORE2
using Microsoft.AspNetCore.Http.Features;
#endif

// Be sure to leave the namespace at JSNLog.
namespace JSNLog
{
    /// <summary>
    /// Note that the OWIN counterpart of this (also in namespace JSNLog) 
    /// already has name JSNLogMiddlewareComponent
    /// </summary>
    public class JSNLogMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public JSNLogMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<JSNLogMiddleware>();
        }

        public async Task Invoke(HttpContext context)
        {
            // If this is a logging request (based on its url), do the logging and don't pass on the request
            // to the rest of the pipeline.
            string url = context.Request.GetDisplayUrl();
            if (LoggingUrlHelpers.IsLoggingUrl(url))
            {
                await LoggerRequestHelpers.ProcessLoggerRequestAsync(context, _logger);
                return;
            }

            // It was not a logging request

            JsnlogConfiguration jsnlogConfiguration = JavascriptLogging.GetJsnlogConfiguration();
            if (!jsnlogConfiguration.insertJsnlogInHtmlResponses)
            {
                // If automatic insertion is not on, simply call the rest of the pipeline and return.
                await _next(context);
                return;
            }

#if NETFRAMEWORK
            throw new Exception(
                "Automatic insertion of JSNLog into HTML pages is not supported in netstandard2.0. " +
                $"Upgrade to netstandard2.1 or for other options see {SiteConstants.InstallPageUrl}");
#else

            // Use a custom StreamWrapper to rewrite output on Write/WriteAsync, so it contains
            // the jsnlog.js script tag and JavaScript for jsnlog configuration

            string injectedCode = context.Configure(null);

            using (var filteredResponse = new ResponseStreamWrapper(context.Response.Body, context, injectedCode))
            {
#if !NETCORE2
                // Use new IHttpResponseBodyFeature for abstractions of pilelines/streams etc.
                // For 3.x this works reliably while direct Response.Body was causing random HTTP failures
                context.Features.Set<IHttpResponseBodyFeature>(new StreamResponseBodyFeature(filteredResponse));
#else
                context.Response.Body = filteredResponse;
#endif

                await _next(context);
            }
#endif
        }
    }
}
