namespace TOTDBackend.NadeoRefinery.Common.Features;

/// <inheritdoc cref="NadeoSlice{TResp}>" />
internal abstract class NadeoSliceWithStorage<TResp>
    : NadeoSlice<TResp>, ISliceStorable<TResp>
    where TResp : notnull
{
    protected abstract IRedisRepositoryComponent<TResp> RepositoryComponent { get; }

    public async Task HandleStorageAsync(TResp data, TimeSpan? expiry = null)
    {
        ArgumentNullException.ThrowIfNull(RepositoryComponent);
        
        await RepositoryComponent.StoreDataAsync(data, expiry);
    }
    
    public TResp HandleRetrieval(string key)
    {
        ArgumentNullException.ThrowIfNull(RepositoryComponent);

        return RepositoryComponent.RetrieveData(key);
    }

    public async Task<TResp> HandleConsumeAndStorageAsync()
    {
        var response = await HandleConsumeAsync();
        await HandleStorageAsync(response);

        return response;
    }
}

/// <inheritdoc cref="NadeoSlice{TReq, TResp}>" />
internal abstract class NadeoSliceWithStorage<TReq, TResp>
    : NadeoSlice<TReq, TResp>, ISliceStorable<TResp>
    where TReq : notnull
    where TResp : notnull
{
    protected abstract IRedisRepositoryComponent<TResp> RepositoryComponent { get; }

    public async Task HandleStorageAsync(TResp data, TimeSpan? expiry = null)
    {
        ArgumentNullException.ThrowIfNull(RepositoryComponent);
        
        await RepositoryComponent.StoreDataAsync(data, expiry);
    }
    
    public TResp HandleRetrieval(string key)
    {
        ArgumentNullException.ThrowIfNull(RepositoryComponent);

        return RepositoryComponent.RetrieveData(key);
    }

    public async Task<TResp> HandleConsumeAndStorageAsync(TReq request)
    {
        var response = await HandleConsumeAsync(request);
        await HandleStorageAsync(response);

        return response;
    }
}