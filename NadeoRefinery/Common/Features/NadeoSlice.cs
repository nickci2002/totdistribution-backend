namespace TOTDBackend.NadeoRefinery.Common.Features;

/// <summary>
/// A vertical slice that takes no input and retrieves query data for processing.<br/>
/// ALL DERIVED CLASSES SHOULD USE THE 'internal' KEYWORD!!!
/// </summary>
/// <typeparam name="TResp">The return type</typeparam>
internal interface INadeoSlice<TResp>
{
    Task<TResp> HandleConsumeAsync();
}

/// <inheritdoc cref="INadeoSlice{TResp}>" />
internal abstract class NadeoSlice<TResp>
    : INadeoSlice<TResp>
    where TResp : notnull
{
    protected abstract INadeoConsumerComponent<TResp> ConsumerComponent { get; }

    public async Task<TResp> HandleConsumeAsync()
    {
        ArgumentNullException.ThrowIfNull(ConsumerComponent);

        return await ConsumerComponent.FetchDataAsync();
    }
}

/// <summary>
/// A vertical slice that takes input of type <typeparamref name="TReq"/> and retrieves query data for processing.<br/>
/// ALL DERIVED CLASSES SHOULD USE THE 'internal' KEYWORD!!!
/// </summary>
/// <typeparam name="TReq">The input type</typeparam>
/// <typeparam name="TResp">The return type</typeparam>
internal interface INadeoSlice<TReq, TResp>
{
    Task<TResp> HandleConsumeAsync(TReq request);
}

/// <inheritdoc cref="INadeoSlice{TReq, TResp}>" />
internal abstract class NadeoSlice<TReq, TResp>
    : INadeoSlice<TReq, TResp>
    where TReq : notnull
    where TResp : notnull
{
    protected abstract INadeoConsumerComponent<TReq, TResp> ConsumerComponent { get; }

    public async Task<TResp> HandleConsumeAsync(TReq request)
    {
        ArgumentNullException.ThrowIfNull(ConsumerComponent);

        return await ConsumerComponent.FetchDataAsync(request);
    }    
}