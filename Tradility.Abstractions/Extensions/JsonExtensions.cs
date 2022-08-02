using System.Text.Json;

namespace Tradility.Abstractions.Extensions
{
    public static class JsonExtensions
    {
        private static readonly JsonSerializerOptions _options;

        static JsonExtensions()
        {
            _options = new()
            {
                WriteIndented = true
            };
        }

        public static string ToJson<T>(this T data)
        {
            return JsonSerializer.Serialize(data, _options);
        }

        public static T ToObject<T>(this string json)
        {
            return JsonSerializer.Deserialize<T>(json, _options)!;
        }
    }
}
