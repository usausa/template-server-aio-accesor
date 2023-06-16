namespace Template.Services;

using Rester;

#pragma warning disable SYSLIB1006
public static partial class Log
{
    [LoggerMessage(Level = LogLevel.Warning, Message = "Sample get failed. result=[{restResult}], statusCode=[{statusCode}]")]
    public static partial void InfoConnectorGetFailed(this ILogger logger, RestResult restResult, HttpStatusCode statusCode);
}
#pragma warning restore SYSLIB1006
