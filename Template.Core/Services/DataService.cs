namespace Template.Services;

using Template.Accessors;

public sealed class DataSearchParameter : Pageable
{
    public bool? Flag { get; set; }

    public string? Name { get; set; }

    public DateTime? DateTimeFrom { get; set; }

    public DateTime? DateTimeTo { get; set; }
}

public sealed class DataService
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
