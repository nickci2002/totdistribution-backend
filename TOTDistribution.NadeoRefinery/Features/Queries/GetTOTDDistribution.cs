using Redis.OM;
using Redis.OM.Searching;
using Serilog;
using System.Text.Json;
using TOTDistribution.NadeoRefinery.Common.Endpoints;
using TOTDistribution.NadeoRefinery.Common.Features;
using TOTDistribution.NadeoRefinery.Common.Requests;
using TOTDistribution.NadeoRefinery.Common.Utils;
using TOTDistribution.NadeoRefinery.Entities;
using TOTDistribution.NadeoRefinery.NadeoApi;
using TOTDistribution.Shared;

namespace TOTDistribution.NadeoRefinery.Features.Queries;

/// <inheritdoc cref="INadeoQuerySlice{TReq, TResp}">
public sealed class GetTOTDDistribution : INadeoQuerySlice<MapPlacementsRequest, Distribution>
{
    /// <inheritdoc cref="IConsumer{TReq, TResp}">
    internal sealed class Consumer : IConsumer<MapPlacementsRequest, Distribution>
    {
        private readonly ExtendedNadeoLiveServices _nadeoLiveServices;

        internal Consumer(ExtendedNadeoLiveServices nadeoLiveServices)
        {
            _nadeoLiveServices = nadeoLiveServices;
        }

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
                var position = (await _nadeoLiveServices
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
    internal sealed class Repository : IRedisRepository<Distribution>
    {
        private readonly RedisConnectionProvider _provider;
        private readonly RedisCollection<Distribution> _collection;

        internal Repository(RedisConnectionProvider provider)
        {
            _provider = provider;
            _collection = (RedisCollection<Distribution>)_provider.RedisCollection<Distribution>();
        }

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

    private Consumer consumer;
    private Repository repository;

    public GetTOTDDistribution(ExtendedNadeoLiveServices liveServices, RedisConnectionProvider provider)
    {
        consumer = new Consumer(liveServices);
        repository = new Repository(provider);
    }

    public async Task<Distribution> HandleAsync(MapPlacementsRequest request)
    {
        var response = await consumer.FetchData(request);
        await repository.StoreDataAsync(response);

        return response;
    }
}
