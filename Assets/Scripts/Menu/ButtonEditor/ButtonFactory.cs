using UnityEngine;

namespace Menu.ButtonEditor
{
    public class ButtonFactory
    {
        public const string Play = nameof(Play);
        public const string Shop = nameof(Shop);
        public const string Settings = nameof(Settings);
        public const string Leaderboard = nameof(Leaderboard);

        public static IMenuButton CreateButton(string buttonType)
        {
            switch (buttonType)
            {
                case Play:
                    return new PlayButton();
                case Settings:
                    return new SettingsButton();
                case Leaderboard:
                    return new LeaderboardButton();
                default:
                    Debug.LogError($"Unknown button type: {buttonType}");
                    return null;
            }
        }
    }
}