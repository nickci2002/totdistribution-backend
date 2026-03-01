using TOTDBackend.NadeoRefinery.NadeoApi;

namespace TOTDBackend.NadeoRefinery.Common.Features;

#pragma warning disable CS9113
/// <summary>
/// A query that takes no input and returns the response from Nadeo's servers.
/// </summary>
/// <typeparam name="TResp">The return type</typeparam>
internal interface INadeoConsumerComponent<TResp>
{
    Task<TResp> FetchData();
}

/// <inheritdoc cref="INadeoConsumerComponent{TResp}">
internal abstract class NadeoConsumerComponent<TResp> : INadeoConsumerComponent<TResp>
    where TResp : notnull
{
    public abstract Task<TResp> FetchData();
}

/// <summary>
/// Query that takes input of type <typeparamref name="TReq"/> and returns the response from
/// Nadeo's servers.
/// </summary>
/// <typeparam name="TReq">The input type</typeparam>
/// <typeparam name="TResp">The return type</typeparam>
internal interface INadeoConsumerComponent<TReq, TResp>
{
    Task<TResp> FetchData(TReq request);
}

/// <inheritdoc cref="INadeoConsumerComponent{TReq, TResp}">
internal abstract class NadeoConsumerComponent<TReq, TResp> : INadeoConsumerComponent<TReq, TResp>
    where TReq : notnull
    where TResp : notnull
{
    public abstract Task<TResp> FetchData(TReq request);
}