public interface IRedisStorage<TModel>
{
    Task StoreDataAsync(string key, TModel data, TimeSpan? expiry = null);
    Task<TModel> RetrieveDataAsync(string key);
}