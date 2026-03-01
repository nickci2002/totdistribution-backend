namespace TOTDBackend.NadeoRefinery.Common.Features;

/// <summary>
/// A vertical slice that takes no input and retrieves query data for processing.<br/>
/// ALL DERIVED CLASSES SHOULD USE THE 'internal' KEYWORD!!!
/// </summary>
/// <typeparam name="TResp">The return type</typeparam>
internal interface INadeoSlice<TResp>
{
    Task<TResp> HandleAsync();
}

/// <inheritdoc cref="INadeoSlice{TResp}>" />
internal abstract class NadeoSlice<TResp>
    : INadeoSlice<TResp>
    where TResp : notnull
{
    protected abstract INadeoConsumerComponent<TResp> ConsumerComponent { get; }
    protected abstract RedisRepositoryComponent<TResp>? RepositoryComponent { get; }

    public async Task<TResp> HandleAsync()
    {
        if (ConsumerComponent is null)
        {
            throw new ArgumentNullException("The Consumer Component cannot be null");
        }

        var response = await ConsumerComponent.FetchData();
        
        if (RepositoryComponent is not null)
        {
            await RepositoryComponent.StoreDataAsync(response);
        }

        return response;
    }

    Task<TResp> INadeoSlice<TResp>.HandleAsync() => HandleAsync();
}

/// <summary>
/// A vertical slice that takes input of type <typeparamref name="TReq"/> and retrieves query data
/// for processing.<br/>
/// ALL DERIVED CLASSES SHOULD USE THE 'internal' KEYWORD!!!
/// </summary>
/// <typeparam name="TReq">The input type</typeparam>
/// <typeparam name="TResp">The return type</typeparam>
internal interface INadeoSlice<TReq, TResp>
{
    Task<TResp> HandleAsync(TReq request);
}

/// <inheritdoc cref="INadeoSlice{TReq, TResp}>" />
internal abstract class NadeoSlice<TReq, TResp>
    : INadeoSlice<TReq, TResp>
    where TReq : notnull
    where TResp : notnull
{
    protected abstract INadeoConsumerComponent<TReq, TResp> ConsumerComponent { get; }
    protected abstract RedisRepositoryComponent<TResp>? RepositoryComponent { get; }

    public async Task<TResp> HandleAsync(TReq request)
    {
        if (ConsumerComponent is null)
        {
            throw new ArgumentNullException("The ConsumerComponent cannot be null");
        }

        var response = await ConsumerComponent.FetchData(request);
        
        if (RepositoryComponent is not null)
        {
            await RepositoryComponent.StoreDataAsync(response);
        }

        return response;
    }

    Task<TResp> INadeoSlice<TReq, TResp>.HandleAsync(TReq request) => HandleAsync(request);
}