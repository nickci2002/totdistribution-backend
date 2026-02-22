using System.Diagnostics;
using Redis.OM;
using Redis.OM.Searching;
using TOTDBackend.NadeoRefinery.Common.Endpoints;
using TOTDBackend.NadeoRefinery.Common.Features;
using TOTDBackend.NadeoRefinery.Common.Utils;
using TOTDBackend.NadeoRefinery.Entities;
using TOTDBackend.NadeoRefinery.NadeoApi;

namespace TOTDBackend.NadeoRefinery.Features.Queries;

/// <inheritdoc cref="INadeoQuerySlice{TResp}">
public sealed class ObtainCurrentTOTDInfo(ExtendedNadeoLiveServices liveServices, RedisConnectionProvider provider) : INadeoQuerySlice<TOTDInfo>
{
    /// <inheritdoc cref="IConsumer{TResp}"/>
    internal sealed class Consumer(ExtendedNadeoLiveServices nadeoLiveServices) : IConsumer<TOTDInfo>
    {
        public async Task<TOTDInfo> FetchData()
        {
            var totdList = await nadeoLiveServices.GetTrackOfTheDaysAsync(1, 0, false);
            Debug.Assert(totdList is not null, "Failed to retrieve TOTD list from Nadeo Live Services");
            Debug.Assert(totdList.MonthList.Any(), "No track of the days for the given month!");

            var totdDay = TOTDDayFinder.CalculateCurrentTOTDDay();
            var totdCurrent = totdList.MonthList[0].Days[totdDay - 1];
            Debug.Assert(totdCurrent is not null, 
                         "There is no TOTD for {dateNow.Day}/{dateNow.Month}/{dateNow.Year}");
            
            var totdSeasonGuid = totdCurrent.SeasonUid;
            Debug.Assert(totdSeasonGuid != Guid.Empty, "TOTD Season UID is null");

            var totdMapUid = totdCurrent.MapUid;
            var totdInfo = await nadeoLiveServices.GetMapInfoAsync(totdMapUid);
            Debug.Assert(totdInfo is not null, $"The map {totdMapUid} does not exist");

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

    /// <inheritdoc cref="IRedisRepository{T}"/>
    internal sealed class Repository(RedisConnectionProvider provider) : IRedisRepository<TOTDInfo>
    {
        private readonly RedisCollection<TOTDInfo> _collection = 
            (RedisCollection<TOTDInfo>)provider.RedisCollection<TOTDInfo>();

        public async Task<TOTDInfo> RetrieveDataAsync(string key)
        {
            throw new NotImplementedException();
        }

        public async Task StoreDataAsync(TOTDInfo data, TimeSpan? expiry = null)
        {
            await _collection.InsertAsync(data, WhenKey.Always, expiry);
        }
    }

    /// <inheritdoc cref="ITestableEndpoint"/>
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

    private readonly Consumer consumer = new(liveServices);
    private readonly Repository repository = new(provider);

    public async Task<TOTDInfo> HandleAsync()
    {
        var response = await consumer.FetchData();
        await repository.StoreDataAsync(response);

        return response;
    }
}