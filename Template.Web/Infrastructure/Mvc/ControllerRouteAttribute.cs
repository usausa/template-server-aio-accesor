namespace Template.Web.Infrastructure.Mvc;

public sealed class ControllerRouteAttribute : RouteAttribute
{
    public ControllerRouteAttribute()
        : base("~/[controller]")
    {
    }
}
