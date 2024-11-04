using System;

namespace Code
{
    public static class LanguageController
    {
        public static Language Current { get; private set; } = Language.English;
        
        public static event System.Action<Language> OnLanguageChanged;
        
        public static void SetLanguage(Language language)
        {
            Current = language;
            OnLanguageChanged?.Invoke(language);
        }
    }

    public enum Language
    {
        English,
        Russian
    }
}