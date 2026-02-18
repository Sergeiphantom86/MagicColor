using System.Collections.Generic;

public class QuestCustomizer
{
    private readonly IProgressSaver _progressSaver;
    private readonly int _indexTutorialBasics = 0;
    private readonly int _indexAbilityTutorial = 2;
    private readonly int _indexUnblockingTutorial;
    private readonly bool _isTutorialBasics;
    private readonly bool _isUnblockingTutorial;
    private readonly bool _isAbilityTutorial;

    public QuestCustomizer(IProgressSaver progressSaver)
    {
        _progressSaver = progressSaver;

        _indexUnblockingTutorial = _progressSaver.Saves.IndexSecondQuest;

        _isTutorialBasics = _progressSaver.Saves.IsTutorialBasics;
        _isUnblockingTutorial = _progressSaver.Saves.IsUnblockingTutorial;
        _isAbilityTutorial = _progressSaver.Saves.IsAbilityTutorial;
    }

    public void Apply(List<Quest> quests)
    {
        for (int i = 0; i < quests.Count; i++)
        {
            ApplyState(quests[i], i);
        }
    }

    private void ApplyState(Quest quest, int index)
    {
        quest.SetIndex(index);

        SetTutorial(quest, _isTutorialBasics, _indexTutorialBasics);
        SetTutorial(quest, _isUnblockingTutorial, _indexUnblockingTutorial);
        SetTutorial(quest, _isAbilityTutorial, _indexAbilityTutorial);
    }

    private void SetTutorial(Quest quest, bool isTutorial, int index)
    {
        if (quest.Index == index && !isTutorial)
        {
            quest.SetTutorial(false);
        }
    }
}