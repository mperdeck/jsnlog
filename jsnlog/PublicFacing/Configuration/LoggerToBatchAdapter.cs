using System.Collections.Generic;

namespace JSNLog
{
    public class LoggerToBatchAdapter : ILoggingBatchAdapter
    {
        private readonly ILoggingAdapter _adapter;

        private LoggerToBatchAdapter(ILoggingAdapter adapter)
        {
            _adapter = adapter;
        }

        public void Process(IEnumerable<FinalLogData> finalLogData)
        {
            foreach (var data in finalLogData)
            {
                _adapter.Log(data);
            }
        }

        public static ILoggingBatchAdapter ToBatch(ILoggingAdapter logger)
        {
            return logger == null ? null : new LoggerToBatchAdapter(logger);
        }
    }
}