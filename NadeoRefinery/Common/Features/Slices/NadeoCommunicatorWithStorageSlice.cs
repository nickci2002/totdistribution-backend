namespace TOTDBackend.NadeoRefinery.Common.Features;

/// <inheritdoc cref="NadeoCommunicatorSlice{TResp}>" />
internal abstract class NadeoCommunicatorWithStorageSlice<TResp>
    : NadeoCommunicatorSlice<TResp>, ISliceStorable<TResp>
    where TResp : notnull
{
    protected abstract IRedisRepositoryComponent<TResp> RepositoryComponent { get; }

    public virtual async Task HandleStorageAsync(TResp data, TimeSpan? expiry = null)
    {
        ArgumentNullException.ThrowIfNull(RepositoryComponent);
        
        await RepositoryComponent.StoreDataAsync(data, expiry);
    }
    
    public virtual TResp HandleRetrieval(string key)
    {
        ArgumentNullException.ThrowIfNull(RepositoryComponent);

        return RepositoryComponent.RetrieveData(key);
    }

    public virtual async Task<TResp> HandleConsumeAndStorageAsync(TimeSpan? expiry = null)
    {
        var response = await HandleConsumeAsync();
        await HandleStorageAsync(response);

        return response;
    }
}

/// <inheritdoc cref="NadeoCommunicatorSlice{TReq, TResp}>" />
internal abstract class NadeoCommunicatorWithStorageSlice<TReq, TResp>
    : NadeoCommunicatorSlice<TReq, TResp>, ISliceStorable<TResp>
    where TReq : notnull
    where TResp : notnull
{
    protected abstract IRedisRepositoryComponent<TResp> RepositoryComponent { get; }

    public virtual async Task HandleStorageAsync(TResp data, TimeSpan? expiry = null)
    {
        ArgumentNullException.ThrowIfNull(RepositoryComponent);
        
        await RepositoryComponent.StoreDataAsync(data, expiry);
    }
    
    public virtual TResp HandleRetrieval(string key)
    {
        ArgumentNullException.ThrowIfNull(RepositoryComponent);

        return RepositoryComponent.RetrieveData(key);
    }

    public virtual async Task<TResp> HandleConsumeAndStorageAsync(TReq request, TimeSpan? expiry = null)
    {
        var response = await HandleConsumeAsync(request);
        await HandleStorageAsync(response);

        return response;
    }
}