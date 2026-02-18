public interface ILocalization
{
    public string GetTranslationLanguage();
    public void SwitchLanguage(string langCode);
}