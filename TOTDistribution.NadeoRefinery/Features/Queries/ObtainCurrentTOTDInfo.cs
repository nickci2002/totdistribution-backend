using System.Diagnostics;
using Redis.OM;
using Redis.OM.Searching;
using TOTDistribution.NadeoRefinery.Common.Endpoints;
using TOTDistribution.NadeoRefinery.Common.Features;
using TOTDistribution.NadeoRefinery.Common.Utils;
using TOTDistribution.NadeoRefinery.Entities;
using TOTDistribution.NadeoRefinery.NadeoApi;

namespace TOTDistribution.NadeoRefinery.Features.Queries;

public sealed class ObtainCurrentTOTDInfo : INadeoQuery<TOTDInfo>
{
    /// <inheritdoc cref="IConsumer{TResp}"/>
    internal sealed class Consumer : IConsumer<TOTDInfo>
    {
        private readonly ExtendedNadeoLiveServices _nadeoLiveServices;

        internal Consumer(ExtendedNadeoLiveServices nadeoLiveServices)
        {
            _nadeoLiveServices = nadeoLiveServices;
        }
        
        public async Task<TOTDInfo> FetchData()
        {
            var totdList = await _nadeoLiveServices.GetTrackOfTheDaysAsync(1, 0, false);
            Debug.Assert(totdList is not null, "Failed to retrieve TOTD list from Nadeo Live Services");
            Debug.Assert(totdList.MonthList.Any(), "No track of the days for the given month!");

            var totdDay = TOTDDayFinder.CalculateCurrentTOTDDay();
            var totdCurrent = totdList.MonthList[0].Days[totdDay - 1];
            Debug.Assert(totdCurrent is not null, 
                         "There is no TOTD for {dateNow.Day}/{dateNow.Month}/{dateNow.Year}");
            
            var totdSeasonGuid = totdCurrent.SeasonUid;
            Debug.Assert(totdSeasonGuid != Guid.Empty, "TOTD Season UID is null");

            var totdMapUid = totdCurrent.MapUid;
            var totdInfo = await _nadeoLiveServices.GetMapInfoAsync(totdMapUid);
            Debug.Assert(totdInfo is not null, $"The map {totdMapUid} does not exist");

            //return new MapInfoLiveWithSeasonGuid(totdInfo, totdSeasonGuid);

            return new TOTDInfo
            {
                Id = TOTDDayFinder.CreateTOTDDayId(),
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
                ThumbnailUrl = new Uri(totdInfo.ThumbnailUrl)
            };
        }
    }

    /// <inheritdoc cref="IRedisRepository{T}"/>
    internal sealed class Repository : IRedisRepository<TOTDInfo>
    {
        private readonly RedisConnectionProvider _provider;
        private readonly RedisCollection<TOTDInfo> _collection;

        internal Repository(RedisConnectionProvider provider)
        {
            _provider = provider;
            _collection = (RedisCollection<TOTDInfo>)_provider.RedisCollection<TOTDInfo>();
        }

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

    private Consumer consumer;
    private Repository repository;

    public ObtainCurrentTOTDInfo(ExtendedNadeoLiveServices liveServices, RedisConnectionProvider provider)
    {
        consumer = new Consumer(liveServices);
        repository = new Repository(provider);
    }
    
    public async Task<TOTDInfo> HandleAsync()
    {
        var response = await consumer.FetchData();
        await repository.StoreDataAsync(response);

        return response;
    }
}
