using System;

namespace Tradility.Common.Extensions
{
    public static class DateTimeExtensions
    {
        public static bool Compare(this DateOnly? a, DateOnly? b)
        {
            if (a == null || b == null)
                return false;

            return a == b;
        }
        public static bool Compare(this DateOnly? a, DateOnly b) => Compare(a, b);
        public static bool Compare(this DateOnly a, DateOnly? b) => Compare(a, b);

        public static DateOnly ToDateOnly(this DateTime dateTime) => DateOnly.FromDateTime(dateTime);
        public static DateOnly ToDateOnly(this DateTime? dateTime) => ToDateOnly(dateTime ?? throw new ArgumentNullException(nameof(dateTime)));
        public static DateOnly ToDateOnly(this DateTimeOffset dateTimeOffset) => new(dateTimeOffset.Year, dateTimeOffset.Month, dateTimeOffset.Day);
        public static DateOnly ToDateOnly(this DateTimeOffset? dateTimeOffset) => ToDateOnly(dateTimeOffset ?? throw new ArgumentNullException(nameof(dateTimeOffset)));
        
        public static DateTime ToDateTime(this DateOnly dateOnly) => new (dateOnly.Year, dateOnly.Month, dateOnly.Day);
        public static DateTimeOffset ToDateTimeOffset(this DateOnly dateOnly) => new (dateOnly.ToDateTime());

        public static long ToUnixSeconds(this DateTime dateTime) => new DateTimeOffset(dateTime).ToUnixTimeSeconds();
        public static long ToUTCUnixSeconds(this DateTime dateTime) => new DateTimeOffset(dateTime, TimeSpan.FromTicks(0)).ToUnixTimeSeconds();
        public static long ToUTCUnixSeconds(this DateTimeOffset dateTimeOffset) => new DateTimeOffset(dateTimeOffset.Ticks, TimeSpan.FromTicks(0)).ToUnixTimeSeconds();
        public static long ToUnixSeconds(this DateTime? dateTime)
        {
            if (dateTime == null)
                return 0;

            return dateTime.Value.ToUnixSeconds();
        }
        public static long ToUTCUnixSeconds(this DateTime? dateTime)
        {
            if (dateTime == null)
                return 0;

            return dateTime.Value.ToUTCUnixSeconds();
        }

        public static DateTimeOffset UnixSecondsToDateTimeOffset(this long unixTimestamp)
        {
            var dateTimeOffset = new DateTimeOffset(1970, 1, 1, 0, 0, 0, 0, TimeSpan.FromTicks(0));
            dateTimeOffset = dateTimeOffset.AddSeconds(unixTimestamp);
            return dateTimeOffset;
        }
    }

    public static class Now
    {
        public static DateOnly DateOnly => DateOnly.FromDateTime(DateTime.Now);
        public static TimeOnly TimeOnly => TimeOnly.FromDateTime(DateTime.Now);
        public static DateTimeOffset DateTimeOffset => DateTimeOffset.UtcNow;
        public static DateTime DateTime => DateTime.Now;
    }
}
