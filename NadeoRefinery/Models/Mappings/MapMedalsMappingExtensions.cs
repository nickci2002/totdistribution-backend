using TOTDBackend.NadeoRefinery.Models.Entities;
using TOTDBackend.NadeoRefinery.Models.Requests;

namespace TOTDBackend.NadeoRefinery.Models.Mappings;

public static partial class MappingExtensions
{
    public static MapMedals ToMapMedals(this TOTDInfo info)
    {
        return new MapMedals
        {
            MapUid = info.MapGuid.ToString(),
            GroupUid = info.SeasonGuid.ToString(),
            AuthorScore = info.AuthorTime,
            GoldScore = info.GoldTime,
            SilverScore = info.SilverTime,
            BronzeScore = info.BronzeTime
        };
    }
}