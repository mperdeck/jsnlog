using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JSNLog
{
    /// <summary>
    /// Elements (loggers, etc.) that implement this interface can generate JavaScript that 
    /// creates the element.
    /// 
    /// These elements must have a display name that can be used in exceptions.
    /// </summary>
    public interface ICanCreateElement
    {
        /// <summary>
        /// Creates JavaScript code that creates the element. For example JL("loggername").setOptions(...);
        /// for a logger.
        /// </summary>
        /// <param name="sb">
        /// The JavaScript code is added to this.
        /// </param>
        /// <param name="appenderNames">
        /// Provides mapping from appender configuration names to their JavaScript names.
        /// Appenders add their details to this.
        /// </param>
        /// <param name="sequence">
        /// Every call to this method receives a unique sequence number.
        /// Used to create unique JavaScript names.
        /// </param>
        /// <param name="virtualToAbsoluteFunc">
        /// Used to translate virtual paths.
        /// </param>
        void CreateElement(StringBuilder sb, Dictionary<string, string> appenderNames, 
            int sequence, Func<string, string> virtualToAbsoluteFunc);
    }
}
