namespace Template.Web.Areas.Admin.Controllers;

public class MenuController : BaseAdminController
{
    [AreaControllerRoute]
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }
}
