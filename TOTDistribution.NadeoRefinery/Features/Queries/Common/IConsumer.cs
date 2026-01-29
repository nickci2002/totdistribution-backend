using ManiaAPI.NadeoAPI;

namespace TOTDistribution.NadeoRefinery.Features;

/// <summary>
/// Interface representing a request for a consumer. <br/>
/// It should be implemented as a record struct.
/// </summary>
public interface IConsumerRequest
{
    IConsumerRequest GetRequest();
}

/// <summary>
/// Responsible for consuming data from Nadeo API services.
/// </summary>
/// <typeparam name="TResp">A response type defined in the ManiaAPI.NadeoAPI package</typeparam>
public interface IConsumer<TResp>
    where TResp : notnull
{
    Task<TResp> FetchData(IConsumerRequest? request);
}