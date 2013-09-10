using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;

namespace JSNLog.Infrastructure
{
    public class Utils
    {
        /// <summary>
        /// Generates the JavaScript for the call to setOptions to set the options for an object
        /// (the JL object itself, an appender or a logger).
        /// </summary>
        /// <param name="parentName">
        /// JavaScript variable that holds the element.
        /// </param>
        /// <param name="xe">
        /// XML element. The attributes on this element will provide the values for the options.
        /// </param>
        /// <param name="attributeInfos">
        /// Describes which attributes to use as options and how to validate them.
        /// </param>
        /// <param name="initialAttributeValues">
        /// Initial attribute values. The elements found in xe will be added to this.
        /// If null, this method will create an empty collection itself.
        /// </param>
        /// <param name="sb">
        /// The JS code is added to this.
        /// </param>
        /// <param name="validate">
        /// If not null, this method is called on the generated attribute values. This can be used to throw an exception
        /// if the given parameters are not valid.
        /// </param>
        public static void ProcessOptionAttributes(string parentName, XmlElement xe, IEnumerable<AttributeInfo> attributeInfos,
            AttributeValueCollection initialAttributeValues, StringBuilder sb,
            Action<AttributeValueCollection> validate = null)
        {
            var attributeValues = initialAttributeValues ?? new AttributeValueCollection();
            XmlHelpers.ProcessAttributes(xe, attributeInfos, attributeValues);

            if (validate != null) { validate(attributeValues); }

            JavaScriptHelpers.GenerateSetOptions(parentName, attributeValues, sb);
        }

        /// <summary>
        /// Creates an ID that is unique hopefully.
        /// 
        /// This method initially tries to use the request id that IIS already uses internally. This allows us to correlate across even more log files.
        /// If this fails, for example if this is not part of a web request, than it uses a random GUID.
        /// 
        /// See
        /// http://blog.tatham.oddie.com.au/2012/02/07/code-request-correlation-in-asp-net/
        /// </summary>
        /// <returns></returns>
        public static string CreateRequestId()
        {
            var provider = (IServiceProvider)HttpContext.Current;
            if (provider != null)
            {
                var workerRequest = (HttpWorkerRequest)provider.GetService(typeof(HttpWorkerRequest));
                if (workerRequest != null)
                {
                    Guid requestIdGuid = workerRequest.RequestTraceIdentifier;
                    if (requestIdGuid != Guid.Empty)
                    {
                        return requestIdGuid.ToString();
                    }
                }
            }

            // Couldn't get the GUID as used in in all ETW outputs by IIS. So, use a random GUID.
            return Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Takes a DateTime in UTC and returns the same timestamp in local time.
        /// </summary>
        /// <param name="utcTime"></param>
        /// <returns></returns>
        public static DateTime UtcToLocalDateTime(DateTime utcTime)
        {
            TimeZoneInfo localZone = TimeZoneInfo.Local;
            DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, localZone);
            return localTime;
        }
    }
}
