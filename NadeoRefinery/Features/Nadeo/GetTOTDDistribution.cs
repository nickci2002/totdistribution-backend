using ManiaAPI.NadeoAPI;
using Redis.OM;
using Redis.OM.Searching;
using TOTDBackend.NadeoRefinery.Common.Endpoints;
using TOTDBackend.NadeoRefinery.Common.Features;
using TOTDBackend.NadeoRefinery.Models.Requests;
using TOTDBackend.NadeoRefinery.Common.Utils;
using TOTDBackend.NadeoRefinery.Models.Entities;
using TOTDBackend.Shared.Primatives;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace TOTDBackend.NadeoRefinery.Features.Nadeo;

/// <inheritdoc cref="NadeoCommunicatorWithStorageSlice{TReq, TResp}" />
internal sealed class GetTOTDDistribution(
    NadeoLiveServices nadeoLive,
    RedisConnectionProvider redis)
    : NadeoCommunicatorWithStorageSlice<MapMedals, Distribution>
{
    /// <inheritdoc cref="INadeoConsumerComponent{TReq, TResp}" />
    internal sealed class Consumer(NadeoLiveServices nadeoLive)
        : INadeoConsumerComponent<MapMedals, Distribution>
    {
        public async Task<Distribution> FetchDataAsync(MapMedals request)
        {
            Log.Information("Fetching distribution data from Nadeo servers...");
            
            var mapUid = request.MapUid;
            var groupUid = request.GroupUid;
            
            var numRequests = Enum.GetNames<MedalType>().Length;
            var positionList = new int[5];
            for (int i = 0; i < numRequests; i++)
            {
                var score = request.GetScore(i);
                var position = (await nadeoLive
                    .GetLeaderboardPositionByTimeAsync(mapUid, groupUid, score))
                    .ElementAt(0);
                positionList[i] = position.Zones[0].Ranking.Position - 1;
            }

            Log.Information("Finished fetching data!");
            Log.Information("Formatting distribution data for proper storage...");

            var authorCount = positionList[(int)MedalType.Author];
            var goldCount = positionList[(int)MedalType.Gold] - positionList[(int)MedalType.Author];
            var silverCount = positionList[(int)MedalType.Silver] - positionList[(int)MedalType.Gold];
            var bronzeCount = positionList[(int)MedalType.Bronze] - positionList[(int)MedalType.Gold];
            var noneCount = positionList[(int)MedalType.None] - positionList[(int)MedalType.Bronze];

            Log.Information("Finished formatting data!");

            return new Distribution
            {
                Id = TOTDDayFinder.CreateRedisTOTDIdKeyAsInt(),
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
    internal sealed class Repository(RedisConnectionProvider redis)
        : IRedisRepositoryComponent<Distribution>
    {
        private readonly RedisCollection<Distribution> _collection =
            (RedisCollection<Distribution>)redis.RedisCollection<Distribution>();
        
        public async Task StoreDataAsync(Distribution data, TimeSpan? expiry = null)
        {
            await _collection.InsertAsync(data, WhenKey.Always, expiry);
        }

        public Distribution RetrieveData(string key)
        {
            Log.Information("Retrieving information for type {Type} with key {Key}...", nameof(Distribution), key);

            _collection.FindById(key);
            if (!int.TryParse(key, out var keyAsInt))
            {
                Log.Error("{Key} is not a valid key! Cancelling search...", key);
                return Distribution.Empty;
            }

            var redisEntry = _collection.Where(x => x.Id == keyAsInt).FirstOrDefault(Distribution.Empty);

            if (redisEntry == Distribution.Empty)
            {
                Log.Error("Key {Key} was not found in the Redis database!", key);
            }
            else
            {
                Log.Information("Found data with key {Key}!", key);
                Log.Debug("Value found: {Id}", JsonSerializer.Serialize(redisEntry));
            }

            return redisEntry;
        }
    }

    /// <inheritdoc cref="IEndpoint" />
    public sealed class TestEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("/nadeo/distribution", async(
                [FromServices] GetTOTDDistribution query, [FromBody] MapMedals request) =>
            {
                Log.Verbose("{Request}", request);
                return await query.HandleConsumeAndStorageAsync(request);
            });
        }
    }

    protected override Consumer ConsumerComponent => new(nadeoLive);
    protected override Repository RepositoryComponent => new(redis);

    public override async Task<Distribution> HandleConsumeAsync(MapMedals request)
    {
        return await base.HandleConsumeAsync(request);
    }

    public override async Task HandleStorageAsync(Distribution distribution, TimeSpan? expiry = null)
    {
        await base.HandleStorageAsync(distribution, expiry);
    }

    public override async Task<Distribution> HandleConsumeAndStorageAsync(
        MapMedals request, TimeSpan? expiry = null)
    {
        return await base.HandleConsumeAndStorageAsync(request, expiry);
    }

    public override Distribution HandleRetrieval(string key)
    {
        return base.HandleRetrieval(key);
    }
}