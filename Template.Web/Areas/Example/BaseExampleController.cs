namespace Template.Web.Areas.Example;

[Area("example")]
[Route("[area]/[controller]/[action]")]
[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true, Duration = 0)]
[ApiExplorerSettings(IgnoreApi = true)]
public abstract class BaseExampleController : Controller
{
}
