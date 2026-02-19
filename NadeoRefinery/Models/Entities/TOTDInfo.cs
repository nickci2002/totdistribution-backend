using Redis.OM.Modeling;
using TOTDBackend.Shared;

namespace TOTDBackend.NadeoRefinery.Entities;

/// <summary>
/// Entity for storing TOTD data in Redis
/// </summary>
[Document(Prefixes = new[]{"totd:map"})]
public class TOTDInfo
{
    [RedisIdField]
    [Indexed]
    public int Id { get; set; }

    [Indexed]
    public MapUid MapUid { get; set; }

    [Indexed]
    public MapGuid MapGuid { get; set; }

    [Indexed]
    public MapGuid SeasonGuid { get; set; }

    [Indexed]
    public string Name { get; set; } = string.Empty;

    [Indexed]
    public PlayerGuid Author { get; set; }

    [Indexed]
    public PlayerGuid Submitter { get; set; }

    [Indexed]
    public MedalScore AuthorTime { get; set; }

    [Indexed]
    public MedalScore GoldTime { get; set; }

    [Indexed]
    public MedalScore SilverTime { get; set; }

    [Indexed]
    public MedalScore BronzeTime { get; set; }

    [Indexed]
    public DateTimeOffset UploadTimestamp { get; set; }

    [Indexed]
    public DateTimeOffset UpdateTimestamp { get; set; }

    public string ThumbnailUrl { get; set; } = string.Empty;
}
