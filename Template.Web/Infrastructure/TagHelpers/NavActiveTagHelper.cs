namespace Template.Web.Infrastructure.TagHelpers
{
    using System.Diagnostics.CodeAnalysis;

    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using Microsoft.AspNetCore.Razor.TagHelpers;

    [HtmlTargetElement(Attributes = AttributeName)]
    public sealed class NavActiveTagHelper : TagHelper
    {
        private const string AttributeName = "nav-active";

        [HtmlAttributeName(AttributeName)]
        [AllowNull]
        public string Path { get; set; }

        [HtmlAttributeNotBound]
        [ViewContext]
        [AllowNull]
        public ViewContext ViewContext { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var a = ViewContext.HttpContext.Request.Path;
        }
    }
}
