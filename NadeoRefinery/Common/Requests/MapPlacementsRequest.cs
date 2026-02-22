using System.ComponentModel;
using TOTDBackend.Shared;

namespace TOTDBackend.NadeoRefinery.Common.Requests;

public readonly record struct MapPlacementsRequest
{
    public string MapUid { get; init; }
    public string GroupUid { get; init; }
    public int AuthorScore { get; init; }
    public int GoldScore { get; init; }
    public int SilverScore { get; init; }
    public int BronzeScore { get; init; }
    public readonly int NoneScore => int.MaxValue;

    public readonly int GetScore(MedalType medal)
    {
        return medal switch
        {
            MedalType.Author => AuthorScore,
            MedalType.Gold => GoldScore,
            MedalType.Silver => SilverScore,
            MedalType.Bronze => BronzeScore,
            MedalType.None => NoneScore,
            _ => throw new InvalidEnumArgumentException("The argument, {medal}, is invalid!"),
        };
    }

    // Input corresponds to the int representation of the MedalType enum
    public readonly int GetScore(int medalAsInt)
    {
        return GetScore((MedalType)medalAsInt);
    }
}
