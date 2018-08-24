using System;
using System.Collections.Generic;

namespace JSNLog
{
    public interface ILoggingBatchAdapter
    {
        void Process(IEnumerable<FinalLogData> finalLogData);
    }
}