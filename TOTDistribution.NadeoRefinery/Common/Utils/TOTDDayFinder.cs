namespace TOTDistribution.NadeoRefinery.Common.Utils;

public static class TOTDDayFinder
{
    public static int CalculateCurrentTOTDDay()
    {
        return GetCurrentRSTDateTime().GetTOTDDayOfTheMonth();
    }

    public static int CreateTOTDDayId()
    {
        DateTime date = GetCurrentRSTDateTime();
        
        return date.Year * 10000 + date.Month * 100 + date.GetTOTDDayOfTheMonth();
    }

    private static DateTime GetCurrentRSTDateTime()
    {
        return TimeZoneInfo.ConvertTime(DateTime.Now,
                                        TimeZoneInfo.Local,
                                        TimeZoneInfo.FindSystemTimeZoneById("Romance Standard Time"));
    }

    private static int GetTOTDDayOfTheMonth(this DateTime date)
    {
        return date.Hour >= 19 ? date.Day : date.Day - 1;
    }
}