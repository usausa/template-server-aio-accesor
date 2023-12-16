namespace Template.Services;

using Template.Accessors;

public sealed class AccountService
{
    private IAccountAccessor AccountAccessor { get; }

    public AccountService(
        IAccessorResolver<IAccountAccessor> accountAccessor)
    {
        AccountAccessor = accountAccessor.Accessor;
    }

    public ValueTask<AccountEntity?> QueryAccountAsync(string id) =>
        AccountAccessor.QueryAccountAsync(id);
}
