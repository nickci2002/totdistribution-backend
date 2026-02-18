using ManiaAPI.NadeoAPI;
using System.Collections.Immutable;

namespace TOTDistribution.NadeoRefinery.NadeoApi.Records;

public sealed record Position(string GroupUid,
                              string MapUid,
                              int Score,
                              ImmutableList<SeasonPlayerRankingZone> Zones);