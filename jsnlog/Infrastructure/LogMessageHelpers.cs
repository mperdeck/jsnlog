using System;
using System.Collections.Generic;
using System.Text.Json;

namespace JSNLog.Infrastructure
{
    internal class LogMessageHelpers
    {
        public static T DeserializeJson<T>(string json)
        {
            T result = JsonSerializer.Deserialize<T>(json);
            return result;
        }

        public static bool IsPotentialJson(string msg)
        {
            string trimmedMsg = msg.Trim();
            return (trimmedMsg.StartsWith("{") && trimmedMsg.EndsWith("}"));
        }

        /// <summary>
        /// Tries to deserialize msg.
        /// If that works, returns the resulting object.
        /// Otherwise returns msg itself (which is a string).
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static Object DeserializeIfPossible(string msg)
        {
            try
            {
                if (IsPotentialJson(msg))
                {
                    Object result = DeserializeJson<Object>(msg);
                    return result;
                }
            }
            catch
            {
            }

            return msg;
        }

        /// <summary>
        /// Returns true if the msg contains a valid JSON string.
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static bool IsJsonString(string msg)
        {
            try
            {
                if (IsPotentialJson(msg))
                {
                    // Try to deserialise the msg. If that does not throw an exception,
                    // decide that msg is a good JSON string.

                    DeserializeJson<Dictionary<string, Object>>(msg);

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

            return HtmlHelpers.JavaScriptStringEncode(msg, true);
        }
    }
}
