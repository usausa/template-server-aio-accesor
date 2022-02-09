namespace Template.Web.Areas.Api.Controllers;

using System;

using Microsoft.AspNetCore.Mvc;

using Template.Components.Storage;
using Template.Web.Infrastructure.Filters;

using Template.Web.Infrastructure.Token;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA5391", Justification = "API Controller")]
[Area("api")]
[Route("[area]/[controller]")]
[ApiController]
[TokenFilter]
[ApiExplorerSettings(IgnoreApi = true)]
public class StorageController : Controller
{
    private const string ContextType = "application/octet-stream";

    private ILogger<StorageController> Log { get; }

    private IStorage Storage { get; }

    public StorageController(
        ILogger<StorageController> log,
        IStorage storage)
    {
        Log = log;
        Storage = storage;
    }

    [HttpGet("{**path}")]
    public async ValueTask<IActionResult> Get([FromRoute] string? path = "/")
    {
        Log.LogInformation("Get. path=[{Path}]", path);

        if (path!.EndsWith('/'))
        {
            if (!await Storage.DirectoryExistsAsync(path))
            {
                Log.LogWarning("Get not found. path=[{Path}]", path);

                return NotFound();
            }

            var files = await Storage.ListAsync(path);
            return Ok(files);
        }

        if (!await Storage.FileExistsAsync(path))
        {
            Log.LogWarning("Get not found. path=[{Path}]", path);

            return NotFound();
        }

        var stream = await Storage.ReadAsync(path);
        var index = path.LastIndexOf("/", StringComparison.OrdinalIgnoreCase);
        return File(stream, ContextType, index >= 0 ? path[(index + 1)..] : path);
    }

    [HttpPost("{**path}")]
    [ReadableBodyStream]
    public async ValueTask<IActionResult> Post([FromRoute] string path)
    {
        Log.LogInformation("Post. path=[{Path}]", path);

        await Storage.WriteAsync(path, Request.Body);

        return Ok();
    }

    [HttpDelete("{**path}")]
    public async ValueTask<IActionResult> Delete([FromRoute] string path)
    {
        Log.LogInformation("Delete. path=[{Path}]", path);

        await Storage.DeleteAsync(path);

        return Ok();
    }
}
