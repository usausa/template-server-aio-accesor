namespace Template.Web.Areas.Api.Controllers;

public class TestController : BaseApiController
{
    [HttpGet]
    public IActionResult Error() => throw new InvalidOperationException("Cause exception.");
}
