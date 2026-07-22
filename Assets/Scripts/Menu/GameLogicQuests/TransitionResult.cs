namespace Menu.GameLogicQuests
{
    public class TransitionResult
    {
        public TransitionResult(bool showOffer, bool useMobilePanel, string sceneName)
        {
            ShowOffer = showOffer;
            UseMobilePanel = useMobilePanel;
            SceneName = sceneName;
        }

        public bool ShowOffer { get; }

        public bool UseMobilePanel { get; }

        public string SceneName { get; }
    }
}