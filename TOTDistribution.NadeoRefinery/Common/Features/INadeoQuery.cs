namespace TOTDistribution.NadeoRefinery.Common.Features;

internal interface INadeoQuery<TResp>
{
    Task<TResp> HandleAsync();
}

internal interface INadeoQuery<TReq, TResp>
{
    Task<TResp> HandleAsync(TReq request);
}