namespace Template.Web.Areas.Default.Controllers;

public class DashboardController : BaseDefaultController
{
    [AllowAnonymous]
    [DefaultRoute]
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }
}
