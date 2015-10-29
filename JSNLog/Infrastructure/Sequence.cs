using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JSNLog.Infrastructure
{
    internal class Sequence
    {
        private int _current = -1;

        public int Next()
        {
            _current++;
            return _current;
        }
    }
}
