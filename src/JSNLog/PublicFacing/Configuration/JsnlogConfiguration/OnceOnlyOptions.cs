
#if !DNXCORE50
using System.Xml.Serialization;
#endif

namespace JSNLog
{
    public class OnceOnlyOptions
    {
#if !DNXCORE50
        [XmlAttribute]
#endif
        public string regex { get; set; }
    }
}
