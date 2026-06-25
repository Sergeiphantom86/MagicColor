namespace Game.SaveEditor
{
    public interface ILocalization
    {
        public string GetTranslationLanguage();

        public void SwitchLanguage(string langCode);

        public void SetCurrentLanguage(string langCode);
    }
}