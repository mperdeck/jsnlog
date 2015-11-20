using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Logging;

namespace JSNLog.LogHandling
{
    internal class CommonLoggingLogger: ILogger
    {
        public void Log(Level level, string loggerName, string message)
        {
            ILog log = LogManager.GetLogger(loggerName);

            switch (level)
            {
                case Level.TRACE:
                    log.Trace(message);
                    break;

                case Level.DEBUG:
                    log.Debug(message);
                    break;

                case Level.INFO:
                    log.Info(message);
                    break;

                case Level.WARN:
                    log.Warn(message);
                    break;

                case Level.ERROR:
                    log.Error(message);
                    break;

                case Level.FATAL:
                    log.Fatal(message);
                    break;

                default:
                    throw new Exception(string.Format("Logger.Log - unknown level={0}", level));
            }
        }
    }
}
