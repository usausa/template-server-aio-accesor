namespace Template.Web.Authentication;

using System.Security.Claims;

public static class UserExtensions
{
    public static bool IsAuthenticated(this ClaimsPrincipal user) => user.Identity?.IsAuthenticated ?? false;

    public static bool IsAdmin(this ClaimsPrincipal user) => user.IsInRole(Role.Admin);
}
