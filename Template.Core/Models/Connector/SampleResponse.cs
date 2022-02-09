namespace Template.Models.Connector;

using System.Text.Json.Serialization;

public class SampleResponse
{
    [JsonPropertyName("ip")]
    [AllowNull]
    public string Ip { get; set; }
}
