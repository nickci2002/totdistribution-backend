namespace TOTDistribution.NadeoRefinery.Common.Features;

internal interface IRedisRepository<T>
{
    abstract Task StoreDataAsync(T data, TimeSpan? expiry = null);
    abstract Task<T> RetrieveDataAsync(string key);
}