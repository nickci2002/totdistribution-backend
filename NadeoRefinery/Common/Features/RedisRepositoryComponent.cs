using Redis.OM;
using Redis.OM.Searching;

namespace TOTDBackend.NadeoRefinery.Common.Features;

internal interface IRedisRepositoryComponent<T>
{
    Task StoreDataAsync(T data, TimeSpan? expiry = null);
    Task<T> RetrieveDataAsync(string key);
}

/// <inheritdoc cref="IRedisRepositoryComponent{T}">
internal abstract class RedisRepositoryComponent<T>(RedisConnectionProvider provider)
    : IRedisRepositoryComponent<T>
    where T : notnull
{
    protected readonly RedisCollection<T> _collection =
        (RedisCollection<T>)provider.RedisCollection<T>();

    public abstract Task<T> RetrieveDataAsync(string key);
    public abstract Task StoreDataAsync(T data, TimeSpan? expiry = null);
}