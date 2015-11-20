using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JSNLog
{
    /// <summary>
    /// Classes (loggers, etc.) that implement this interface can generate 
    /// JSON name-value fields based on their properties. These fields are used in the
    /// JSON object passed to setOptions.
    /// </summary>
    public interface ICanCreateJsonFields
    {
        /// <summary>
        /// Creates JSON fields for a JSON object that will be passed to setOptions
        /// for the element (logger, etc.) that implementes this interface.
        /// </summary>
        /// <param name="jsonFields">
        /// The JSON fields are added to this.
        /// </param>
        /// <param name="appenderNames"></param>
        /// <param name="virtualToAbsoluteFunc"></param>
        void AddJsonFields(IList<string> jsonFields, Dictionary<string, string> appenderNames, 
            Func<string, string> virtualToAbsoluteFunc);
    }
}
