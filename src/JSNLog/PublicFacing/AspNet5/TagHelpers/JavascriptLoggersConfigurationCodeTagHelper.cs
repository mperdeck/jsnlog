#if !NET40

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Razor.TagHelpers;
using Microsoft.AspNet.Http;

namespace JSNLog
{
    public class JlJavascriptLoggerDefinitionsTagHelper : TagHelper
    {
        // Can be passed via <jl-javascript-logger-definitions request-id="..." />. 
        // Pascal case gets translated into lower-kebab-case.
        public string RequestId { get; set; }

        private readonly IHttpContextAccessor _httpContextAccessor;

        public JlJavascriptLoggerDefinitionsTagHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = ""; // Remove the jl-javascript-logger-definitions tag completely

            HttpContext httpContext = _httpContextAccessor.HttpContext;
            string JSCode = httpContext.Configure(RequestId);

            output.Content.SetHtmlContent(JSCode);

            output.TagMode = TagMode.StartTagAndEndTag;
        }
    }
}

#endif
