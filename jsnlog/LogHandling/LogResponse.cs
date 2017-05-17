using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JSNLog.LogHandling
{
    internal class LogResponse
    {
        private Dictionary<string, string> _headers = new Dictionary<string, string>();

        public Dictionary<string, string> Headers
        {
            get { return _headers; }
        }

        public int StatusCode { get; set; }

        public void AppendHeader(string name, string value)
        {
            _headers[name] = value;
        }
    }
}
