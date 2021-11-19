namespace Template.Services;

using System.Collections.Generic;
using System.Threading.Tasks;

using Smart.Data;
using Smart.Data.Accessor;

using Template.Accessors;
using Template.Models.Entity;

public class ItemService
{
    private IDbProvider DbProvider { get; }

    private IItemAccessor ItemAccessor { get; }

    public ItemService(
        IDbProvider dbProvider,
        IAccessorResolver<IItemAccessor> dataAccessor)
    {
        DbProvider = dbProvider;
        ItemAccessor = dataAccessor.Accessor;
    }

    public ValueTask<List<ItemEntity>> QueryItemListAsync(string category) =>
        ItemAccessor.QueryItemListAsync(category);

    public async ValueTask UpdateItemList(IEnumerable<ItemEntity> entities)
    {
        await DbProvider.UsingTxAsync(async (_, tx) =>
        {
            foreach (var entity in entities)
            {
                await ItemAccessor.UpdateItem(tx, entity).ConfigureAwait(false);
            }

            await tx.CommitAsync().ConfigureAwait(false);
        }).ConfigureAwait(false);
    }
}
