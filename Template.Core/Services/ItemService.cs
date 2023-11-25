namespace Template.Services;

using Template.Accessors;

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

    public ValueTask UpdateItemList(IEnumerable<ItemEntity> entities) =>
        DbProvider.UsingTxAsync(async (_, tx) =>
        {
            foreach (var entity in entities)
            {
                await ItemAccessor.UpdateItem(tx, entity).ConfigureAwait(false);
            }

            await tx.CommitAsync().ConfigureAwait(false);
        });
}
