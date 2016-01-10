using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml.Serialization;

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
