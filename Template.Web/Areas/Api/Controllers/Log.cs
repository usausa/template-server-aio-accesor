namespace Template.Web.Areas.Api.Controllers;

#pragma warning disable SYSLIB1006
public static partial class Log
{
    [LoggerMessage(Level = LogLevel.Information, Message = "Get. path=[{path}]")]
    public static partial void InfoStorageGet(this ILogger logger, string? path);

    [LoggerMessage(Level = LogLevel.Information, Message = "Post. path=[{path}]")]
    public static partial void InfoStoragePost(this ILogger logger, string? path);

    [LoggerMessage(Level = LogLevel.Information, Message = "Delete. path=[{path}]")]
    public static partial void InfoStorageDelete(this ILogger logger, string? path);

    [LoggerMessage(Level = LogLevel.Warning, Message = "No found. path=[{path}]")]
    public static partial void WarnStorageNotFound(this ILogger logger, string? path);
}
#pragma warning restore SYSLIB1006
