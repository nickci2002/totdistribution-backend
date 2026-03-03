using System.Diagnostics;
using System.Text.Json;
using ManiaAPI.NadeoAPI;
using Redis.OM;
using Redis.OM.Searching;
using TOTDBackend.NadeoRefinery.Common.Endpoints;
using TOTDBackend.NadeoRefinery.Common.Features;
using TOTDBackend.NadeoRefinery.Common.Utils;
using TOTDBackend.NadeoRefinery.Models.Entities;
using TOTDBackend.NadeoRefinery.NadeoApi;

namespace TOTDBackend.NadeoRefinery.Features.Nadeo;

/// <inheritdoc cref="NadeoCommunicatorWithStorageSlice{TResp}" />
internal sealed class GetTOTDInfo(
    NadeoLiveServices liveServices,
    RedisConnectionProvider provider)
    : NadeoCommunicatorWithStorageSlice<TOTDInfo>
{
    /// <inheritdoc cref="INadeoConsumerComponent{TResp}" />
    internal sealed class Consumer(NadeoLiveServices liveServices)
        : INadeoConsumerComponent<TOTDInfo>
    {
        public async Task<TOTDInfo> FetchDataAsync()
        {
            Log.Information("Fetching TOTD information from Nadeo servers...");

            var totdList = await liveServices.GetTrackOfTheDaysAsync(1, 0, false);
            // Bail out if certain conditions aren't met
            Debug.Assert(totdList is not null, "Failed to retrieve TOTD list from Nadeo Live Services");
            Debug.Assert(!totdList.MonthList.IsEmpty, "No track of the days for the given month!");

            var totdDay = TOTDDayFinder.CalculateCurrentTOTDDay();
            var totdCurrent = totdList.MonthList[0].Days[totdDay - 1];
            // Bail out if certain conditions aren't met
            Debug.Assert(totdCurrent is not null, 
                         "There is no TOTD for {dateNow.Day}/{dateNow.Month}/{dateNow.Year}");
            
            var totdSeasonGuid = totdCurrent.SeasonUid;
            // Bail out if certain conditions aren't met
            Debug.Assert(totdSeasonGuid != Guid.Empty, "TOTD Season UID is null");

            var totdMapUid = totdCurrent.MapUid;
            var totdInfo = await liveServices.GetMapInfoAsync(totdMapUid);
            // Bail out if certain conditions aren't met
            Debug.Assert(totdInfo is not null, $"The map {totdMapUid} does not exist");

            Log.Information("Finished fetching TOTD information!");

            return new TOTDInfo
            {
                Id = TOTDDayFinder.CreateRedisTOTDIdKey(),
                MapUid = totdInfo.Uid,
                MapGuid = totdInfo.MapId,
                SeasonGuid = totdSeasonGuid,
                Name = totdInfo.Name,
                Author = totdInfo.Author,
                Submitter = totdInfo.Submitter,
                AuthorTime = totdInfo.AuthorTime,
                GoldTime = totdInfo.GoldTime,
                SilverTime = totdInfo.SilverTime,
                BronzeTime = totdInfo.BronzeTime,
                UploadTimestamp = totdInfo.UploadTimestamp,
                UpdateTimestamp = totdInfo.UpdateTimestamp,
                ThumbnailUrl = totdInfo.ThumbnailUrl
            };
        }
    }

    /// <inheritdoc cref="IRedisRepositoryComponent{T}" />
    internal sealed class Repository(RedisConnectionProvider provider)
        : IRedisRepositoryComponent<TOTDInfo>
    {
        private readonly RedisCollection<TOTDInfo> _collection =
            (RedisCollection<TOTDInfo>)provider.RedisCollection<TOTDInfo>();

        public async Task StoreDataAsync(TOTDInfo data, TimeSpan? expiry = null)
        {
            var totdId = data.Id;
            Log.Information("Storing data with key {} into the Redis database...", totdId);
            Log.Debug("Value found: {}", JsonSerializer.Serialize(data));
            
            await _collection.InsertAsync(data, WhenKey.Always, expiry);

            Log.Information("Successfully stored data with key {}!", totdId);
        }
        
        public TOTDInfo RetrieveData(string key)
        {
            Log.Information("Retrieving information for type {} with value {}...", nameof(TOTDInfo), key);

            _collection.FindById(key);
            if (!int.TryParse(key, out var keyAsInt))
            {
                Log.Error("{} is not a valid key! Cancelling search...");
                return TOTDInfo.Empty;
            }

            var redisEntry = _collection.Where(x => x.Id == keyAsInt).FirstOrDefault(TOTDInfo.Empty);

            if (redisEntry == TOTDInfo.Empty)
            {
                Log.Error("Key {} was not found in the Redis database!", key);
            }
            else
            {
                Log.Information("Found data with key {}!", key);
                Log.Debug("Value found: {}", JsonSerializer.Serialize(redisEntry));
            }
            
            return redisEntry;
        }
    }

    /// <inheritdoc cref="IEndpoint" />
    public sealed class TestEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("/totd/map-info", async (GetTOTDInfo query) => 
            {
                return await query.HandleConsumeAndStorageAsync();
            });
        }
    }

    protected override Consumer ConsumerComponent => new(liveServices);
    protected override Repository RepositoryComponent => new(provider);

    public override async Task<TOTDInfo> HandleConsumeAsync()
    {
        return await base.HandleConsumeAsync();
    }

    public override async Task HandleStorageAsync(TOTDInfo totdInfo, TimeSpan? expiry = null)
    {
        await base.HandleStorageAsync(totdInfo, expiry);
    }

    public override async Task<TOTDInfo> HandleConsumeAndStorageAsync(TimeSpan? expiry = null)
    {
        return await base.HandleConsumeAndStorageAsync();
    }

    public override TOTDInfo HandleRetrieval(string key)
    {
        return base.HandleRetrieval(key);
    }
}