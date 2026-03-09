using ManiaAPI.NadeoAPI;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using StackExchange.Redis;
using System.Diagnostics;
using System.Text.Json;
using TOTDBackend.NadeoRefinery.Common.Endpoints;
using TOTDBackend.NadeoRefinery.Common.Features;
using TOTDBackend.NadeoRefinery.Common.Utils;
using TOTDBackend.NadeoRefinery.Models.Entities;

namespace TOTDBackend.NadeoRefinery.Features.Nadeo;

/// <inheritdoc cref="NadeoCommunicatorWithStorageSlice{TResp}" />
internal sealed class GetTOTDInfo(
    NadeoLiveServices nadeoLive,
    IConnectionMultiplexer redis)
    : NadeoCommunicatorWithStorageSlice<TOTDInfo>
{
    /// <inheritdoc cref="INadeoConsumerComponent{TResp}" />
    internal sealed class Consumer(NadeoLiveServices nadeoLive)
        : INadeoConsumerComponent<TOTDInfo>
    {
        public async Task<TOTDInfo> FetchDataAsync()
        {
            Log.Information("Fetching TOTD information from Nadeo servers...");

            var totdList = await nadeoLive.GetTrackOfTheDaysAsync(1, 0, false);
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
            var totdInfo = await nadeoLive.GetMapInfoAsync(totdMapUid);
            // Bail out if certain conditions aren't met
            Debug.Assert(totdInfo is not null, $"The map {totdMapUid} does not exist");

            Log.Information("Finished fetching TOTD information!");

            return new TOTDInfo
            {
                Id = TOTDDayFinder.CreateRedisTOTDIdKeyAsInt(),
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
    internal sealed class Repository(IConnectionMultiplexer redis)
        : IRedisRepositoryComponent<TOTDInfo>
    {
        private readonly IDatabase _db = redis.GetDatabase();

        public async Task StoreDataAsync(TOTDInfo data, TimeSpan? expiry = null)
        {
            var totdId = data.Id.ToString();

            Log.Information("Storing data with key {Id} into the Redis database...", totdId);
            Log.Debug("Value found: {Data}", JsonSerializer.Serialize(data));
            
            var key = TOTDInfo.GetKey(totdId);
            var entries = data.Hashify();
            await _db.HashSetAsync(key, entries);

            if (expiry.HasValue)
                await _db.KeyExpireAsync(key, expiry);

            Log.Information("Successfully stored data with key {Id}!", totdId);
        }

        public TOTDInfo RetrieveData(string key)
        {
            Log.Information("Retrieving information for type {Type} with key {Key}...", nameof(TOTDInfo), key);
            
            var fullKey = TOTDInfo.GetKey(key);
            var entries = _db.HashGetAll(fullKey);
            if (entries.Length == 0)
            {
                Log.Error("Key {Key} was not found in the Redis database!", fullKey);
                return TOTDInfo.Empty;
            }

            var redisEntry = TOTDInfo.Dehashify(entries);
            if (redisEntry == TOTDInfo.Empty)
            {
                Log.Error("Key {Key} was not found in the Redis database!", fullKey);
                return TOTDInfo.Empty;
            }

            Log.Information("Found data with key {Key}!", fullKey);
            Log.Debug("Value found: {Id}", JsonSerializer.Serialize(redisEntry));

            return redisEntry;
        }
    }

    /// <inheritdoc cref="IEndpoint" />
    public sealed class TestEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("/nadeo/map-info", async ([FromServices] GetTOTDInfo query) => 
            {
                return await query.HandleConsumeAndStorageAsync();
            });
        }
    }

    protected override Consumer ConsumerComponent => new(nadeoLive);
    protected override Repository RepositoryComponent => new(redis);

#pragma warning disable CS0114 // Member hides inherited member; missing override keyword
    public async Task<TOTDInfo> HandleConsumeAsync()
    {
        return await base.HandleConsumeAsync();
    }

    public async Task HandleStorageAsync(TOTDInfo totdInfo, TimeSpan? expiry = null)
    {
        await base.HandleStorageAsync(totdInfo, expiry);
    }

    public async Task<TOTDInfo> HandleConsumeAndStorageAsync(TimeSpan? expiry = null)
    {
        return await base.HandleConsumeAndStorageAsync();
    }

    public TOTDInfo HandleRetrieval(string key)
    {
        return base.HandleRetrieval(key);
    }
#pragma warning restore CS0114 // Member hides inherited member; missing override keyword
}