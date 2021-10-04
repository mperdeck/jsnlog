using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

/// <summary>
/// This class based on 
/// https://weblog.west-wind.com/posts/2020/Mar/29/Content-Injection-with-Response-Rewriting-in-ASPNET-Core-3x
/// </summary>
namespace JSNLog.Infrastructure
{

    public static class ScriptInjectionHelper
    {
        private const string _bodyMarker = "</body>";
        private static readonly byte[] _bodyBytes = Encoding.UTF8.GetBytes(_bodyMarker);

        /// <summary>
        /// Adds script into the page before the body tag.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <param name="context"></param>
        /// <param name="baseStream">The raw Response Stream</param>
        /// <param name="scriptToInject">
        /// String to inject before the </body> tag
        /// </param>
        /// <returns></returns>
        public static Task InjectScriptAsync(byte[] buffer, int offset, int count, HttpContext context, Stream baseStream, string scriptToInject)
        {
            Span<byte> currentBuffer = buffer;
            var curBuffer = currentBuffer.Slice(offset, count).ToArray();
            return InjectScriptAsync(curBuffer, context, baseStream, scriptToInject);
        }

        /// <summary>
        /// Adds script into the page before the body tag.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="context"></param>
        /// <param name="baseStream">The raw Response Stream</param>
        /// <returns></returns>
        public static async Task InjectScriptAsync(byte[] buffer, HttpContext context, Stream baseStream, string scriptToInject)
        {
            int index = buffer.LastIndexOf(_bodyBytes);
            if (index == -1)
            {
                await baseStream.WriteAsync(buffer, 0, buffer.Length);
                return;
            }

            var endIndex = index + _bodyBytes.Length;

            // Write pre-marker buffer
            await baseStream.WriteAsync(buffer, 0, index - 1);

            // Write the injected script
            var scriptBytes = Encoding.UTF8.GetBytes(scriptToInject);
            await baseStream.WriteAsync(scriptBytes, 0, scriptBytes.Length);

            // Write the rest of the buffer/HTML doc
            await baseStream.WriteAsync(buffer, endIndex, buffer.Length - endIndex);
        }

        static int LastIndexOf<T>(this T[] array, T[] sought) where T : IEquatable<T> =>
            array.AsSpan().LastIndexOf(sought);
    }
}
