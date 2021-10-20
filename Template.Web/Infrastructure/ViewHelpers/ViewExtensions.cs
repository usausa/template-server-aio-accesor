namespace Template.Web.Infrastructure.ViewHelpers
{
    using System;
    using System.Globalization;

    public static class ViewExtensions
    {
        //--------------------------------------------------------------------------------
        // Basic
        //--------------------------------------------------------------------------------

        public static string Then(this bool condition, string value) => condition ? value : string.Empty;

        public static string NotThen(this bool condition, string value) => !condition ? value : string.Empty;

        //--------------------------------------------------------------------------------
        // Format
        //--------------------------------------------------------------------------------

        public static string Date(this DateTime? value) => value?.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture) ?? string.Empty;

        public static string Date(this DateTime value) => value.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);

        public static string DateTime(this DateTime? value) => value?.ToString("yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture) ?? string.Empty;

        public static string DateTime(this DateTime value) => value.ToString("yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture);
    }
}
