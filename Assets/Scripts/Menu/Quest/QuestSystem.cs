using System.Collections.Generic;
using UnityEngine;
using YG;

public class QuestSystem : MonoBehaviour
{
    [Header("Quest Settings")]
    [SerializeField] private List<Quest> _allQuests;
    [SerializeField] private Quest _activeQuest;

    private static int _questCount = 0;

    private void Start()
    {
        if (YG2.saves.QuestID >= 0 && YG2.saves.QuestID <= _allQuests.Count) return;
        
        InitializeQuests();
    }

    private void InitializeQuests()
    {
        if (_allQuests.Count == 0) return;
        if (_allQuests.Count < _questCount) return;
        
        for (int i = 0; i <= _questCount; i++)
        {
            _allQuests[i].Unlock();
            SetActiveQuest(_allQuests[i]);
            _allQuests[i].OnCompleted += HandleQuestCompleted;
        }
    }

    private void HandleQuestCompleted(Quest completedQuest)
    {
        if (completedQuest == _activeQuest)
        {
            _questCount = _allQuests.IndexOf(_activeQuest) +1;
        }
    }

    private void SetActiveQuest(Quest quest)
    {
        if (quest == null || _allQuests.Contains(quest) == false || !quest.IsUnlocked) return;

        if (_activeQuest != null)
        {
            _activeQuest.SetActiveIndicator(false);
        }

        _activeQuest = quest;
        _activeQuest.SetActiveIndicator(true);
    }

    private void OnDestroy()
    {
        foreach (var quest in _allQuests)
        {
            if (quest != null) quest.OnCompleted -= HandleQuestCompleted;
        }

        YG2.saves.SetQuestID(_questCount);
        YG2.SaveProgress();
    }
}