namespace Template.Web.Areas.Default.Controllers;

using System.Diagnostics;

using Template.Web.Areas.Default.Models;

[AllowAnonymous]
[Area("default")]
[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true, Duration = 0)]
[ApiExplorerSettings(IgnoreApi = true)]
public class ErrorController : Controller
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA5395", Justification = "Ignore")]
    [Route("~/[controller]/{statusCode:int}")]
    public IActionResult Index(int statusCode)
    {
        return View(new ErrorViewModel
        {
            StatusCode = statusCode,
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}
