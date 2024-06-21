namespace MAOToolkit.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime ToDateTime(this double timestamp)
        {
            return DateTime.UnixEpoch.AddSeconds(timestamp);
        }

        public static double ToUnixTimestamp(this DateTime value)
        {
            var diff = value.ToUniversalTime() - DateTime.UnixEpoch;
            return Math.Floor(diff.TotalSeconds);
        }

        public static DateTime FirstDayOfMonth(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, 1);
        }

        public static int DaysInMonth(this DateTime value)
        {
            return DateTime.DaysInMonth(value.Year, value.Month);
        }

        public static DateTime LastDayOfMonth(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, value.DaysInMonth());
        }

        public static DateTime StartOfDay(this DateTime value)
        {
            return value.Date;
        }

        public static DateTime EndOfDay(this DateTime value)
        {
            return value.Date.AddDays(1).AddTicks(-1);
        }
    }
}