using System.Collections.Generic;
using UnityEngine;
using YG;

public class QuestSystem : MonoBehaviour
{
    private int _currentQuestIndex;
    private Quest _active;
    private IReadOnlyList<Quest> _quests;
    private QuestCollector _questCollector;
    private TransitionChooser _transitionChooser;
    private List<Quest> _subscribedQuests;

    private void Awake()
    {
        _questCollector = GetComponent<QuestCollector>();
        _transitionChooser = GetComponent<TransitionChooser>();
        _subscribedQuests = new List<Quest>();

        if (_transitionChooser == null)
        {
            Debug.LogError("TransitionChooser not found!");
            return;
        }

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

        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        _questCollector.HasListCreated += Initialize; ;
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
        if (_quests == null || _quests.Count == 0)
        {
            Debug.LogError("Quests list is null or empty");
            return;
        }

        for (int i = 0; i <= GetIndex(); i++)
        {
            _active = _quests[i];

            if (_active.IsUnlocked == false)
            {
                _active.Unlock();
                _active.OnCompleted += OnCompleted;
                _subscribedQuests.Add(_active);
            }
        }

        if (_active != null)
            _active.SetActiveIndicator(true);
    }

    private int GetIndex()
    {
        return Mathf.Clamp(_currentQuestIndex, 0, _quests.Count - 1);
    }

    private void OnCompleted(Quest quest)
    {
        if (_active == quest)
        {
            YG2.saves.SetSimilarity(true);
        }
        else
        {
            YG2.saves.SetSimilarity(false);
        }

        _transitionChooser.ChoosePuzzle(quest);
    }

    private void OnDestroy()
    {
        foreach (var quest in _subscribedQuests)
        {
            if (quest != null)
            {
                quest.OnCompleted -= OnCompleted;
            }
        }
        _subscribedQuests.Clear();
    }
}