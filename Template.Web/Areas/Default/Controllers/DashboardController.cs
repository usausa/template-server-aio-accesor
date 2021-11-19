namespace Template.Web.Areas.Default.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Template.Web.Infrastructure.Mvc;

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
