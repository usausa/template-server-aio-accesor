namespace Template.Web.Authentication;

using System.Security.Claims;

using Microsoft.AspNetCore.Authentication.Cookies;

using Template.Components.Security;

public sealed class AccountManager
{
    private IHttpContextAccessor HttpContextAccessor { get; }

    private IPasswordProvider PasswordProvider { get; }

    private AccountService AccountService { get; }

    public AccountManager(
        IHttpContextAccessor httpContextAccessor,
        IPasswordProvider passwordProvider,
        AccountService accountService)
    {
        HttpContextAccessor = httpContextAccessor;
        PasswordProvider = passwordProvider;
        AccountService = accountService;
    }

    public async ValueTask<bool> LoginAsync(string id, string password)
    {
        var account = await AccountService.QueryAccountAsync(id);
        if ((account is null) || !PasswordProvider.Match(password, Convert.FromBase64String(account.PasswordHash)))
        {
            return false;
        }

        var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
        identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, id));
        identity.AddClaim(new Claim(ClaimTypes.Name, account.Name));
        if (account.IsAdmin)
        {
            identity.AddClaim(new Claim(ClaimTypes.Role, Role.Admin));
        }

        await HttpContextAccessor.HttpContext!.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            new AuthenticationProperties { AllowRefresh = true, IsPersistent = true });

        return true;
    }

    public async ValueTask LogoutAsync() =>
        await HttpContextAccessor.HttpContext!.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
}
