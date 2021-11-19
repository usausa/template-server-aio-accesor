namespace Template.Web.Infrastructure.TagHelpers;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;

using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

[HtmlTargetElement("app-message")]
public sealed class MessageTagHelper : TagHelper
{
    [HtmlAttributeNotBound]
    [ViewContext]
    [AllowNull]
    public ViewContext ViewContext { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = string.Empty;

        if (!ViewContext.TempData.TryGetValue("Message", out var message))
        {
            return;
        }

        var builder = new StringBuilder();
        builder.Append("<div class=\"alert alert-success\" role=\"alert\">");
        builder.Append(((string)message!).Replace("\n", "<br />", StringComparison.InvariantCulture));
        builder.Append("<a href=\"#\" class=\"close\" data-dismiss=\"alert\"><span>&times;</span></a>");
        builder.Append("</div>");
        output.Content.SetHtmlContent(builder.ToString());
    }
}
