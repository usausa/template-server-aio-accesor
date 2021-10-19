namespace Template.Accessors
{
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Threading.Tasks;

    using Template.Models.Entity;

    using Smart.Data.Accessor.Attributes;

    [DataAccessor]
    public interface IItemAccessor
    {
        [Query]
        ValueTask<List<ItemEntity>> QueryItemListAsync(string category);

        [Execute]
        ValueTask<int> UpdateItem(DbTransaction tx, ItemEntity entity);
    }
}
