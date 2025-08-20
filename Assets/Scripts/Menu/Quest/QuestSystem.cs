using System.Collections.Generic;
using UnityEngine;
using YG;

public class QuestSystem : MonoBehaviour
{
    private int _currentQuestIndex;
    private Quest _active;
    private IReadOnlyList<Quest> _quests;
    private QuestCollector _questCollector;

    private void Awake()
    {
        _questCollector = GetComponent<QuestCollector>();

        if (_questCollector == null)
        {
            Debug.LogError("QuestCollector not found!");
            return;
        }

        if (YG2.saves == null)
        {
            Debug.LogError("QuestCollector not found!");
            return;
        }
  
        _currentQuestIndex = YG2.saves.QuestIndex;
    }

    private void OnEnable()
    {
        _questCollector.HasListCreated += Initialize;;
    }

    private void OnDisable()
    {
        _questCollector.HasListCreated -= Initialize;
    }

    private void Initialize(IReadOnlyList<Quest> quests)
    {
        if (quests == null || quests.Count == 0) return;

        _quests = quests;
        SetNextIndex();
        ProcessSavedProgress();
    }

    private void SetNextIndex()
    {
        if (YG2.saves.Complete && YG2.saves.IsSimilar)
        {
            _currentQuestIndex++;

            YG2.saves.SetQuestIndex(_currentQuestIndex);
        }
    }

    private void ProcessSavedProgress()
    {
        for (int i = 0; i <= _currentQuestIndex; i++)
        {
            _active = _quests[i];

            if (_active.IsUnlocked == false)
            {
                _active.Unlock();
                _active.OnCompleted += OnCompleted;
            }
        }

        _active.SetActiveIndicator(true);
    }

    private void OnCompleted(Quest quest)
    {
        if (_active == quest)
        {
            YG2.saves.SetSimilarity(true);
            
            return;
        }

        YG2.saves.SetSimilarity(false);
    }

    private void OnDestroy()
    {
        for (int i = 0; i <= _currentQuestIndex; i++)
        {
            _active.OnCompleted -= OnCompleted;
        }
    }
}