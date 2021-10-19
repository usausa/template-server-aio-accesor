namespace Template.Web.Areas.Example
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Area("example")]
    [Route("[area]/[controller]/[action]")]
    [Authorize]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true, Duration = 0)]
    [ApiExplorerSettings(IgnoreApi = true)]
    public abstract class BaseExampleController : Controller
    {
    }
}
