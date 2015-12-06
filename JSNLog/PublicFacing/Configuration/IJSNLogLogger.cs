using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JSNLog;

namespace JSNLog
{
    public interface IJSNLogLogger
    {
        void Log(FinalLogData finalLogData);
    }
}
