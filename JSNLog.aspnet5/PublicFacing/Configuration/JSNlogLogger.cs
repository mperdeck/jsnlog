using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using JSNLog.Infrastructure;

namespace JSNLog
{
    public class JSNlogLogger : IJSNLogLogger
    {
        private ILoggerFactory _loggerFactory;

        public JSNlogLogger(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        public void Log(FinalLogData finalLogData)
        {
            ILogger logger = _loggerFactory.CreateLogger(finalLogData.FinalLogger);

            Object message = LogMessageHelpers.DeserializeIfPossible(finalLogData.FinalMessage);

            switch (finalLogData.FinalLevel)
            {
                case Level.TRACE: logger.LogDebug("{logMessage}", message); break;
                case Level.DEBUG: logger.LogVerbose("{logMessage}", message); break;
                case Level.INFO: logger.LogInformation("{logMessage}", message); break;
                case Level.WARN: logger.LogWarning("{logMessage}", message); break;
                case Level.ERROR: logger.LogError("{logMessage}", message); break;
                case Level.FATAL: logger.LogCritical("{logMessage}", message); break;
            }
        }
    }
}
