using System.Collections.Generic;
using System.Threading.Tasks;

namespace JSNLog
{
    public class LoggerToBatchAdapter : ILoggingBatchAdapter
    {
        private readonly ILoggingAdapter _adapter;

        private LoggerToBatchAdapter(ILoggingAdapter adapter)
        {
            _adapter = adapter;
        }

        public Task Process(IEnumerable<FinalLogData> finalLogData)
        {
            foreach (var data in finalLogData)
            {
                _adapter.Log(data);
            }

            return Task.FromResult(1);
        }

        public static ILoggingBatchAdapter ToBatch(ILoggingAdapter logger)
        {
            return logger == null ? null : new LoggerToBatchAdapter(logger);
        }
    }
}