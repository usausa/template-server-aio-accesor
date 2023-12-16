namespace Template.Web.Areas.Api.Controllers;

using Template.Components.Storage;

[SuppressMessage("Security", "CA5391", Justification = "API Controller")]
[Area("api")]
[Route("[area]/[controller]")]
[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
public class StorageController : ControllerBase
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
        Log.InfoStorageGet(path);

        if (path!.EndsWith('/'))
        {
            if (!await Storage.DirectoryExistsAsync(path))
            {
                Log.WarnStorageNotFound(path);

                return NotFound();
            }

            var files = await Storage.ListAsync(path);
            return Ok(files);
        }

        if (!await Storage.FileExistsAsync(path))
        {
            Log.WarnStorageNotFound(path);

            return NotFound();
        }

        var stream = await Storage.ReadAsync(path);
        var index = path.LastIndexOf('/');
        return File(stream, ContextType, index >= 0 ? path[(index + 1)..] : path);
    }

    [HttpPost("{**path}")]
    [ReadableBodyStream]
    public async ValueTask<IActionResult> Post([FromRoute] string path)
    {
        Log.InfoStoragePost(path);

        await Storage.WriteAsync(path, Request.Body);

        return Ok();
    }

    [HttpDelete("{**path}")]
    public async ValueTask<IActionResult> Delete([FromRoute] string path)
    {
        Log.InfoStorageDelete(path);

        await Storage.DeleteAsync(path);

        return Ok();
    }
}
