namespace Template.Web.Areas.Default.Controllers;

public sealed class DashboardController : BaseDefaultController
{
    [AllowAnonymous]
    [DefaultRoute]
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }
}
