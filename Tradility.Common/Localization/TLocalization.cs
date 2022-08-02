using System;

namespace Tradility.Common.Localization
{
    public class TLocalization
    {
        public static readonly TLocalization Instance = new();

        public event EventHandler<LanguageChangedEventArgs>? LanguageChanged;

        private Language language;
        public Language Language
        {
            get => language;
            set => SetLanguage(value);
        }

        private void SetLanguage(Language value)
        {
            if (language == value)
                return;

            var oldLanguage = language;
            language = value;

            OnLanguageChanged(oldLanguage, language);
        }

        private void OnLanguageChanged(Language oldLanguage, Language newLanguage)
        {
            var args = new LanguageChangedEventArgs(oldLanguage, newLanguage);

            LanguageChanged?.Invoke(this, args);
        }
    }
}
