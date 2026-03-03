namespace TOTDBackend.NadeoRefinery.Common.Features;

/// <summary>
/// A vertical slice that takes no input and retrieves query data for processing.<br/>
/// ALL DERIVED CLASSES SHOULD USE THE 'internal' KEYWORD!!!
/// </summary>
/// <typeparam name="TResp">The return type</typeparam>
internal interface INadeoCommunicatorSlice<TResp>
{
    Task<TResp> HandleConsumeAsync();
}

/// <inheritdoc cref="INadeoCommunicatorSlice{TResp}>" />
internal abstract class NadeoCommunicatorSlice<TResp>
    : INadeoCommunicatorSlice<TResp>
    where TResp : notnull
{
    protected abstract INadeoConsumerComponent<TResp> ConsumerComponent { get; }

    public virtual async Task<TResp> HandleConsumeAsync()
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
internal interface INadeoCommunicatorSlice<TReq, TResp>
{
    Task<TResp> HandleConsumeAsync(TReq request);
}

/// <inheritdoc cref="INadeoCommunicatorSlice{TReq, TResp}>" />
internal abstract class NadeoCommunicatorSlice<TReq, TResp>
    : INadeoCommunicatorSlice<TReq, TResp>
    where TReq : notnull
    where TResp : notnull
{
    protected abstract INadeoConsumerComponent<TReq, TResp> ConsumerComponent { get; }

    public virtual async Task<TResp> HandleConsumeAsync(TReq request)
    {
        ArgumentNullException.ThrowIfNull(ConsumerComponent);

        return await ConsumerComponent.FetchDataAsync(request);
    }    
}