namespace Template.Web.Areas.Api.Controllers;

using Template.Web.Areas.Api.Models;

public sealed class TerminalController : BaseApiController
{
    [HttpGet]
    public IActionResult Time()
    {
        return Ok(new TerminalTimeResponse { DateTime = DateTime.Now });
    }
}
