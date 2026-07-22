using System.Collections.Generic;
using YG;

namespace Menu.GameLogicQuests
{
    public class QuestCustomizer
    {
        private readonly int _indexTutorialBasics = 0;
        private readonly int _indexAbilityTutorial;
        private readonly int _indexUnblockingTutorial;
        private readonly bool _isTutorialBasics;
        private readonly bool _isUnblockingTutorial;
        private readonly bool _isAbilityTutorial;

        public QuestCustomizer()
        {
            _indexUnblockingTutorial = YG2.saves.IndexSecondQuest;
            _indexAbilityTutorial = YG2.saves.ObstacleDeactivateIndex;

            _isTutorialBasics = YG2.saves.IsTutorialBasics;
            _isUnblockingTutorial = YG2.saves.IsUnblockingTutorial;
            _isAbilityTutorial = YG2.saves.IsAbilityTutorial;
        }

        public void Apply(List<Quest> quests)
        {
            for (int i = 0; i < quests.Count; i++)
            {
                quests[i].SetIndex(i);
            }

            if (YG2.saves.MaxReachedQuestIndex >= _indexAbilityTutorial)
            {
                EnableAllTutorials();
                return;
            }

            for (int i = 0; i < quests.Count; i++)
            {
                ApplyState(quests[i]);
            }
        }

        private void ApplyState(Quest quest)
        {
            SetTutorial(quest, _isTutorialBasics, _indexTutorialBasics);
            SetTutorial(quest, _isUnblockingTutorial, _indexUnblockingTutorial);
            SetTutorial(quest, _isAbilityTutorial, _indexAbilityTutorial);
        }

        private void SetTutorial(Quest quest, bool isTutorial, int index)
        {
            if (quest.Index == index && isTutorial == false)
            {
                quest.SetTutorial(false);
            }
        }

        private void EnableAllTutorials()
        {
            YG2.saves.IsTutorialBasics = true;
            YG2.saves.IsUnblockingTutorial = true;
            YG2.saves.IsAbilityTutorial = true;
        }
    }
}