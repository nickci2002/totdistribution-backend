namespace TOTDBackend.NadeoRefinery.Common.Features;

/// <summary>
/// A query that takes no input and returns the response from Nadeo's servers.
/// </summary>
/// <typeparam name="TResp">The return type</typeparam>
internal interface INadeoConsumerComponent<TResp>
{
    Task<TResp> FetchDataAsync();
}

/// <summary>
/// Query that takes input of type <typeparamref name="TReq"/> and returns the response from
/// Nadeo's servers.
/// </summary>
/// <typeparam name="TReq">The input type</typeparam>
/// <typeparam name="TResp">The return type</typeparam>
internal interface INadeoConsumerComponent<TReq, TResp>
{
    Task<TResp> FetchDataAsync(TReq request);
}