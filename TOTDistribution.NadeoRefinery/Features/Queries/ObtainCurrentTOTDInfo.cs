using System.Diagnostics;
using ManiaAPI.NadeoAPI;
using Redis.OM;
using Redis.OM.Searching;
using TOTDistribution.NadeoRefinery.Entities;
using TOTDistribution.Shared;

namespace TOTDistribution.NadeoRefinery.Features.Queries;

public sealed class ObtainCurrentTOTDInfo : INadeoQueryWithStorage<TOTDInfo>
{
    private readonly RedisConnectionProvider _provider;
    private readonly RedisCollection<TOTDInfo> _collection;

    /// <summary>
    /// Custom record struct to hold the TOTD's MapInfoLive data along with its SeasonGuid
    /// </summary>
    internal record struct MapInfoLiveWithSeasonGuid(MapInfoLive Info, MapGuid SeasonGuid);

    /// <inheritdoc cref="IConsumer{TResp}"/>
    internal class Consumer : IConsumer<MapInfoLiveWithSeasonGuid>
    {
        private readonly NadeoLiveServices _nadeoLiveServices;

        public Consumer(NadeoLiveServices nadeoLiveServices)
        {
            _nadeoLiveServices = nadeoLiveServices;
        }
        public async Task<MapInfoLiveWithSeasonGuid> FetchData(IConsumerRequest? request = null)
        {
            var totdList = await _nadeoLiveServices.GetTrackOfTheDaysAsync(1, 0, false);
            Debug.Assert(totdList is not null, "Failed to retrieve TOTD list from Nadeo Live Services");
            Debug.Assert(totdList.MonthList.Any(), "No track of the days for the given month!");

            var dateNow = DateTime.UtcNow;
            var totdCurrent = totdList.MonthList[0].Days[dateNow.Day - 1];
            Debug.Assert(totdCurrent is not null, 
                         "There is no TOTD for {dateNow.Day}/{dateNow.Month}/{dateNow.Year}");
            
            var totdMapUid = totdCurrent.MapUid;
            var totdInfo = await _nadeoLiveServices.GetMapInfoAsync(totdMapUid);
            Debug.Assert(totdInfo is not null, $"The map {totdMapUid} does not exist");

            var totdSeasonGuid = totdCurrent.SeasonUid;
            Debug.Assert(totdSeasonGuid != Guid.Empty, "TOTD Season UID is null");

            return new MapInfoLiveWithSeasonGuid(totdInfo, totdSeasonGuid);
        }
    }

    /// <inheritdoc cref="IFormatter{TInput,TOutput}"/>
    internal class Formatter : IFormatter<MapInfoLiveWithSeasonGuid, TOTDInfo>
    {
        public TOTDInfo Format(MapInfoLiveWithSeasonGuid mapInfoLive)
        {
            return new TOTDInfo
            {
                MapUid = mapInfoLive.Info.Uid,
                MapGuid = mapInfoLive.Info.MapId,
                SeasonGuid = mapInfoLive.SeasonGuid,
                Name = mapInfoLive.Info.Name,
                Author = mapInfoLive.Info.Author,
                Submitter = mapInfoLive.Info.Submitter,
                AuthorTime = mapInfoLive.Info.AuthorTime,
                GoldTime = mapInfoLive.Info.GoldTime,
                SilverTime = mapInfoLive.Info.SilverTime,
                BronzeTime = mapInfoLive.Info.BronzeTime,
                UploadTimestamp = mapInfoLive.Info.UploadTimestamp,
                UpdateTimestamp = mapInfoLive.Info.UpdateTimestamp,
                ThumbnailUrl = new Uri(mapInfoLive.Info.ThumbnailUrl)
            };
        }
    }

    // Main class implementation
    private readonly Consumer _consumer;
    private readonly Formatter _formatter;

    public ObtainCurrentTOTDInfo(NadeoLiveServices liveServices, RedisConnectionProvider provider)
    {
        _consumer = new Consumer(liveServices);
        _formatter = new Formatter();
        _provider = provider;
        _collection = (RedisCollection<TOTDInfo>)_provider.RedisCollection<TOTDInfo>();
    }
    
    public async Task<TOTDInfo> ExecuteQuery()
    {
        return await _consumer.FetchData()
            .ContinueWith(task => _formatter.Format(task.Result));
    }

    public async Task StoreDataAsync(TOTDInfo data, TimeSpan? expiry = null)
    {
        await _collection.InsertAsync(data);
    }

    public Task<TOTDInfo> RetrieveDataAsync(string key)
    {
        throw new NotImplementedException();
    }
}
