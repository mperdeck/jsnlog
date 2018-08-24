using System;
using System.Linq;
using System.Text;
using JSNLog;

namespace JSNLog
{
    public interface ILoggingAdapter
    {
        void Log(FinalLogData finalLogData);
    }
}
