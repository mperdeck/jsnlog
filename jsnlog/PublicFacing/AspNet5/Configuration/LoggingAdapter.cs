using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using JSNLog.Infrastructure;

namespace JSNLog
{
    // Unlike CommonLoggingAdapter, LoggingAdapter has to be public, so the user can instantiate it to make ILoggingFactory
    // available in an ASP.NET 5 environment.

    public class LoggingAdapter : ILoggingAdapter
    {
        private ILoggerFactory _loggerFactory;

        public LoggingAdapter(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        public void Log(FinalLogData finalLogData)
        {
            ILogger logger = _loggerFactory.CreateLogger(finalLogData.FinalLogger);

            Object message = LogMessageHelpers.DeserializeIfPossible(finalLogData.FinalMessage);

            IDisposable scope = null;
            try
            {
                if (finalLogData.LogRequest.TraceContext != null)
                {
                    scope = logger.BeginScope("Client TraceContext: TraceId={TraceId},SpanId={SpanId},ParentId={ParentId}",
                        finalLogData.LogRequest.TraceContext.TraceId,
                        finalLogData.LogRequest.TraceContext.SpanId,
                        finalLogData.LogRequest.TraceContext.ParentId ?? "0000000000000000");
                }

                switch (finalLogData.FinalLevel)
                {
                    case Level.TRACE:
                        logger.LogTrace("{logMessage}", message);
                        break;
                    case Level.DEBUG:
                        logger.LogDebug("{logMessage}", message);
                        break;
                    case Level.INFO:
                        logger.LogInformation("{logMessage}", message);
                        break;
                    case Level.WARN:
                        logger.LogWarning("{logMessage}", message);
                        break;
                    case Level.ERROR:
                        logger.LogError("{logMessage}", message);
                        break;
                    case Level.FATAL:
                        logger.LogCritical("{logMessage}", message);
                        break;
                }
            }
            finally
            {
                scope?.Dispose();
            }
        }
    }
}
