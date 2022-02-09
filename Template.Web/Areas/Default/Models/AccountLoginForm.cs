namespace Template.Web.Areas.Default.Models;

using System.ComponentModel.DataAnnotations;

public class AccountLoginForm
{
    [Required(ErrorMessage = Messages.Required)]
    [AllowNull]
    public string Id { get; set; }

    [Required(ErrorMessage = Messages.Required)]
    [AllowNull]
    public string Password { get; set; }
}
