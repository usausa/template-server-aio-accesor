namespace Template.Models.Entity;

public sealed class AccountEntity
{
    public string Id { get; set; } = default!;

    public string Name { get; set; } = default!;

    public string PasswordHash { get; set; } = default!;

    public bool IsAdmin { get; set; }
}
