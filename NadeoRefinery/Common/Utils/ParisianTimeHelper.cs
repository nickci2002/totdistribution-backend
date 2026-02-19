namespace TOTDBackend.NadeoRefinery.Common.Utils;

public static class ParisianTimeHelper
{
    // Windows
    private static readonly TimeZoneInfo ParisianTimeZoneInfo =
        TimeZoneInfo.FindSystemTimeZoneById("Romance Standard Time");

    // Linux/macOS
    // private static readonly TimeZoneInfo RomanceStandardTime =
    //     TimeZoneInfo.FindSystemTimeZoneById("Europe/Paris");

    /// <summary>
    /// Gets the current Parisian time
    /// </summary>
    public static DateTime CurrentParisianDateTime =>
        TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, ParisianTimeZoneInfo);

    /// <summary>
    /// Converts the provided DateTime in UTC to Parisian time.
    /// </summary>
    /// <param name="dateTime">The date and time to convert in UTC format</param>
    /// <returns>The corresponding Parisian time</returns>
    public static DateTime ConvertToParisianDateTimeFromUtc(this DateTime dateTime)
    {
        var utc = TimeZoneInfo.ConvertTimeToUtc(dateTime, TimeZoneInfo.Local);
        return TimeZoneInfo.ConvertTimeFromUtc(utc, ParisianTimeZoneInfo);
    }

    /// <summary>
    /// Converts the provided DateTime in timeZoneInfo to Parisian time.
    /// </summary>
    /// <param name="dateTime">The date and time to convert</param>
    /// <param name="timeZoneInfo">The time zone we're converting from</param>
    /// <returns></returns>
    public static DateTime ConvertToParisianDateTime(this DateTime dateTime, TimeZoneInfo timeZoneInfo)
    {
        var utc = TimeZoneInfo.ConvertTimeToUtc(dateTime, timeZoneInfo);
        return TimeZoneInfo.ConvertTimeFromUtc(utc, ParisianTimeZoneInfo);
    }
}
