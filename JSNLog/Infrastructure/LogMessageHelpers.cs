using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Web;

namespace JSNLog.Infrastructure
{
    internal class LogMessageHelpers
    {
        /// <summary>
        /// Returns true if the msg contains a valid JSON string.
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static bool IsJsonString(string msg)
        {
            try
            {
                if (msg.TrimStart().StartsWith("{"))
                {
                    // Try to deserialise the msg. If that does not throw an exception,
                    // decide that msg is a good JSON string.

                    JavaScriptSerializer js = new JavaScriptSerializer();
                    js.Deserialize<Dictionary<string, Object>>(msg);

                    return true;
                }
            }
            catch
            {
            }

            return false;
        }

        /// <summary>
        /// Takes a log message and finds out if it contains a valid JSON string.
        /// If so, returns it unchanged.
        /// 
        /// Otherwise, surrounds the string with quotes (") and escapes the string for JavaScript.
        /// </summary>
        /// <param name="ms"></param>
        /// <returns></returns>
        public static string EnsureValidJson(string msg)
        {
            if (IsJsonString(msg))
            {
                return msg;
            }

            return HttpUtility.JavaScriptStringEncode(msg, true);
        }
    }
}
