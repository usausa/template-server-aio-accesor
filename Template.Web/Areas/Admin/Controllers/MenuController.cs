namespace Template.Web.Areas.Admin.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    using Template.Web.Infrastructure.Mvc;

    public class MenuController : BaseAdminController
    {
        [AreaControllerRoute]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}
