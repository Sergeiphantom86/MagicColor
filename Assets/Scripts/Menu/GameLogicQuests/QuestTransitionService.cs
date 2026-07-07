using UnityEngine;
using YG;

namespace Menu.GameLogicQuests
{
    public class QuestTransitionService : IQuestTransitionService
    {
        private const string Puzzle = nameof(Puzzle);
        private const int MinIndexValue = 0;

        private readonly ZoomChanger _zoomChanger;
        private readonly int _transparentIndex = 2;

        private TransitionResult _result;

        public QuestTransitionService(ZoomChanger zoomChanger)
        {
            _zoomChanger = zoomChanger;
            _result = new();
        }

        public TransitionResult ProcessQuest(Quest quest)
        {
            if (quest.Index == _transparentIndex)
            {
                YG2.saves.IsTransparency = true;
            }

            if (quest.IsTutorial == false)
            {
                quest.SetTutorial(true);
                SetTutorial(quest.Index);

                _result.ShowOffer = true;
                _result.UseMobilePanel = _zoomChanger.IsMobileWithTallScreen();

                return _result;
            }

            YG2.saves.IsUnlockAbilities = true;

            if (quest.Index < YG2.saves.ObstacleDeactivatIndex)
                YG2.saves.IsUnlockAbilities = false;
            else
                YG2.saves.IsUnlockAbilities = true;

            _result.SceneName = Puzzle;

            return _result;
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
            if (index < MinIndexValue)
            {
                Debug.LogWarning($"SetTutorial: index {index} is out of the valid tutorial range");
                return;
            }

            if (index >= YG2.saves.IndexSecondQuest)
            {
                YG2.saves.IsUnlockKey = true;
            }

            if (index >= YG2.saves.ObstacleDeactivatIndex)
            {
                YG2.saves.IsUnlockAbilities = true;
            }
        }
    }
}