using ManiaAPI.NadeoAPI;
using Serilog;
using System.Collections.Immutable;
using System.Text.Json;
using TOTDistribution.NadeoRefinery.NadeoApi.Records;

namespace TOTDistribution.NadeoRefinery.NadeoApi;

public class ExtendedNadeoLiveServices : NadeoLiveServices
{
    public ExtendedNadeoLiveServices(HttpClient httpClient, NadeoAPIHandler nadeoAPIHandler, bool automaticallyAuthorize = true)
        : base(httpClient, nadeoAPIHandler, automaticallyAuthorize)
    {   
    }

    public virtual async Task<TopLeaderboardCollection> GetSurroundingRecordsByTimeAsync(string groupUid,
                                                                                         string mapUid,
                                                                                         int length,
                                                                                         bool onlyWorld = true,
                                                                                         int offset = 0,
                                                                                         CancellationToken cancellationToken = default)
    {
        var queryParams = $"length={length}&onlyWorld={onlyWorld}&offset={offset}";
        
        return await GetJsonAsync($"token/leaderboard/group/{groupUid}/map/{mapUid}/top?{queryParams}",
            ExtendedJsonContext.Default.TopLeaderboardCollection, cancellationToken);
    }
    
    /// <summary>
    /// Retrieves multiple map positions.<br/>
    /// If you want to query ONLY ONE map, consider using <see cref="GetMapPositionByTimeAsync"/>
    /// </summary>
    /// <param name="mapUid"></param>
    /// <param name="score"></param>
    /// <param name="groupUid"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual async Task<ImmutableList<MapPosition>> GetMapPositionsByTimeAsync(IEnumerable<string> mapUids,
                                                                                     IEnumerable<int> scores,
                                                                                     IEnumerable<string> groupUids,
                                                                                     CancellationToken cancellationToken = default)
    {
        var body = new MapGroupIdCollection(
            [.. mapUids.Zip(groupUids, (mapUid, groupUid) => new MapGroupId(mapUid, groupUid))]);
        var jsonContent = JsonContent.Create(body, ExtendedJsonContext.Default.MapGroupIdCollection);
        var json = await jsonContent.ReadAsStringAsync(cancellationToken);

        Log.Debug("{Json}", json);
        var queryParams = mapUids.Zip(scores, (mapUid, score) => $"scores[{mapUid}]={score}")
            .Aggregate((a, b) => $"{a}&{b}");
        
        return await PostJsonAsync($"token/leaderboard/group/map?{queryParams}", jsonContent, 
            ExtendedJsonContext.Default.ImmutableListMapPosition, cancellationToken);
    }

    /// <summary>
    /// Retrieves ONLY ONE map position.<br/>
    /// If you want to query multiple, use <see cref="GetMapPositionsByTimeAsync"/>
    /// </summary>
    /// <param name="mapUid"></param>
    /// <param name="score"></param>
    /// <param name="groupUid"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual async Task<ImmutableList<MapPosition>> GetMapPositionByTimeAsync(string mapUid,
                                                                                    int score,
                                                                                    string groupUid,
                                                                                    CancellationToken cancellationToken = default)
    {
        return await GetMapPositionsByTimeAsync([mapUid], [score], [groupUid], cancellationToken);
    }
}
