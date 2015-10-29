using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using JSNLog.Infrastructure;
using JSNLog.Exceptions;

namespace JSNLog.Elements
{
    internal abstract class AppenderElementBase: ElementBase
    {
        protected void ProcessAppender(XmlElement xe, string parentName, Dictionary<string, string> appenderNames, Sequence sequence,
            IEnumerable<AttributeInfo> appenderAttributes, string jsCreateMethodName, List<XmlHelpers.TagInfo> childTagInfos, StringBuilder sb)
        {
            if (xe == null) { return; }

            string appenderName = XmlHelpers.RequiredAttribute(xe, "name");

            string appenderVariableName = string.Format("{0}{1}", Constants.JsAppenderVariablePrefix, sequence.Next());
            appenderNames[appenderName] = appenderVariableName;

            JavaScriptHelpers.GenerateCreate(appenderVariableName, jsCreateMethodName, appenderName, sb);
            Utils.ProcessOptionAttributes(
                appenderVariableName, xe, appenderAttributes, null, sb, (attributeValues) => { Validate(appenderName, attributeValues); });

            XmlHelpers.ProcessNodeList(
                xe.ChildNodes,
                childTagInfos,
                appenderVariableName, appenderNames, sequence, sb);
        }

        public void Validate(string appenderName, AttributeValueCollection attributeValues)
        {
            // Ensure that if any of the buffer specific attributes are provided, they are all provided, and that they make sense.

            if (attributeValues.ContainsKey(Constants.AttributeNameSendWithBufferLevel) ||
                attributeValues.ContainsKey(Constants.AttributeNameStoreInBufferLevel) ||
                attributeValues.ContainsKey(Constants.AttributeNameBufferSize))
            {
                if ((!attributeValues.ContainsKey(Constants.AttributeNameSendWithBufferLevel)) ||
                    (!attributeValues.ContainsKey(Constants.AttributeNameStoreInBufferLevel)) ||
                    (!attributeValues.ContainsKey(Constants.AttributeNameBufferSize)))
                {
                    throw new GeneralAppenderException(appenderName, string.Format(
                        "If any of {0}, {1} or {2} is specified, than the other two need to be specified as well",
                        Constants.AttributeNameSendWithBufferLevel, Constants.AttributeNameStoreInBufferLevel, Constants.AttributeNameBufferSize));
                }

                int level =
                    attributeValues.ContainsKey(Constants.AttributeNameLevel) ?
                    LevelUtils.LevelNumber(attributeValues[Constants.AttributeNameLevel].Text) :
                    (int)Constants.DefaultAppenderLevel;

                int storeInBufferLevel = LevelUtils.LevelNumber(attributeValues[Constants.AttributeNameStoreInBufferLevel].Text);
                int sendWithBufferLevel = LevelUtils.LevelNumber(attributeValues[Constants.AttributeNameSendWithBufferLevel].Text);

                if ((storeInBufferLevel > level) || (level > sendWithBufferLevel))
                {
                    throw new GeneralAppenderException(appenderName, string.Format(
                        "{0} must be equal or greater than {1} and equal or smaller than {2}",
                        Constants.AttributeNameLevel, Constants.AttributeNameStoreInBufferLevel, 
                        Constants.AttributeNameSendWithBufferLevel));
                }
            }
        }
    }
}
