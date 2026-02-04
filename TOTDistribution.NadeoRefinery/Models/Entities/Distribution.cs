using Redis.OM.Modeling;

namespace TOTDistribution.NadeoRefinery.Entities;

[Document(Prefixes = new[] { "totd:distribution" })]
public class Distribution
{
    /// <summary>
    /// Format YYYYMMDD where DD is the TOTD day (changes at 19:00 RST)
    /// </summary>
    [RedisIdField]
    public int Id { get; set; }

    [Indexed]
    public string MapUid { get; set; } = string.Empty;

    [Indexed]
    public string GroupUid { get; set; } = string.Empty;

    [Indexed]
    public int AuthorCount { get; set; }

    [Indexed]
    public int GoldCount { get; set; }

    [Indexed]
    public int SilverCount { get; set; }

    [Indexed]
    public int BronzeCount { get; set; }

    [Indexed]
    public int NoneCount { get; set; }

    public int TotalCount => AuthorCount + GoldCount + SilverCount + BronzeCount + NoneCount;
}
