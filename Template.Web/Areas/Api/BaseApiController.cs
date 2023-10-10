namespace Template.Web.Areas.Api;

[Area("api")]
[Route("[area]/[controller]/[action]")]
[ApiController]
[ApiExceptionFilter]
//[TokenFilter]
public class BaseApiController : ControllerBase
{
}
