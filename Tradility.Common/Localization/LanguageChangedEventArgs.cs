namespace Tradility.Common.Localization
{
    public class LanguageChangedEventArgs
    {
        public Language Old { get; }
        public Language New { get; }

        public LanguageChangedEventArgs(Language oldLanguage, Language newLanguage)
        {
            Old = oldLanguage;
            New = newLanguage;
        }
    }
}
