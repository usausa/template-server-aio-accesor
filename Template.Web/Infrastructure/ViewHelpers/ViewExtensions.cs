namespace Template.Web.Infrastructure.ViewHelpers;

using System;

public static class ViewExtensions
{
    //--------------------------------------------------------------------------------
    // Expression
    //--------------------------------------------------------------------------------

    public static string Then(this bool condition, string value) => condition ? value : string.Empty;

    public static string NotThen(this bool condition, string value) => !condition ? value : string.Empty;

    //--------------------------------------------------------------------------------
    // Basic
    //--------------------------------------------------------------------------------

    public static string Active(this bool value) => value ? "active" : string.Empty;

    //--------------------------------------------------------------------------------
    // Format
    //--------------------------------------------------------------------------------

    public static string Date(this DateTime? value) => value?.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture) ?? string.Empty;

    public static string Date(this DateTime value) => value.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);

    public static string DateTime(this DateTime? value) => value?.ToString("yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture) ?? string.Empty;

    public static string DateTime(this DateTime value) => value.ToString("yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture);
}
