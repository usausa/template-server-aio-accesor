namespace Template.Web.Areas.Admin
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Area("admin")]
    [Route("[area]/[controller]/[action]")]
    [Authorize]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true, Duration = 0)]
    [ApiExplorerSettings(IgnoreApi = true)]
    public abstract class BaseAdminController : Controller
    {
    }
}
