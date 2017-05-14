// All unit tests run under dotnet cli
#if SUPPORTSXML

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using JSNLog.Exceptions;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

#if NET40
using System.Web.Configuration;
#endif

namespace JSNLog.Infrastructure
{
    internal class XmlHelpers
    {

        /// <summary>
        /// Takes an XML element and converts it to an object of type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlNoot"></param>
        /// <returns></returns>
        internal static T DeserialiseXml<T>(XmlNode xmlNode)
        {
            try
            {
                var xmlNodeReader = new XmlNodeReader(xmlNode);
                XmlSerializer deserializer = new XmlSerializer(typeof(T));
                T result = (T)deserializer.Deserialize(xmlNodeReader);
                return result;
            }
            catch (Exception e)
            {
                throw new WebConfigException(e);
            }
        }

#if NET40
        public static XmlElement RootElement()
        {
            XmlElement xe = WebConfigurationManager.GetSection(Constants.ConfigRootName) as XmlElement;

            if (xe == null)
            {
                return null;
            }

            if (xe.Name != Constants.ConfigRootName)
            {
                throw new UnknownRootTagException(xe.Name);
            }

            return xe;
        }
#endif

    }
}

#endif
