namespace TOTDistribution.NadeoRefinery.Common.Features;

/// <summary>
/// A query that takes no input and returns the response from Nadeo's servers.
/// </summary>
/// <typeparam name="TResp">The return type</typeparam>
internal interface IConsumer<TResp>
    where TResp : notnull
{
    abstract Task<TResp> FetchData();
}

/// <summary>
/// Query that takes input of type <typeparamref name="TReq"/> and returns the response from
/// Nadeo's servers.
/// </summary>
/// <typeparam name="TReq">The input type</typeparam>
/// <typeparam name="TResp">The return type</typeparam>
internal interface IConsumer<TReq, TResp>
    where TReq : notnull
    where TResp : notnull
{
    abstract Task<TResp> FetchData(TReq request);
}
