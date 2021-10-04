﻿using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

/// <summary>
/// This class based on 
/// https://weblog.west-wind.com/posts/2020/Mar/29/Content-Injection-with-Response-Rewriting-in-ASPNET-Core-3x
/// </summary>
namespace JSNLog.Infrastructure
{
    /// <summary>
    /// Wraps the Response Stream to inject the given script into all HTML pages just before the </body> tag.
    ///
    /// This class based on 
    /// https://weblog.west-wind.com/posts/2020/Mar/29/Content-Injection-with-Response-Rewriting-in-ASPNET-Core-3x
    /// </summary>
    public class ResponseStreamWrapper : Stream
    {
        private Stream _baseStream;
        private HttpContext _context;
        private string _scriptToInject;

        private bool _isContentLengthSet = false;

        public ResponseStreamWrapper(Stream baseStream, HttpContext context, string scriptToInject)
        {
            _baseStream = baseStream;
            _context = context;
            CanWrite = true;
            _scriptToInject = scriptToInject;
        }

        public override void Flush() => _baseStream.Flush();

        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            // this is called at the beginning of a request in 3.x and so
            // we have to set the ContentLength here as the flush/write locks headers
            if (!_isContentLengthSet && IsHtmlResponse())
            {
                _context.Response.Headers.ContentLength = null;
                _isContentLengthSet = true;
            }

            return _baseStream.FlushAsync(cancellationToken);
        }


        public override int Read(byte[] buffer, int offset, int count)
        {
            return _baseStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin) => _baseStream.Seek(offset, origin);

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return _baseStream.ReadAsync(buffer, offset, count, cancellationToken);
        }

        public override void SetLength(long value)
        {
            _baseStream.SetLength(value);
            IsHtmlResponse(forceReCheck: true);
        }

#if !NETFRAMEWORK
        public override void Write(ReadOnlySpan<byte> buffer)
        {
            _baseStream.Write(buffer);
        }
#endif

        public override void WriteByte(byte value)
        {
            _baseStream.WriteByte(value);
        }


        public override void Write(byte[] buffer, int offset, int count)
        {
            if (IsHtmlResponse())
            {
                ScriptInjectionHelper.InjectScriptAsync(buffer, offset, count, _context, _baseStream, _scriptToInject)
                                              .GetAwaiter()
                                              .GetResult();
            }
            else
                _baseStream.Write(buffer, offset, count);
        }

        public override async Task WriteAsync(byte[] buffer, int offset, int count,
                                              CancellationToken cancellationToken)
        {
            if (IsHtmlResponse())
            {
                await ScriptInjectionHelper.InjectScriptAsync(
                    buffer, offset, count,
                    _context, _baseStream, _scriptToInject);
            }
            else
                await _baseStream.WriteAsync(buffer, offset, count, cancellationToken);
        }


        private bool? _isHtmlResponse = null;
        

        private bool IsHtmlResponse(bool forceReCheck = false)
        {
            if (!forceReCheck && _isHtmlResponse != null)
                return _isHtmlResponse.Value;

            _isHtmlResponse =
                _context.Response?.Body != null &&
                _context.Response.StatusCode == 200 &&
                _context.Response.ContentType != null &&
                _context.Response.ContentType.Contains("text/html", StringComparison.OrdinalIgnoreCase) &&
                (_context.Response.ContentType.Contains("utf-8", StringComparison.OrdinalIgnoreCase) ||
                !_context.Response.ContentType.Contains("charset=", StringComparison.OrdinalIgnoreCase));

            if (!_isHtmlResponse.Value)
                return false;

            // Make sure we force dynamic content type since we're
            // rewriting the content - static content will set the header explicitly
            // and fail when it doesn't matchif (_isHtmlResponse.Value)
            if (!_isContentLengthSet && _context.Response.ContentLength != null)
            {
                _context.Response.Headers.ContentLength = null;
                _isContentLengthSet = true;
            } 
                
            return _isHtmlResponse.Value;
        }

#if !NETFRAMEWORK
        public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        {
            return _baseStream.WriteAsync(buffer, cancellationToken);
        }
#endif

        protected override void Dispose(bool disposing)
        {
            _baseStream?.Dispose();
            _baseStream = null;
            _context = null;

            base.Dispose(disposing);
        }


        public override bool CanRead { get; }
        public override bool CanSeek { get; }
        public override bool CanWrite { get; }
        public override long Length { get; }
        public override long Position { get; set; }
    }
}
