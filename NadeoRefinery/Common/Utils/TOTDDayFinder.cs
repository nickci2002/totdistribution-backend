using static TOTDBackend.NadeoRefinery.Common.Utils.ParisianTimeHelper;

namespace TOTDBackend.NadeoRefinery.Common.Utils;

public static class TOTDDayFinder
{
    public static int CalculateCurrentTOTDDay()
    {
        return CurrentParisianDateTime.GetTOTDDayOfTheMonth();
    }

    public static int CreateRedisTOTDIdKey()
    {
        DateTime date = CurrentParisianDateTime;
        
        return date.Year * 10000 + date.Month * 100 + date.GetTOTDDayOfTheMonth();
    }

    private static int GetTOTDDayOfTheMonth(this DateTime date)
    {
        return date.Hour >= 19 ? date.Day : date.Day - 1;
    }
}