namespace MAOToolkit.Extensions
{
    public static class TimeZoneInfoExtensions
    {
        /// <summary>
        /// Converts the time in the UTC zone to the time in the customer's time zone.
        /// </summary>
        /// <param name="timeZone"><see cref="TimeZoneInfo"/> of the original time zone.</param>
        /// <param name="value">Date and time in UTC format.</param>
        /// <returns>Date and time in the customer's time zone.</returns>
        public static DateTime ConvertTimeFromUtc(this TimeZoneInfo timeZone, DateTime value)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(value, timeZone);
        }

        /// <summary>
        /// Converts the time in the client's time zone to the time in the UTC zone.
        /// </summary>
        /// <param name="timeZone"><see cref="TimeZoneInfo"/> of the original time zone.</param>
        /// <param name="value">Date and time in the customer's time zone.</param>
        /// <returns>Date and time in UTC format.</returns>
        public static DateTime ConvertTimeToUtc(this TimeZoneInfo timeZone, DateTime value)
        {
            return TimeZoneInfo.ConvertTimeToUtc(value, timeZone);
        }

        public static string GetIanaTimeZoneId(this TimeZoneInfo tzi)
        {
            if (tzi.HasIanaId)
                return tzi.Id;  // no conversion necessary

            if (TimeZoneInfo.TryConvertWindowsIdToIanaId(tzi.Id, out string? ianaId))
                return ianaId;  // use the converted ID

            throw new TimeZoneNotFoundException($"No IANA time zone found for \"{tzi.Id}\".");
        }

        public static string GetWindowsTimeZoneId(this TimeZoneInfo tzi)
        {
            if (!tzi.HasIanaId)
                return tzi.Id;  // no conversion necessary

            if (TimeZoneInfo.TryConvertIanaIdToWindowsId(tzi.Id, out string? winId))
                return winId;   // use the converted ID

            throw new TimeZoneNotFoundException($"No Windows time zone found for \"{tzi.Id}\".");
        }
    }
}