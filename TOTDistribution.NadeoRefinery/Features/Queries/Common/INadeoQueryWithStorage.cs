public interface INadeoQueryWithStorage<TModel> : INadeoQuery<TModel>
{
    Task StoreDataAsync(TModel data, TimeSpan? expiry = null);
    Task<TModel> RetrieveDataAsync(string key);
}