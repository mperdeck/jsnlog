using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JSNLog.ValueInfos;

namespace JSNLog.Infrastructure
{
    public class Value
    {
        /// <summary>
        /// The (unescaped) text representing the value
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// Info about the value
        /// </summary>
        public IValueInfo ValueInfo { get; private set; }

        /// <summary>
        /// Returns the value in a form that can be used in JavaScript code.
        /// </summary>
        /// <returns></returns>
        public string AsJavaScript()
        {
            return ValueInfo.ToJavaScript(Text);
        }

        public Value(string text, IValueInfo valueInfo)
        {
            Text = text;
            ValueInfo = valueInfo;
        }
    }
}
