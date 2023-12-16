namespace Template.Web.Areas.Admin.Controllers;

public sealed class MenuController : BaseAdminController
{
    [AreaControllerRoute]
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }
}
