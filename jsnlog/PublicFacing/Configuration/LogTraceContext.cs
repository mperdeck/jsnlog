using System.Diagnostics;

namespace JSNLog
{
    public class LogTraceContext
    {
        public LogTraceContext(string traceId, string spanId, string parentId)
        {
            TraceId = traceId;
            SpanId = spanId;
            ParentId = parentId;
        }
        
        public string ParentId { get; }
        public string SpanId {get;}
        public string TraceId {get;}
    }
}