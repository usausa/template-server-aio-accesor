namespace Template.Accessors;

[DataAccessor]
public interface IItemAccessor
{
    [Query]
    ValueTask<List<ItemEntity>> QueryItemListAsync(string category);

    [Execute]
    ValueTask<int> UpdateItem(DbTransaction tx, ItemEntity entity);
}
