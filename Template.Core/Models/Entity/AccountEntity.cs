namespace Template.Models.Entity;

public class AccountEntity
{
    [AllowNull]
    public string Id { get; set; }

    [AllowNull]
    public string Name { get; set; }

    [AllowNull]
    public string PasswordHash { get; set; }

    public bool IsAdmin { get; set; }
}
