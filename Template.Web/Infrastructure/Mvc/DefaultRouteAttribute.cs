namespace Template.Web.Infrastructure.Mvc;

public sealed class DefaultRouteAttribute : RouteAttribute
{
    public DefaultRouteAttribute()
        : base("~/")
    {
    }
}
