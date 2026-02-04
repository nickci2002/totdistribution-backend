using System.ComponentModel;
using TOTDistribution.Shared;

namespace TOTDistribution.NadeoRefinery.Common.Requests;

public record struct MapPlacementsRequest
{
    public string MapUid { get; init; }
    public string GroupUid { get; init; }
    public int AuthorScore { get; init; }
    public int GoldScore { get; init; }
    public int SilverScore { get; init; }
    public int BronzeScore { get; init; }
    public int NoneScore => int.MaxValue;

    // 
    public int GetScore(MedalType medal)
    {
        switch(medal)
        {
            case MedalType.Author:
                return AuthorScore;
            case MedalType.Gold:
                return GoldScore;
            case MedalType.Silver:
                return SilverScore;
            case MedalType.Bronze:
                return BronzeScore;
            case MedalType.None:
                return NoneScore;
        }
        
        throw new InvalidEnumArgumentException("The argument, {medal}, is invalid!");
    }

    // Input corresponds to the int representation of the MedalType enum
    public int GetScore(int medalAsInt)
    {
        return GetScore((MedalType)medalAsInt);
    }
}
