namespace Template.Accessors;

[DataAccessor]
public interface IAccountAccessor
{
    [QueryFirstOrDefault]
    ValueTask<AccountEntity?> QueryAccountAsync(string id);
}
