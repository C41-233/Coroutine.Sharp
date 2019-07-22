using System;

namespace Coroutine.Timer
{
    public static class DateTimeExtension
    {

        private static readonly DateTime UtcStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));

        public static long ToTimeStamp(this DateTime self)
        {
            var span = self.Subtract(UtcStart);
            return (long)span.TotalMilliseconds;
        }

        public static DateTime ToDateTime(this long self)
        {
            return UtcStart + TimeSpan.FromMilliseconds(self);
        }

    }
}
