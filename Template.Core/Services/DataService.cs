namespace Template.Services;

using System;
using System.Threading.Tasks;

using Template.Accessors;
using Template.Models.Entity;
using Template.Models.Paging;

using Smart.Data.Accessor;

public class DataSearchParameter : Pageable
{
    public bool? Flag { get; set; }

    public string? Name { get; set; }

    public DateTime? DateTimeFrom { get; set; }

    public DateTime? DateTimeTo { get; set; }
}

public class DataService
{
    private IDataAccessor DataAccessor { get; }

    public DataService(
        IAccessorResolver<IDataAccessor> dataAccessor)
    {
        DataAccessor = dataAccessor.Accessor;
    }

    public async ValueTask<Paged<DataEntity>> QueryAccountPagedAsync(DataSearchParameter parameter)
    {
        var list = await DataAccessor.QueryDataListAsync(parameter.Flag, parameter.Size, parameter.Offset).ConfigureAwait(false);
        var count = await DataAccessor.CountDataAsync(parameter.Flag).ConfigureAwait(false);
        return new Paged<DataEntity>(parameter, list, count);
    }
}
