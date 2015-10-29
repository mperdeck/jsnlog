using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JSNLog.ValueInfos;

namespace JSNLog.Infrastructure
{
    /// <summary>
    /// Describes a tag attribute
    /// </summary>
    internal class AttributeInfo
    {
        public enum AttributeValidityEnum
        {
            // This is a valid attribute, but won't be used as an option for setOptions
            NoOption,

            // Will be used as an option for setOptions. Optional.
            OptionalOption,

            // Will be used as an option for setOptions. Required.
            RequiredOption
        }

        public string Name { get; private set; }
        public IValueInfo ValueInfo { get; private set; }

        public AttributeValidityEnum AttributeValidity { get; private set; }

        // If this is null, this attribute is a normal attribute of the tag, with a single value.
        // If this is not null, the tag can have sub elements with this tag name (that is, a tag name equalling SubTagName).
        // These sub elements have one required attribute, with name Name, and a value described by ValueInfo.
        // The key used for the setOptions call is SubTagName.
        // The ultimate value is a JavaScript array.
        public string SubTagName { get; private set; }

        public AttributeInfo(string name, IValueInfo valueInfo, AttributeValidityEnum attributeValidity, string subTagName = null)
        {
            Name = name;
            ValueInfo = valueInfo ?? new StringValue();
            AttributeValidity = attributeValidity;
            SubTagName = subTagName;
        }
    }
}
