namespace Template.Web.Settings;

public class ServerSetting
{
    public string ApiToken { get; set; } = default!;

    public int LongTimeThreshold { get; set; }

    public string[]? AllowHealth { get; set; }

    public string[]? AllowMetrics { get; set; }
}
