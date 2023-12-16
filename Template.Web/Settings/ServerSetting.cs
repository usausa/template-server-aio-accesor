namespace Template.Web.Settings;

public sealed class ServerSetting
{
    public string ApiToken { get; set; } = default!;

    public int LongTimeThreshold { get; set; }
}
