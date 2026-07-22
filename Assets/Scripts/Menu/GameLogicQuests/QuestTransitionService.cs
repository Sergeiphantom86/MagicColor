using UnityEngine;
using YG;

namespace Menu.GameLogicQuests
{
    public class QuestTransitionService : IQuestTransitionService
    {
        private const string Puzzle = nameof(Puzzle);
        private const int MinTutorialIndex = 0;
        private const int TransparentQuestIndex = 2;

        private readonly ZoomChanger _zoomChanger;

        public QuestTransitionService(ZoomChanger zoomChanger)
        {
            _zoomChanger = zoomChanger;
        }

        public TransitionResult ProcessQuest(Quest quest)
        {
            if (quest.Index == TransparentQuestIndex)
            {
                YG2.saves.IsTransparency = true;
            }

            if (quest.IsTutorial == false)
            {
                quest.SetTutorial(true);
                SetTutorial(quest.Index);

                return new TransitionResult(
                    showOffer: true,
                    useMobilePanel: _zoomChanger.IsMobileWithTallScreen(),
                    sceneName: null
                );
            }

            YG2.saves.IsUnlockAbilities = true;

            if (quest.Index < YG2.saves.ObstacleDeactivateIndex)
            {
                YG2.saves.IsUnlockAbilities = false;
            }

            return new TransitionResult(
                showOffer: false,
                useMobilePanel: false,
                sceneName: Puzzle
            );
        }

        public void SaveSprite(Sprite sprite)
        {
            if (sprite != null)
            {
                YG2.saves.SetCurrent(sprite);
            }
            else
            {
                Debug.LogWarning("Cached sprite is null during transition.");
            }
        }

        private void SetTutorial(int index)
        {
            if (index < MinTutorialIndex)
            {
                Debug.LogWarning($"SetTutorial: index {index} is out of the valid tutorial range");
                return;
            }

            if (index >= YG2.saves.IndexSecondQuest)
            {
                YG2.saves.IsUnlockKey = true;
            }

            if (index >= YG2.saves.ObstacleDeactivateIndex)
            {
                YG2.saves.IsUnlockAbilities = true;
            }
        }
    }
}