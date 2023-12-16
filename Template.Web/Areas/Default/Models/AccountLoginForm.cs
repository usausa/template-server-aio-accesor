namespace Template.Web.Areas.Default.Models;

public sealed class AccountLoginForm
{
    [Required(ErrorMessage = Messages.Required)]
    public string Id { get; set; } = default!;

    [Required(ErrorMessage = Messages.Required)]
    public string Password { get; set; } = default!;
}
