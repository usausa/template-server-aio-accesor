namespace Template.Services
{
    using System.Threading.Tasks;

    using Smart.Data.Accessor;

    using Template.Accessors;
    using Template.Models.Entity;

    public class AccountService
    {
        private IAccountAccessor AccountAccessor { get; }

        public AccountService(
            IAccessorResolver<IAccountAccessor> accountAccessor)
        {
            AccountAccessor = accountAccessor.Accessor;
        }

        public ValueTask<AccountEntity> QueryAccountAsync(string id) =>
            AccountAccessor.QueryAccountAsync(id);
    }
}
