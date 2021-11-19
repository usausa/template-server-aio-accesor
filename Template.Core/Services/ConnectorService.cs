namespace Template.Services
{
    using System.Net.Http;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using Rester;

    using Template.Models.Connector;

    public class ConnectorService
    {
        private ILogger<ConnectorService> Log { get; }

        private IHttpClientFactory HttpClientFactory { get; }

        public ConnectorService(
            ILogger<ConnectorService> log,
            IHttpClientFactory httpClientFactory)
        {
            Log = log;
            HttpClientFactory = httpClientFactory;
        }

        public async ValueTask<SampleResponse?> GetSampleAsync()
        {
            using var client = HttpClientFactory.CreateClient(ConnectorNames.Sample);
            var result = await client.GetAsync<SampleResponse>("?format=json").ConfigureAwait(false);
            if (result.Content is null)
            {
                Log.LogWarning("Sample get failed. result=[{RestResult}], statusCode=[{StatusCode}]", result.RestResult, result.StatusCode);
            }

            return result.Content;
        }
    }
}
