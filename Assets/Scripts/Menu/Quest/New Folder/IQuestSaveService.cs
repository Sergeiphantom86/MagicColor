using System.Collections.Generic;
using UnityEngine;

public interface IQuestSaveService
{
    public int QuestIndex { get; }
    public bool IsAutoTransition { get; }

    public void AdvanceQuest();
    public void SetAutoTransition(bool value);
    public Sprite GetCurrentSprite(IReadOnlyList<Quest> quests);
    public void Save();
}