namespace Template.Web.Areas.Api;

using Template.Web.Infrastructure.Token;

[Area("api")]
[Route("[area]/[controller]/[action]")]
[ApiController]
[TokenFilter]
public class BaseApiController : Controller
{
}
