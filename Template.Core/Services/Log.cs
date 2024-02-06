namespace Template.Services;

using Rester;

internal static partial class Log
{
    [LoggerMessage(Level = LogLevel.Warning, Message = "Sample get failed. result=[{restResult}], statusCode=[{statusCode}]")]
    public static partial void InfoConnectorGetFailed(this ILogger logger, RestResult restResult, HttpStatusCode statusCode);
}
