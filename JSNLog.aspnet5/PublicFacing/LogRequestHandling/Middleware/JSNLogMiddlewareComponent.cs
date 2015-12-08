using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;

// Be sure to leave the namespace at JSNLog.
namespace JSNLog
{
    public class JSNLogMiddlewareComponent
    {
        private readonly RequestDelegate next;

        public JSNLogMiddlewareComponent(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);


                byte[] toBytes = Encoding.UTF8.GetBytes("<hr /><p>blah blah</p>");
                int len = toBytes.GetLength(0);

                context.Response.Body.Write(toBytes, 0, len);


            }
            catch (Exception)
            {
            }
        }
    }
}

