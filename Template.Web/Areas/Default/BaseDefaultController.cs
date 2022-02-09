namespace Template.Web.Areas.Default;

using Microsoft.AspNetCore.Authorization;

[Area("default")]
[Route("[controller]/[action]")]
[Authorize]
[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true, Duration = 0)]
[ApiExplorerSettings(IgnoreApi = true)]
public abstract class BaseDefaultController : Controller
{
}
