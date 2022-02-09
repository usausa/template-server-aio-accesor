namespace Template.Web.Infrastructure.Mvc;

public sealed class AreaControllerRouteAttribute : RouteAttribute
{
    public AreaControllerRouteAttribute()
        : base("~/[area]/[controller]")
    {
    }
}
