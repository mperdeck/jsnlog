using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Logging;

namespace JSNLog.LogHandling
{
    public class Logger: ILogger
    {
        public void Log(Constants.Level level, string loggerName, string message)
        {
            ILog log = LogManager.GetLogger(loggerName);

            switch (level)
            {
                case Constants.Level.TRACE:
                    log.Trace(message);
                    break;

                case Constants.Level.DEBUG:
                    log.Debug(message);
                    break;

                case Constants.Level.INFO:
                    log.Info(message);
                    break;

                case Constants.Level.WARN:
                    log.Warn(message);
                    break;

                case Constants.Level.ERROR:
                    log.Error(message);
                    break;

                case Constants.Level.FATAL:
                    log.Fatal(message);
                    break;

                default:
                    throw new Exception(string.Format("Logger.Log - unknown level={0}", level));
            }
        }
    }
}
