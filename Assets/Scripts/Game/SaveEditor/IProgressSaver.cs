namespace Game.SaveEditor
{
    public interface IProgressSaver
        : IYGInit,
            IProgressRecord,
            IQuestProgress,
            ITutorialProgress,
            ICurrencyProgress,
            IAudioSettings,
            IVisualSettings,
            ILocalization,
            IGameplaySettings,
            IAdsService,
            ILeaderboardService,
            IYGEvents { }
}