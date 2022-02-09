namespace Template.Web.Areas.Api.Controllers;

using System;

using Template.Web.Areas.Api.Models;

public class TerminalController : BaseApiController
{
    [HttpGet]
    public IActionResult Time()
    {
        return Ok(new TerminalTimeResponse { DateTime = DateTime.Now });
    }
}
