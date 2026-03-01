using System.Diagnostics;
using Redis.OM;
using Serilog;
using TOTDBackend.NadeoRefinery.Common.Endpoints;
using TOTDBackend.NadeoRefinery.Common.Features;
using TOTDBackend.NadeoRefinery.Common.Utils;
using TOTDBackend.NadeoRefinery.Entities;
using TOTDBackend.NadeoRefinery.NadeoApi;

namespace TOTDBackend.NadeoRefinery.Features.Queries;

/// <inheritdoc cref="NadeoSlice{TResp}" />
internal sealed class ObtainCurrentTOTDInfo(
    ExtendedNadeoLiveServices liveServices,
    RedisConnectionProvider provider)
    : NadeoSlice<TOTDInfo>
{
    /// <inheritdoc cref="NadeoConsumerComponent{TResp}" />
    internal sealed class Consumer(ExtendedNadeoLiveServices nadeoLiveServices)
        : NadeoConsumerComponent<TOTDInfo>
    {
        public override async Task<TOTDInfo> FetchData()
        {
            Log.Information("Fetching TOTD information from Nadeo servers...");

            var totdList = await nadeoLiveServices.GetTrackOfTheDaysAsync(1, 0, false);
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
            var totdInfo = await nadeoLiveServices.GetMapInfoAsync(totdMapUid);
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

    /// <inheritdoc cref="RedisRepositoryComponent{T}" />
    internal sealed class Repository(RedisConnectionProvider provider)
        : RedisRepositoryComponent<TOTDInfo>(provider)
    {
        public override async Task<TOTDInfo> RetrieveDataAsync(string key)
        {
            throw new NotImplementedException();
        }

        public override async Task StoreDataAsync(TOTDInfo data, TimeSpan? expiry = null)
        {
            await _collection.InsertAsync(data, WhenKey.Always, expiry);
        }
    }

    /// <inheritdoc cref="ITestableEndpoint" />
    public sealed class TestEndpoint : ITestableEndpoint
    {
        public void MapTestingEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("/totd/obtain", async (ObtainCurrentTOTDInfo query) => 
            {
                return await query.HandleAsync();
            });
        }
    }

    protected override Consumer ConsumerComponent => new(liveServices);
    protected override Repository RepositoryComponent => new(provider);
}