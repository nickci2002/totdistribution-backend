using Redis.OM;
using Redis.OM.Searching;
using Serilog;
using System.Text.Json;
using TOTDBackend.NadeoRefinery.Common.Endpoints;
using TOTDBackend.NadeoRefinery.Common.Features;
using TOTDBackend.NadeoRefinery.Common.Requests;
using TOTDBackend.NadeoRefinery.Common.Utils;
using TOTDBackend.NadeoRefinery.Entities;
using TOTDBackend.NadeoRefinery.NadeoApi;
using TOTDBackend.Shared;

namespace TOTDBackend.NadeoRefinery.Features.Queries;

/// <inheritdoc cref="INadeoQuerySlice{TReq, TResp}">
public sealed class GetTOTDDistribution(
    ExtendedNadeoLiveServices liveServices,
    RedisConnectionProvider provider)
    : INadeoQuerySlice<MapPlacementsRequest, Distribution>
{
    /// <inheritdoc cref="IConsumer{TReq, TResp}">
    internal sealed class Consumer(ExtendedNadeoLiveServices nadeoLiveServices)
        : IConsumer<MapPlacementsRequest, Distribution>
    {
        public async Task<Distribution> FetchData(MapPlacementsRequest request)
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
                
                Log.Debug("Position JSON: {Json}", JsonSerializer.Serialize(position));
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

    /// <inheritdoc cref="IRedisRepository{T}">
    internal sealed class Repository(RedisConnectionProvider provider) : IRedisRepository<Distribution>
    {
        private readonly RedisCollection<Distribution> _collection = 
            (RedisCollection<Distribution>)provider.RedisCollection<Distribution>();

        public async Task<Distribution> RetrieveDataAsync(string key)
        {
            throw new NotImplementedException();
        }

        public async Task StoreDataAsync(Distribution data, TimeSpan? expiry = null)
        {
            await _collection.InsertAsync(data, WhenKey.Always, expiry);
        }
    }

    /// <inheritdoc cref="ITestableEndpoint">
    public sealed class TestEndpoint : ITestableEndpoint
    {
        public void MapTestingEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("/totd/get-distribution", async(GetTOTDDistribution query,
                                                        MapPlacementsRequest request) =>
            {
                Log.Debug("{Request}", request);
                return await query.HandleAsync(request);
            });
        }
    }

    private readonly Consumer consumer = new(liveServices);
    private readonly Repository repository = new(provider);

    public async Task<Distribution> HandleAsync(MapPlacementsRequest request)
    {
        var response = await consumer.FetchData(request);
        await repository.StoreDataAsync(response);

        return response;
    }
}