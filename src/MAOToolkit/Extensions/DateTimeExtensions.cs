namespace MAOToolkit.Extensions;

public static class DateTimeExtensions
{
    /// <summary>
    /// Converts a Unix timestamp to a DateTime value.
    /// </summary>
    public static DateTime ToDateTime(this double timestamp)
    {
        return DateTime.UnixEpoch.AddSeconds(timestamp);
    }

    /// <summary>
    /// Converts a DateTime value to a Unix timestamp.
    /// </summary>
    public static double ToUnixTimestamp(this DateTime value)
    {
        var diff = value.ToUniversalTime() - DateTime.UnixEpoch;
        return Math.Floor(diff.TotalSeconds);
    }

    /// <summary>
    /// Returns the first day of the month.
    /// </summary>
    public static DateTime FirstDayOfMonth(this DateTime value)
    {
        return new DateTime(value.Year, value.Month, 1);
    }

    /// <summary>
    /// Returns the number of days in the month.
    /// </summary>
    public static int DaysInMonth(this DateTime value)
    {
        return DateTime.DaysInMonth(value.Year, value.Month);
    }

    /// <summary>
    /// Returns the last day of the month.
    /// </summary>
    public static DateTime LastDayOfMonth(this DateTime value)
    {
        return new DateTime(value.Year, value.Month, value.DaysInMonth());
    }

    /// <summary>
    /// Returns the start of the day.
    /// </summary>
    public static DateTime StartOfDay(this DateTime value)
    {
        return value.Date;
    }

    /// <summary>
    /// Returns the end of the day.
    /// </summary>
    public static DateTime EndOfDay(this DateTime value)
    {
        return value.Date.AddDays(1).AddTicks(-1);
    }
}
