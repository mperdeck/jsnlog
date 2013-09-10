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
    public class AttributeInfo
    {
        public enum AttributeValidityEnum
        {
            // This is a valid attribute, but won't be used as an option for setOptions
            NoOption,

            // Will be used as an option for setOptions. Optional.
            OptionalOption
        }

        public string Name { get; private set; }
        public IValueInfo ValueInfo { get; private set; }
        public AttributeValidityEnum AttributeValidity { get; private set; }

        public AttributeInfo(string name, IValueInfo valueInfo, AttributeValidityEnum attributeValidity)
        {
            Name = name;
            ValueInfo = valueInfo ?? new StringValue();
            AttributeValidity = attributeValidity;
        }
    }
}
