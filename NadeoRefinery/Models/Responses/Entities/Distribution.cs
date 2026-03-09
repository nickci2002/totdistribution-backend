using ManiaAPI.NadeoAPI;
using Redis.OM.Modeling;
using StackExchange.Redis;
using TOTDBackend.Shared.Primatives;

namespace TOTDBackend.NadeoRefinery.Models.Entities;

[Document(Prefixes = new[] { "totd:distribution" })]
public class Distribution : IRedisEntity
{
    public static string KeyPrefix { get; } = "totd:distribution";

    public int Id { get; set; }
    public string MapUid { get; set; } = string.Empty;
    public string GroupUid { get; set; } = string.Empty;
    public int AuthorCount { get; set; }
    public int GoldCount { get; set; }
    public int SilverCount { get; set; }
    public int BronzeCount { get; set; }
    public int NoneCount { get; set; }
    public int TotalCount => AuthorCount + GoldCount + SilverCount + BronzeCount + NoneCount;

    public static readonly Distribution Empty = new();

    public static string GetKey(string key) => IRedisEntity.GetKey<Distribution>(key);

    public HashEntry[] Hashify()
    {
        return [
            new(nameof(Id), Id),
            new(nameof(MapUid), MapUid),
            new(nameof(GroupUid), GroupUid.ToString()),
            new(nameof(AuthorCount), AuthorCount),
            new(nameof(GoldCount), GoldCount),
            new(nameof(SilverCount), SilverCount),
            new(nameof(BronzeCount), BronzeCount),
            new(nameof(NoneCount), NoneCount)
        ];
    }

    public static Distribution Dehashify(HashEntry[]? entries)
    {
        if (entries is null)
        {
            return Empty;
        }

        var dict = entries.ToDictionary(x => x.Name.ToString(), x => x.Value);

        return new Distribution
        {
            Id = (int)dict[nameof(Id)],
            MapUid = dict[nameof(MapUid)].ToString(),
            //GroupUid = new MapGuid(Guid.Parse(dict[nameof(GroupUid)].ToString())),
            GroupUid = dict[nameof(GroupUid)].ToString(),
            AuthorCount = (int)dict[nameof(AuthorCount)],
            GoldCount = (int)dict[nameof(GoldCount)],
            SilverCount = (int)dict[nameof(SilverCount)],
            BronzeCount = (int)dict[nameof(BronzeCount)],
            NoneCount = (int)dict[nameof(NoneCount)]
        };
    }

    IRedisEntity IRedisEntity.Dehashify(HashEntry[]? entries) => Dehashify(entries);
}