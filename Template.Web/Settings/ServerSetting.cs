namespace Template.Web.Settings;

public class ServerSetting
{
    [AllowNull]
    public string ApiToken { get; set; }

    public int LongTimeThreshold { get; set; }

    public string[]? AllowHealth { get; set; }

    public string[]? AllowMetrics { get; set; }
}
