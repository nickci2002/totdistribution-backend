using StackExchange.Redis;
using TOTDBackend.Shared.Primatives;

namespace TOTDBackend.NadeoRefinery.Models.Entities;

/// <summary>
/// Entity for storing TOTD data in Redis
/// </summary>
public class TOTDInfo : IRedisEntity
{
    public static string KeyPrefix { get; } = "totd:map";

    public int Id { get; set; }
    public MapUid MapUid { get; set; }
    public MapGuid MapGuid { get; set; }
    public MapGuid SeasonGuid { get; set; }
    public string Name { get; set; } = string.Empty;
    public PlayerGuid Author { get; set; }
    public PlayerGuid Submitter { get; set; }
    public MedalScore AuthorTime { get; set; }
    public MedalScore GoldTime { get; set; }
    public MedalScore SilverTime { get; set; }
    public MedalScore BronzeTime { get; set; }
    public DateTimeOffset UploadTimestamp { get; set; }
    public DateTimeOffset UpdateTimestamp { get; set; }
    public string ThumbnailUrl { get; set; } = string.Empty;

    // Represents an empty instance
    public static readonly TOTDInfo Empty = new();

    // Methods
    public static string GetKey(string key) => IRedisEntity.GetKey<TOTDInfo>(key);
    
    public HashEntry[] Hashify()
    {
        return [
            new(nameof(Id), Id),
            new(nameof(MapUid), MapUid.Value),
            new(nameof(MapGuid), MapGuid.Value.ToString()),
            new(nameof(SeasonGuid), SeasonGuid.Value.ToString()),
            new(nameof(Name), Name),
            new(nameof(Author), Author.Value.ToString()),
            new(nameof(Submitter), Submitter.Value.ToString()),
            new(nameof(AuthorTime), AuthorTime.Value),
            new(nameof(GoldTime), GoldTime.Value),
            new(nameof(SilverTime), SilverTime.Value),
            new(nameof(BronzeTime), BronzeTime.Value),
            new(nameof(UploadTimestamp), UploadTimestamp.ToUnixTimeSeconds()),
            new(nameof(UpdateTimestamp), UpdateTimestamp.ToUnixTimeSeconds()),
            new(nameof(ThumbnailUrl), ThumbnailUrl)
        ];
    }

    public static TOTDInfo Dehashify(HashEntry[]? entries)
    {
        if (entries is null)
        {
            return Empty;
        }

        var dict = entries.ToDictionary(x => x.Name.ToString(), x => x.Value);

        return new TOTDInfo
        {
            Id = (int)dict[nameof(Id)],
            MapUid = new MapUid(dict[nameof(MapUid)].ToString()),
            MapGuid = new MapGuid(Guid.Parse(dict[nameof(MapGuid)].ToString())),
            SeasonGuid = new MapGuid(Guid.Parse(dict[nameof(SeasonGuid)].ToString())),
            Name = dict[nameof(Name)].ToString(),
            Author = new PlayerGuid(Guid.Parse(dict[nameof(Author)].ToString())),
            Submitter = new PlayerGuid(Guid.Parse(dict[nameof(Submitter)].ToString())),
            AuthorTime = new MedalScore((int)dict[nameof(AuthorTime)]),
            GoldTime = new MedalScore((int)dict[nameof(GoldTime)]),
            SilverTime = new MedalScore((int)dict[nameof(SilverTime)]),
            BronzeTime = new MedalScore((int)dict[nameof(BronzeTime)]),
            UploadTimestamp = DateTimeOffset.FromUnixTimeSeconds((long)dict[nameof(UploadTimestamp)]),
            UpdateTimestamp = DateTimeOffset.FromUnixTimeSeconds((long)dict[nameof(UpdateTimestamp)]),
            ThumbnailUrl = dict[nameof(ThumbnailUrl)].ToString()
        };
    }

    IRedisEntity IRedisEntity.Dehashify(HashEntry[]? entries) => Dehashify(entries);

    /// DELETE LATER THIS IS ONLY FOR DATABASE FIXING
    public static TOTDInfo RecoveryDehashify(HashEntry[]? entries)
    {
        if (entries is null)
        {
            return Empty;
        }

        var dict = entries.ToDictionary(x => x.Name.ToString(), x => x.Value);

        return new TOTDInfo
        {
            Id = (int)dict[nameof(Id)],
            MapUid = new MapUid(dict["MapUid.Value"].ToString()),
            MapGuid = new MapGuid(Guid.Parse(dict["MapGuid.Value"].ToString())),
            SeasonGuid = new MapGuid(Guid.Parse(dict["SeasonGuid.Value"].ToString())),
            Name = dict[nameof(Name)].ToString(),
            Author = new PlayerGuid(Guid.Parse(dict["Author.Value"].ToString())),
            Submitter = new PlayerGuid(Guid.Parse(dict["Submitter.Value"].ToString())),
            AuthorTime = new MedalScore((int)dict["AuthorTime.Value"]),
            GoldTime = new MedalScore((int)dict["GoldTime.Value"]),
            SilverTime = new MedalScore((int)dict["SilverTime.Value"]),
            BronzeTime = new MedalScore((int)dict["BronzeTime.Value"]),
            UploadTimestamp = DateTimeOffset.Parse(dict[nameof(UploadTimestamp)].ToString()),
            UpdateTimestamp = DateTimeOffset.Parse(dict[nameof(UpdateTimestamp)].ToString()),
            ThumbnailUrl = dict[nameof(ThumbnailUrl)].ToString()
        };
    }
}