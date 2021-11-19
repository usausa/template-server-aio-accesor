namespace Template.Web.Areas.Default.Controllers;

using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Template.Web.Areas.Default.Models;
using Template.Web.Authentication;

[AllowAnonymous]
public class AccountController : BaseDefaultController
{
    private AccountManager AccountManager { get; }

    public AccountController(AccountManager accountManager)
    {
        AccountManager = accountManager;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async ValueTask<IActionResult> Login([FromForm] AccountLoginForm form, string? returnUrl)
    {
        if (ModelState.IsValid)
        {
            if (await AccountManager.LoginAsync(form.Id, form.Password))
            {
                return Redirect(String.IsNullOrEmpty(returnUrl) ? "~/" : returnUrl);
            }

            ModelState.AddModelError(nameof(form.Password), Messages.LoginFailed);
        }

        return View(form);
    }

    [HttpGet]
    public async ValueTask<IActionResult> Logout()
    {
        await AccountManager.LogoutAsync();

        return LocalRedirect("~/");
    }
}
