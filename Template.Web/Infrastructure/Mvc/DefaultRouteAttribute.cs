namespace Template.Web.Infrastructure.Mvc
{
    using Microsoft.AspNetCore.Mvc;

    public sealed class DefaultRouteAttribute : RouteAttribute
    {
        public DefaultRouteAttribute()
            : base("~/")
        {
        }
    }
}
