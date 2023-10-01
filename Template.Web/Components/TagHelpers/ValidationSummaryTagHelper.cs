namespace Template.Web.Components.TagHelpers;

using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

[HtmlTargetElement("app-validation-summary")]
public sealed class ValidationSummaryTagHelper : TagHelper
{
    [HtmlAttributeNotBound]
    [ViewContext]
    public ViewContext ViewContext { get; set; } = default!;

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        if (ViewContext.ViewData.ModelState.ErrorCount == 0)
        {
            output.TagName = string.Empty;
            return;
        }

        output.TagName = "div";
        output.TagMode = TagMode.StartTagAndEndTag;
        output.Attributes.Add("class", "validation-summary-errors alert alert-danger");

        var builder = new StringBuilder();
        builder.AppendLine("<ul class=\"error-list\">");
        foreach (var error in ViewContext.ViewData.ModelState.SelectMany(x => x.Value!.Errors))
        {
            builder.AppendLine(
                "<li class=\"error-list-item\">" +
                "<span class=\"text-danger\"><i class=\"fas fa-exclamation-triangle mr-2\"></i>" +
                $"{error.ErrorMessage}" +
                "</span>" +
                "</li>");
        }

        builder.AppendLine("</ul>");
        output.Content.SetHtmlContent(builder.ToString());
    }
}
