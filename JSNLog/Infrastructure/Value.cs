using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JSNLog.ValueInfos;

namespace JSNLog.Infrastructure
{
    /// <summary>
    /// Holds either a single piece of string, or a collection of strings. 
    /// These would be the values of xml attributes.
    /// You can only have either a string or a collection.
    /// 
    /// Also holds a ValueInfo object describing how to process each string.
    /// </summary>
    internal class Value
    {
        /// <summary>
        /// The (unescaped) text representing the value
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// The (unescaped) string collection representing the value
        /// </summary>
        public IEnumerable<string> TextCollection { get; private set; }

        /// <summary>
        /// Info about the value
        /// </summary>
        public IValueInfo ValueInfo { get; private set; }

        public Value(string text, IValueInfo valueInfo)
        {
            Text = text;
            ValueInfo = valueInfo;
        }

        public Value(IEnumerable<string> textCollection, IValueInfo valueInfo)
        {
            TextCollection = textCollection;
            ValueInfo = valueInfo;
        }
    }
}
