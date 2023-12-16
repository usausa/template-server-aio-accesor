namespace Template.Models.Connector;

using System.Text.Json.Serialization;

public sealed class SampleResponse
{
    [JsonPropertyName("ip")]
    public string Ip { get; set; } = default!;
}
