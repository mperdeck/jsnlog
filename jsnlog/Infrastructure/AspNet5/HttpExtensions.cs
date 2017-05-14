#if !NET40

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace JSNLog.Infrastructure.AspNet5
{
    public static class HttpExtensions
    {
        // This method copied from
        // https://github.com/aspnet/HttpAbstractions/blob/dev/src/Microsoft.AspNet.Http.Extensions/UriHelper.cs
        // Once you can install package Microsoft.AspNet.Http.Extensions again, you can use the version in there.

        private const string SchemeDelimiter = "://";
        public static string GetDisplayUrl(this HttpRequest request)
        {
            var host = request.Host.Value;
            var pathBase = request.PathBase.Value;
            var path = request.Path.Value;
            var queryString = request.QueryString.Value;

            // PERF: Calculate string length to allocate correct buffer size for StringBuilder.
            var length = request.Scheme.Length + SchemeDelimiter.Length + host.Length
                + pathBase.Length + path.Length + queryString.Length;

            return new StringBuilder(length)
                .Append(request.Scheme)
                .Append(SchemeDelimiter)
                .Append(host)
                .Append(pathBase)
                .Append(path)
                .Append(queryString)
                .ToString();
        }
    }
}

#endif
