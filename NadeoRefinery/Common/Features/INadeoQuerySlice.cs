namespace TOTDBackend.NadeoRefinery.Common.Features;

/// <summary>
/// A vertical slice that takes no input and retrieves query data for processing.
/// </summary>
/// <typeparam name="TResp">The return type</typeparam>
internal interface INadeoQuerySlice<TResp>
{
    Task<TResp> HandleAsync();
}

/// <summary>
/// A vertical slice that takes input of type <typeparamref name="TReq"/> and retrieves query data
/// for processing.
/// </summary>
/// <typeparam name="TReq">The input type</typeparam>
/// <typeparam name="TResp">The return type</typeparam>
internal interface INadeoQuerySlice<TReq, TResp>
{
    Task<TResp> HandleAsync(TReq request);
}