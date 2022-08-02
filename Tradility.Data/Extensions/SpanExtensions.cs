using System;

namespace Tradility.Data.Extensions
{
    public static class SpanExtensions
    {
        public static T GetOrDefault<T>(this Span<T> span, int index)
        {
            try
            {
                T value = span[index];
                return value;
            }
            catch (Exception)
            {
                return default!;
            }
        }
    }
}
