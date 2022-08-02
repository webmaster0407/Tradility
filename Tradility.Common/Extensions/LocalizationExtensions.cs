using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Tradility.Common.Localization;

namespace Tradility.Common.Extensions
{
    public static class LocalizationExtensions
    {
        private static readonly Dictionary<Language, string> Codes = new()
        {
            { Language.English, "en-US" },
            { Language.German, "de-DE" },
        };

        public static string ToCode(this Language language)
        {
            var code = Codes[language];
            return code;
        }

        public static Language ToLanguage(this string code)
        {
            var language = Codes.First(x => x.Value == code).Key;

            return language;
        }

        public static CultureInfo ToCultureInfo(this string code)
        {
            var cultureInfo = new CultureInfo(code);

            return cultureInfo;
        }

        public static CultureInfo ToCultureInfo(this Language language)
        {
            var code = language.ToCode();
            var cultureInfo = code.ToCultureInfo();

            return cultureInfo;
        }
    }
}
