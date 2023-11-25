namespace Template.Web.Areas.Example.Controllers;

public class LogController : BaseExampleController
{
    [HttpGet]
    public async ValueTask<IActionResult> Slow()
    {
        await Task.Delay(10_000);

        return View();
    }

    [HttpGet]
    public IActionResult Error() => throw new InvalidOperationException("Cause exception.");
}
