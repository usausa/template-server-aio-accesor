namespace Template.Accessors
{
    using System.Threading.Tasks;

    using Template.Models.Entity;
    using Smart.Data.Accessor.Attributes;

    [DataAccessor]
    public interface IAccountAccessor
    {
        [QueryFirstOrDefault]
        ValueTask<AccountEntity?> QueryAccountAsync(string id);
    }
}
