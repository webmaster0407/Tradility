using System.Text.Json;
using System.Text.Json.Serialization;
using Tradility.Common.Json.Converters;

namespace Tradility.Common.Extensions
{
    public static class JsonExtensions
    {
        public static readonly JsonSerializerOptions Options;
        public static readonly JsonSerializerOptions FormattedOptions;

        public static string ToJson<T>(this T obj) => JsonSerializer.Serialize(obj, Options);
        public static string ToJson<T>(this T obj, bool isFormatted) => JsonSerializer.Serialize(obj, FormattedOptions);
        public static T? ToModel<T>(this string json) => JsonSerializer.Deserialize<T>(json, Options);

        static JsonExtensions()
        {
            Options = new JsonSerializerOptions();
            Options.Converters.Add(new DateOnlyConverter());

            FormattedOptions = new JsonSerializerOptions();
            FormattedOptions.Converters.Add(new DateOnlyConverter());
            FormattedOptions.WriteIndented = true;
        }
    }
}
