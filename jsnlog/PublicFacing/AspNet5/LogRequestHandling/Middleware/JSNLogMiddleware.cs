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

/// <summary>
/// This class based on 
/// https://weblog.west-wind.com/posts/2020/Mar/29/Content-Injection-with-Response-Rewriting-in-ASPNET-Core-3x
/// </summary>

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
                // If automatic insertion is not enabled, simply call the rest of the pipeline and return.
                await _next(context);
                return;
            }

#if NETFRAMEWORK
            throw new Exception(
                "Automatic insertion of JSNLog into HTML pages is not supported in netstandard2.0. " +
                $"Upgrade to netstandard2.1 or for other options see {SiteConstants.InstallPageUrl}");
#else
            // Check other content for HTML
            await HandleHtmlInjection(context);
#endif
        }

#if !NETFRAMEWORK
        /// <summary>
        /// Inspects the responses for all requests for HTML documents
        /// and injects the JavaScript to configure JSNLog client side.
        ///
        /// Uses a wrapper stream to wrap the response and examine
        /// only text/html requests - other content is passed through
        /// as is.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private async Task HandleHtmlInjection(HttpContext context)
        {
            var path = context.Request.Path.Value;

            // Use a custom StreamWrapper to rewrite output on Write/WriteAsync
            using (var filteredResponse = new ResponseStreamWrapper(context.Response.Body, context))
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
        }
#endif
    }
}
