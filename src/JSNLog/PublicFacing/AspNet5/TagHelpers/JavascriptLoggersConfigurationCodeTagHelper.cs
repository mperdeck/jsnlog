#if !NET40

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Razor.TagHelpers;

namespace JSNLog
{
    [HtmlTargetElement("jl-javascript-logging-configuration-code", Attributes = RequestIdAttributeName)]
    public class JavascriptLoggersConfigurationCodeTagHelper : TagHelper
    {
        private const string RequestIdAttributeName = "requestid";

        [HtmlAttributeName(RequestIdAttributeName)]
        public string RequestId { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (output == null)
            {
                throw new ArgumentNullException("output");
            }

            output.Content.SetContent("xxxxxxxxxxxxxxx");
        }
    }
}

#endif
