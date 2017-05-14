#if NET40

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Logging;

namespace JSNLog.LogHandling
{
    // Unlike LoggingAdapter, CommonLoggingAdapter can be made internal, because it is purely used
    // as the default LoggingAdapter.

    internal class CommonLoggingAdapter: ILoggingAdapter
    {
        public void Log(FinalLogData finalLogData)
        {
            ILog log = LogManager.GetLogger(finalLogData.FinalLogger);

            switch (finalLogData.FinalLevel)
            {
                case Level.TRACE:
                    log.Trace(finalLogData.FinalMessage);
                    break;

                case Level.DEBUG:
                    log.Debug(finalLogData.FinalMessage);
                    break;

                case Level.INFO:
                    log.Info(finalLogData.FinalMessage);
                    break;

                case Level.WARN:
                    log.Warn(finalLogData.FinalMessage);
                    break;

                case Level.ERROR:
                    log.Error(finalLogData.FinalMessage);
                    break;

                case Level.FATAL:
                    log.Fatal(finalLogData.FinalMessage);
                    break;

                default:
                    throw new Exception(string.Format("Logger.Log - unknown level={0}", finalLogData.FinalLevel));
            }
        }
    }
}

#endif
