using System;
using System.Collections.Generic;
using System.Text;

namespace JSNLog
{
    public class ConsoleAppender : Appender, ICanCreateJsonFields, ICanCreateElement
    {
        // Implement ICanCreateElement
        public void CreateElement(StringBuilder sb, Dictionary<string, string> appenderNames, int sequence, Func<string, string> virtualToAbsoluteFunc)
        {
            CreateAppender(sb, appenderNames, sequence, virtualToAbsoluteFunc, "createConsoleAppender", "consoleAppender");
        }

        // Implement ICanCreateJsonFields
        public override void AddJsonFields(IList<string> jsonFields, Dictionary<string, string> appenderNames, Func<string, string> virtualToAbsoluteFunc)
        {
            base.AddJsonFields(jsonFields, appenderNames, virtualToAbsoluteFunc);
        }
    }
}
