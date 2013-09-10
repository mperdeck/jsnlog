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
        /// <param name="attributeInfos">
        /// The attributes expected by this element.
        /// </param>
        /// <param name="sb">
        /// The result of processing an xml element will be some text that should be injected in the page.
        /// This text will be appended to this StringBuilder.
        /// </param>
        public delegate void XmlElementProcessor(
            XmlElement xe, string parentName, Dictionary<string,string> appenderNames, Sequence sequence, IEnumerable<AttributeInfo> attributeInfos, StringBuilder sb);



        /// <summary>
        /// Associates a tag name (such as "logger") with the processor that handles a "logger" xml element.
        /// </summary>
        public class TagInfo
        {
            // Name of the tag
            public string Tag { get; private set; }
            
            public XmlElementProcessor XmlElementProcessor { get; private set; }
            public IEnumerable<AttributeInfo> AttributeInfos { get; private set; }

            // Determines the order in which each element type is processed. Lower orderNbr goes first.
            // Note that this only determines the order for elements from the same assembly.
            // Elements from an assembly loaded earlier will always be loaded earlier.
            //
            // >>>This has to be an int rather than a Constants.OrderNbr, to enable elements created in external
            // assemblies to create TagInfos.
            public int OrderNbr { get; private set; }

            public int MaxNbrTags { get; private set; }

            public TagInfo(string tag, XmlElementProcessor xmlElementProcessor, IEnumerable<AttributeInfo> attributeInfos, 
                int orderNbr, int maxNbrTags = int.MaxValue)
            {
                Tag = tag;
                XmlElementProcessor = xmlElementProcessor;
                AttributeInfos = attributeInfos;
                OrderNbr = orderNbr;
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
        /// that is not listed here. However, see childNodeNameRegex.
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
        /// <param name="childNodeNameRegex">
        /// If this is given, then only those nodes in xmlNodeList whose name matches childNodeNameRegex will be processed.
        /// The other nodes will be completely ignored.
        /// 
        /// If this is not given, no filtering takes place and all nodes are processed.
        /// </param>
        public static void ProcessNodeList(
            XmlNodeList xmlNodeList, List<TagInfo> tagInfos, string parentName, Dictionary<string,string> appenderNames, 
            Sequence sequence, StringBuilder sb, string childNodeNameRegex = ".*")
        {
            // Ensure there are no child nodes with names that are unknown - that is, not listed in tagInfos

            foreach (XmlNode xmlNode in xmlNodeList)
            {
                XmlElement xe = xmlNode as XmlElement;
                if (xe != null)
                {
                    if (!Regex.IsMatch(xe.Name, childNodeNameRegex)) { continue; }

                    if (!(tagInfos.Any(t => t.Tag == xe.Name)))
                    {
                        throw new UnknownTagException(xe.Name);
                    }
                }
            }

            // Process each child node
            //
            // Note that the tagInfo list may be added to when tagInfo.XmlElementProcessor is called.
            // Because of this, use for, not foreach

            for (int i = 0; i < tagInfos.Count; i++ )
            {
                TagInfo tagInfo = tagInfos[i];

                int nbrTagsFound = 0;
                foreach (XmlNode xmlNode in xmlNodeList)
                {
                    XmlElement xe = xmlNode as XmlElement;

                    // If xmlNode is not an XmlElement, ignore it.
                    // This will be the case when xmlNode is a comment.
                    if (xe == null) { continue; }

                    if (!Regex.IsMatch(xe.Name, childNodeNameRegex)) { continue; }

                    if (xe != null)
                    {
                        if (xe.Name == tagInfo.Tag)
                        {
                            // Check that all attributes are valid

                            EnsureAllAttributesKnown(xe, tagInfo.AttributeInfos);

                            nbrTagsFound++;

                            tagInfo.XmlElementProcessor(xe, parentName, appenderNames, sequence, tagInfo.AttributeInfos, sb);
                        }
                    }
                }

                if (nbrTagsFound > tagInfo.MaxNbrTags)
                {
                    throw new TooManyTagsException(tagInfo.Tag, tagInfo.MaxNbrTags, nbrTagsFound);
                }

                if (nbrTagsFound == 0)
                {
                    tagInfo.XmlElementProcessor(null, parentName, appenderNames, sequence, tagInfo.AttributeInfos, sb);
                }
            }
        }

        /// <summary>
        /// Ensures that all attributes of xe are actually in attributeInfos.
        /// If not, an exception is thrown.
        /// </summary>
        /// <param name="xe"></param>
        /// <param name="attributeInfos"></param>
        public static void EnsureAllAttributesKnown(
            XmlElement xe, IEnumerable<AttributeInfo> attributeInfos)
        {
            if (attributeInfos == null) { return; }

            foreach (XmlAttribute xmlAttribute in xe.Attributes)
            {
                string attributeName = xmlAttribute.Name;
                if (!(attributeInfos.Any(t => t.Name == attributeName)))
                {
                    throw new UnknownAttributeException(xe.Name, attributeName);
                }
            }
        }

        /// <summary>
        /// Reads the attributes of xe and adds their names and values to attributeValues.
        /// 
        /// Validates the attributes based on the contents of attributeInfos. Attributes that have been
        /// listed as Ignore will not be added to attributeValues.
        /// </summary>
        /// <param name="xe"></param>
        /// <param name="attributeInfos"></param>
        /// <param name="attributeValues">
        /// key: attribute name
        /// value: value of the attribute. Cannot be null.
        /// </param>
        public static void ProcessAttributes(
            XmlElement xe, IEnumerable<AttributeInfo> attributeInfos, AttributeValueCollection attributeValues)
        {
            // Ensure there are no unknown attributes - that is, not listed in attributeInfos

            EnsureAllAttributesKnown(xe, attributeInfos);

            // Process all attributes that should not be ignored

            foreach (AttributeInfo attributeInfo in attributeInfos)
            {
                // Get value of the attribute and validate that it is correct. We want to do this for all attributes.
                string attributeName = attributeInfo.Name;
                string attributeValueText = OptionalAttribute(xe, attributeName, null, attributeInfo.ValueInfo.ValidValueRegex);

                if (attributeInfo.AttributeValidity == AttributeInfo.AttributeValidityEnum.NoOption) { continue; }

                if (attributeValueText != null)
                {
                    attributeValues[attributeName] = new Value(attributeValueText, attributeInfo.ValueInfo);
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
