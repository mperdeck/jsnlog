using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using JSNLog.Exceptions;
using System.Text.RegularExpressions;
using System.Web.Configuration;

namespace JSNLog.Infrastructure
{
    public class XmlHelpers
    {
        /// <summary>
        /// A method that processes an actual xml element.
        /// </summary>
        /// <param name="xe">
        /// The element.
        /// 
        /// If this is null, no elements of the given type were found.
        /// In that case, the method gets called so it can provide default values.
        /// 
        /// For example, if no appender-refs are defined for a logger, we want to generate JavaScript code
        /// to create a default AjaxAppender and attach that to the logger.
        /// </param>
        /// <param name="parentName">
        /// Name of the parent element. This is not the name of an XML element.
        /// Instead, the parent will be worked into a JavaScript object and that object assigned to a JavaScript variable.
        /// This is the name of that JavaScript variable.
        /// </param>
        /// <param name="appenderNames">
        /// Only relevant to some methods.
        /// 
        /// This is the collection of the names of all appenders.
        /// The key is the name of the appender (as set by the "name" attribute in the appender tag).
        /// The value is the the name of the JavaScript variable to which the appender gets assigned.
        /// </param>
        /// <param name="sequence">
        /// Used to get a unique ever increasing number if needed.
        /// </param>
        /// <param name="sb">
        /// The result of processing an xml element will be some text that should be injected in the page.
        /// This text will be appended to this StringBuilder.
        /// </param>
        public delegate void XmlElementProcessor(
            XmlElement xe, string parentName, Dictionary<string,string> appenderNames, Sequence sequence, StringBuilder sb);


        /// <summary>
        /// Associates a tag name (such as "logger") with the processor that handles a "logger" xml element.
        /// </summary>
        public class TagInfo
        {
            public string Tag { get; private set; }
            public XmlElementProcessor XmlElementProcessor { get; private set; }
            public Regex ValidAttributeName { get; private set; }
            public int MaxNbrTags { get; private set; }

            public TagInfo(string tag, XmlElementProcessor xmlElementProcessor, Regex validAttributeName, int maxNbrTags = int.MaxValue)
            {
                Tag = tag;
                XmlElementProcessor = xmlElementProcessor;
                ValidAttributeName = validAttributeName;
                MaxNbrTags = maxNbrTags;
            }
        }


        /// <summary>
        /// Processes a list of nodes. All XmlElements among the nodes are processed,
        /// any other types of nodes (comments, etc.) are ignored.
        /// </summary>
        /// <param name="xmlNodeList">
        /// The list of nodes
        /// </param>
        /// <param name="tagInfos">
        /// Specifies all tags (= node names) that are permissable. You get an exception if there is a node
        /// that is not listed here.
        /// 
        /// The nodes are not listed in the order in which they appear in the list. Instead, first all nodes
        /// with the name listed in the first TagInfo in tagInfos are processed. Then those in the second TagInfo, etc.
        /// 
        /// This way, you can for example first process all appenders, and then all loggers.
        /// 
        /// If there are no nodes at all for a tag name given in tagInfo, the 
        /// XmlElementProcessor given in the tagInfo is called once with a null XmlElement.
        /// </param>
        /// <param name="context">parentName, appenderNames, sequence and sb get passed to the processor method that is listed for each tag in tagInfos.</param>
        /// <param name="sequence"></param>
        /// <param name="sb"></param>
        public static void ProcessNodeList(
            XmlNodeList xmlNodeList, IEnumerable<TagInfo> tagInfos, string parentName, Dictionary<string,string> appenderNames, 
            Sequence sequence, StringBuilder sb)
        {
            // Ensure there are no child nodes with names that are unknown - that is, not listed in tagInfos

            foreach (XmlNode xmlNode in xmlNodeList)
            {
                XmlElement xe = xmlNode as XmlElement;
                if (xe != null)
                {
                    if (!(tagInfos.Any(t => t.Tag == xe.Name)))
                    {
                        throw new UnknownTagException(xe.Name);
                    }
                }
            }

            // Process each child node

            foreach (TagInfo tagInfo in tagInfos)
            {
                int nbrTagsFound = 0;
                foreach (XmlNode xmlNode in xmlNodeList)
                {
                    XmlElement xe = xmlNode as XmlElement;
                    if (xe != null)
                    {
                        if (xe.Name == tagInfo.Tag)
                        {
                            // Check that all attributes are valid
                            
                            foreach (XmlAttribute xa in xe.Attributes)
                            {
                                if ((tagInfo.ValidAttributeName == null) || (!tagInfo.ValidAttributeName.IsMatch(xa.Name)))
                                {
                                    throw new UnknownAttributeException(xe.Name, xa.Name);
                                }
                            }

                            nbrTagsFound++;

                            tagInfo.XmlElementProcessor(xe, parentName, appenderNames, sequence, sb);
                        }
                    }
                }

                if (nbrTagsFound > tagInfo.MaxNbrTags)
                {
                    throw new TooManyTagsException(tagInfo.Tag, tagInfo.MaxNbrTags, nbrTagsFound);
                }

                if (nbrTagsFound == 0)
                {
                    tagInfo.XmlElementProcessor(null, parentName, appenderNames, sequence, sb);
                }
            }
        }

        /// <summary>
        /// Returns the value of an attribute of an XmlElement.
        /// If the attribute is not found, an exception is thrown.
        /// 
        /// If a validValueRegex is given, then if the value is not null and does not match the regex,
        /// an exception is thrown.
        /// </summary>
        /// <param name="xe"></param>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        public static string RequiredAttribute(XmlElement xe, string attributeName, string validValueRegex = null)
        {
            XmlAttribute attribute = xe.Attributes[attributeName];

            if (attribute == null)
            {
                throw new MissingAttributeException(xe, attributeName);
            }

            string attributeValue = attribute.Value;

            ValidateAttributeValue(xe, attributeName, attributeValue, validValueRegex);

            return attributeValue.Trim();
        }

        /// <summary>
        /// Returns the value of an attribute of an XmlElement.
        /// If the attribute is not found, the default value is returned.
        /// 
        /// If a validValueRegex is given, then if the value is not null and does not match the regex,
        /// an exception is thrown.
        /// </summary>
        /// <param name="xe"></param>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        public static string OptionalAttribute(XmlElement xe, string attributeName, string defaultValue, string validValueRegex = null)
        {
            string attributeValue = defaultValue;
            
            XmlAttribute attribute = xe.Attributes[attributeName];
            if (attribute != null)
            {
                attributeValue = attribute.Value;
            }

            ValidateAttributeValue(xe, attributeName, attributeValue, validValueRegex);

            if (attributeValue != null)
            {
                attributeValue = attributeValue.Trim();
            }

            return attributeValue;
        }

        public static void ValidateAttributeValue(XmlElement xe, string attributeName, string value, string validValueRegex)
        {
            if (validValueRegex == null) { return; }
            if (value == null) { return; }

            if (!Regex.IsMatch(value, validValueRegex))
            {
                throw new InvalidAttributeException(xe, attributeName, value);
            }
        }

        public static XmlElement RootElement()
        {
            XmlElement xe = WebConfigurationManager.GetSection(Constants.ConfigRootName) as XmlElement;

            if (xe == null)
            {
                throw new MissingRootTagException();
            }

            if (xe.Name != Constants.ConfigRootName)
            {
                throw new UnknownRootTagException(xe.Name);
            }

            return xe;
        }
    }
}
