namespace Template.Web.Areas.Example.Controllers;

using System;

using Microsoft.AspNetCore.Mvc;

public class LogController : BaseExampleController
{
    [HttpGet]
    public async ValueTask<IActionResult> Slow()
    {
        await Task.Delay(10_000);

        return View();
    }

    [HttpGet]
    public IActionResult Exception()
    {
        throw new InvalidOperationException("Cause exception.");
    }
}
