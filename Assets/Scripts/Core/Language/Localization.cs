public static class Localization
{
    private static Language currentLanguage;
    public static string langFileName;

    public delegate void LanguageLoadedEventHandler();
    public static event LanguageLoadedEventHandler LanguageLoaded;

    public static string GetLocalizedString(string id)
    {
        return currentLanguage.GetString(id);
    }

    public static void LoadLanguage(string langfile)
    {
        langFileName = langfile;
        currentLanguage = new Language(langfile);
        LanguageLoaded?.Invoke();
    }
}