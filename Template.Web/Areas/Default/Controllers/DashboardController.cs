namespace Template.Web.Areas.Default.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using Template.Web.Infrastructure.Mvc;

    [AllowAnonymous]
    public class DashboardController : BaseDefaultController
    {
        [DefaultRoute]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}