namespace Template.Accessors;

[DataAccessor]
public interface IDataAccessor
{
    [Query]
    ValueTask<List<DataEntity>> QueryDataListAsync(bool? flag, int limit, int offset);

    [ExecuteScalar]
    ValueTask<int> CountDataAsync(bool? flag);
}
