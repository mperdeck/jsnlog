using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JSNLog.ValueInfos
{
    /// <summary>
    /// This describes a class that describes how to handle values.
    /// </summary>
    internal interface IValueInfo
    {
        /// <summary>
        /// The value (without escaping) must match this regex.
        /// Return null if there is no regex (so no need to do any checking).
        /// </summary>
        string ValidValueRegex { get; }

        /// <summary>
        /// Takes a value and converts it to a JavaScript value.
        /// Note that this method takes care of quoting strings (and not quoting numbers and booleans).
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        string ToJavaScript(string text);
    }
}
