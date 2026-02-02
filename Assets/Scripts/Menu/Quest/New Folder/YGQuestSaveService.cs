using System.Collections.Generic;
using UnityEngine;
using YG;

public class YGQuestSaveService : IQuestSaveService
{
    public int QuestIndex => YG2.saves.QuestIndex;
    public bool IsAutoTransition => YG2.saves.IsAutomaticallyNewLevel;

    public void AdvanceQuest()
    {
        YG2.saves.SetQuestIndex(YG2.saves.QuestIndex + 1);
    }

    public void SetAutoTransition(bool value)
    {
        YG2.saves.SetAutomaticTransition(value);
    }

    public Sprite GetCurrentSprite(IReadOnlyList<Quest> quests)
    {
        int index = Mathf.Clamp(QuestIndex, 0, quests.Count - 1);
        return quests[index].Sprite;
    }

    public void Save()
    {
        YG2.SaveProgress();
    }
}