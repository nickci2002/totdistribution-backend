namespace TOTDBackend.NadeoRefinery.Common.Features;

internal interface IRedisRepositoryComponent<TData>
    where TData : notnull
{
    Task StoreDataAsync(TData data, TimeSpan? expiry = null);
    TData RetrieveData(string key);
}