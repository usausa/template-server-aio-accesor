namespace Template.Web.Areas.Example.Controllers;

public sealed class ConnectorController : BaseExampleController
{
    private ConnectorService ConnectorService { get; }

    public ConnectorController(ConnectorService connectorService)
    {
        ConnectorService = connectorService;
    }

    [AreaControllerRoute]
    [HttpGet]
    public async ValueTask<IActionResult> Index()
    {
        var response = await ConnectorService.GetSampleAsync();
        if (response is not null)
        {
            ViewBag.Entity = response;
        }

        return View();
    }
}
