namespace Template.Web.Infrastructure.ViewHelpers;

public static class ViewHelper
{
    public static string Status(bool value) => value ? "○" : "×";
}
