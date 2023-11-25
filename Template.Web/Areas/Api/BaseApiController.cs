namespace Template.Web.Areas.Api;

[Area("api")]
[Route("[area]/[controller]/[action]")]
[ApiController]
//[TokenFilter]
public class BaseApiController : ControllerBase
{
}
