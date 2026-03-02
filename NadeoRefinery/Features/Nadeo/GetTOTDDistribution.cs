using Redis.OM;
using Redis.OM.Searching;
using Serilog;
using TOTDBackend.NadeoRefinery.Common.Endpoints;
using TOTDBackend.NadeoRefinery.Common.Features;
using TOTDBackend.NadeoRefinery.Common.Requests;
using TOTDBackend.NadeoRefinery.Common.Utils;
using TOTDBackend.NadeoRefinery.Entities;
using TOTDBackend.NadeoRefinery.NadeoApi;
using TOTDBackend.Shared.Primatives;

namespace TOTDBackend.NadeoRefinery.Features.Nadeo;

/// <inheritdoc cref="INadeoSlice{TReq, TResp}" />
internal sealed class GetTOTDDistribution(
    ExtendedNadeoLiveServices liveServices,
    RedisConnectionProvider provider)
    : NadeoSliceWithStorage<MapPlacementsRequest, Distribution>
{
    /// <inheritdoc cref="INadeoConsumerComponent{TReq, TResp}" />
    internal sealed class Consumer(ExtendedNadeoLiveServices nadeoLiveServices)
        : INadeoConsumerComponent<MapPlacementsRequest, Distribution>
    {
        public async Task<Distribution> FetchDataAsync(MapPlacementsRequest request)
        {
            Log.Information("Fetching distribution data from Nadeo servers...");
            
            var mapUid = request.MapUid;
            var groupUid = request.GroupUid;
            
            var numRequests = Enum.GetNames<MedalType>().Length;
            var positionList = new int[5];
            for (int i = 0; i < numRequests; i++)
            {
                var score = request.GetScore(i);
                var position = (await nadeoLiveServices
                    .GetMapPositionByTimeAsync(mapUid, score, groupUid))
                    .ElementAt(0);
                positionList[i] = position.Zones[0].Ranking.Position - 1;
            }

            Log.Information("Finished fetching distribution data!");

            var authorCount = positionList[(int)MedalType.Author];
            var goldCount = positionList[(int)MedalType.Gold] - positionList[(int)MedalType.Author];
            var silverCount = positionList[(int)MedalType.Silver] - positionList[(int)MedalType.Gold];
            var bronzeCount = positionList[(int)MedalType.Bronze] - positionList[(int)MedalType.Gold];
            var noneCount = positionList[(int)MedalType.None] - positionList[(int)MedalType.Bronze];

            return new Distribution
            {
                Id = TOTDDayFinder.CreateRedisTOTDIdKey(),
                MapUid = mapUid,
                GroupUid = groupUid,
                AuthorCount = authorCount,
                GoldCount= goldCount,
                SilverCount = silverCount,
                BronzeCount = bronzeCount,
                NoneCount = noneCount,
            };
        }
    }

    /// <inheritdoc cref="IRedisRepositoryComponent{T}" />
    internal sealed class Repository(RedisConnectionProvider provider)
        : IRedisRepositoryComponent<Distribution>
    {
        private readonly RedisCollection<Distribution> _collection =
            (RedisCollection<Distribution>)provider.RedisCollection<Distribution>();
        
        public async Task StoreDataAsync(Distribution data, TimeSpan? expiry = null)
        {
            await _collection.InsertAsync(data, WhenKey.Always, expiry);
        }

        public Distribution RetrieveData(string key)
        {
            throw new NotImplementedException();
        }
    }

    /// <inheritdoc cref="ITestableEndpoint" />
    public sealed class TestEndpoint : ITestableEndpoint
    {
        public void MapTestingEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("/totd/get-distribution", async(GetTOTDDistribution query,
                                                        MapPlacementsRequest request) =>
            {
                Log.Debug("{Request}", request);
                return await query.HandleConsumeAndStorageAsync(request);
            });
        }
    }

    protected override Consumer ConsumerComponent => new(liveServices);
    protected override Repository RepositoryComponent => new(provider);
}