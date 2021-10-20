namespace Template.Models.Connector
{
    using System.Diagnostics.CodeAnalysis;
    using System.Text.Json.Serialization;

    public class SampleResponse
    {
        [JsonPropertyName("ip")]
        [AllowNull]
        public string Ip { get; set; }
    }
}
