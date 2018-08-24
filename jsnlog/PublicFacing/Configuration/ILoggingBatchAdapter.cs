using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JSNLog
{
    public interface ILoggingBatchAdapter
    {
        Task Process(IEnumerable<FinalLogData> finalLogData);
    }
}