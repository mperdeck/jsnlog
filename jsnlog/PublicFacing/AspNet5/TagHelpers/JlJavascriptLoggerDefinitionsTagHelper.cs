using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace JSNLog
{
    // HtmlTargetElementAttribute is needed because this tag helper will be used in other projects
    [HtmlTargetElement("jl-javascript-logger-definitions")]
    public class JlJavascriptLoggerDefinitionsTagHelper : TagHelper
    {
        // Can be passed via <jl-javascript-logger-definitions request-id="..." />. 
        // Pascal case gets translated into lower-kebab-case.
        public string RequestId { get; set; }

        // See https://www.jerriepelser.com/blog/accessing-request-object-inside-tag-helper-aspnet-core/

        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            return base.ProcessAsync(context, output);
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = ""; // Remove the jl-javascript-logger-definitions tag completely

            HttpContext httpContext = ViewContext.HttpContext;
            string JSCode = httpContext.Configure(RequestId);

            output.Content.SetHtmlContent(JSCode);

            output.TagMode = TagMode.StartTagAndEndTag;
        }
    }
}
