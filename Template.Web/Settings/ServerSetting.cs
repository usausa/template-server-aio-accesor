namespace Template.Web.Settings;

using System.Diagnostics.CodeAnalysis;

public class ServerSetting
{
    [AllowNull]
    public string ApiToken { get; set; }

    public int LongTimeThreshold { get; set; }

    public string[]? AllowHealth { get; set; }

    public string[]? AllowMetrics { get; set; }
}
