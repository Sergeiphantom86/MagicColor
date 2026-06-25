using System.Collections.Generic;
using Game.SaveEditor;

namespace Menu.QuestEditor
{
    public class QuestCustomizer
    {
        private readonly IProgressSaver _progressSaver;
        private readonly int _indexTutorialBasics = 0;
        private readonly int _indexAbilityTutorial;
        private readonly int _indexUnblockingTutorial;
        private readonly bool _isTutorialBasics;
        private readonly bool _isUnblockingTutorial;
        private readonly bool _isAbilityTutorial;

        public QuestCustomizer(IProgressSaver progressSaver)
        {
            _progressSaver = progressSaver;

            _indexUnblockingTutorial = _progressSaver.Saves.IndexSecondQuest;
            _indexAbilityTutorial = _progressSaver.Saves.ObstacleDeactivatIndex;

            _isTutorialBasics = _progressSaver.Saves.IsTutorialBasics;
            _isUnblockingTutorial = _progressSaver.Saves.IsUnblockingTutorial;
            _isAbilityTutorial = _progressSaver.Saves.IsAbilityTutorial;
        }

        public void Apply(List<Quest> quests)
        {
            for (int i = 0; i < quests.Count; i++)
            {
                quests[i].SetIndex(i);
            }

            if (_progressSaver.Saves.MaxReachedQuestIndex >= _indexAbilityTutorial)
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
            _progressSaver.SetTutorialBasics();
            _progressSaver.SetUnblockingTutorial();
            _progressSaver.SetAbilityTutorial();
        }
    }
}