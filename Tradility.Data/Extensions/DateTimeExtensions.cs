using System;

namespace Tradility.Data.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateOnly TWSDateTimeToDateOnly(this string ibDate)
        {
            //var year = int.Parse(ibDate.Substring(0, 4));
            //var month = int.Parse(ibDate.Substring(4, 2));
            //var day = int.Parse(ibDate.Substring(6, 2));
            //var date = new DateOnly(year, month, day);

            var date = DateOnly.ParseExact(ibDate, "yyyyMMdd");
            return date;
        }

        public static DateTime TWSDateTimeToDateTime(this string ibDate)
        {
            var date = DateTime.ParseExact(ibDate, "yyyyMMdd", null);
            return date;
        }

        public static DateTimeOffset TWSDateTimeToDateTimeOffset(this string ibDate)
        {
            var date = DateTimeOffset.ParseExact(ibDate, "yyyyMMdd", null);
            return date;
        }
    }
}
