using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TransitionChooser))]
public class QuestSystem : MonoBehaviour
{
    private bool _isOn;
    private Quest _next;
    private Quest _active;
    private IReadOnlyList<Quest> _quests;
    private List<Quest> _subscribedQuests;
    private TransitionChooser _transitionChooser;
    private int _currentQuestIndex;
    private IProgressSaver _progressSaver;

    private void Awake()
    {
        _transitionChooser = GetComponent<TransitionChooser>();
        _subscribedQuests = new List<Quest>();
        _progressSaver = new ProgressSaver();

        if (_transitionChooser == null)
        {
            Debug.LogError("TransitionChooser not found!");
            return;
        }

        if (_progressSaver.Saves == null)
        {
            Debug.LogError("QuestCollector not found!");
            return;
        }

        _currentQuestIndex = _progressSaver.Saves.QuestIndex;
        _isOn = _progressSaver.Saves.IsAutomaticallyNewLevel;
    }

    public void Initialize(IReadOnlyList<Quest> quests)
    {
        if (quests == null || quests.Count == 0) return;

        _quests = quests;

        ProcessSavedProgress();
    }

    private void ProcessSavedProgress()
    {
        if (IsQuestListValid() == false)
            return;

        UnlockQuestsUpToSavedIndex();
        SetActiveQuestIndicator();
        TryAutoTransition();
    }

    private bool IsQuestListValid()
    {
        if (_quests == null || _quests.Count == 0)
        {
            Debug.LogError("Quests list is null or empty");
            return false;
        }

        return true;
    }

    private void UnlockQuestsUpToSavedIndex()
    {
        int lastIndex = GetIndex();

        for (int i = 0; i <= lastIndex; i++)
        {
            SetupQuest(i);
        }
    }

    private void SetupQuest(int index)
    {
        _active = _quests[index];
        _next = GetNextQuest(index);

        if (_active.IsUnlocked)
            return;

        _active.Unlock();
        SubscribeToQuest(_active);
    }

    private Quest GetNextQuest(int index)
    {
        return (index + 1 < _quests.Count)
            ? _quests[index + 1]
            : null;
    }

    private void SubscribeToQuest(Quest quest)
    {
        quest.OnSelect += OnCompleted;
        _subscribedQuests.Add(quest);
    }

    private void SetActiveQuestIndicator()
    {
        if (_active != null)
            _active.SetActiveIndicator(true);
    }

    private void TryAutoTransition()
    {
        if (_isOn == false || _active == null)
            return;

        _active.OnClicked();
        _progressSaver.SetAutomaticTransition(false);
    }



    private int GetIndex()
    {
        return Mathf.Clamp(_currentQuestIndex, 0, _quests.Count - 1);
    }

    private void OnCompleted(Quest quest)
    {
        if (_active == quest)
        {
            if (_next != null)
            {
                _currentQuestIndex++;
                _progressSaver.SetQuestIndex(_currentQuestIndex);
                _progressSaver.SetNewSprite(_next.Sprite);
            }
        }

        _progressSaver.SaveProgress();

        if (_transitionChooser != null)
        {
            _transitionChooser.ChoosePuzzle(quest);
        }
    }

    private void OnDestroy()
    {
        foreach (var quest in _subscribedQuests)
        {
            if (quest != null)
            {
                quest.OnSelect -= OnCompleted;
            }
        }

        _subscribedQuests.Clear();
    }
}