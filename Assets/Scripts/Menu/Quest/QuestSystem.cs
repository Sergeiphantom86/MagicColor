using System.Collections.Generic;
using UnityEngine;
using YG;

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

    private void Awake()
    {
        _transitionChooser = GetComponent<TransitionChooser>();
        _subscribedQuests = new List<Quest>();

        if (_transitionChooser == null)
        {
            Debug.LogError("TransitionChooser not found!");
            return;
        }

        if (YG2.saves == null)
        {
            Debug.LogError("QuestCollector not found!");
            return;
        }

        _currentQuestIndex = YG2.saves.QuestIndex;
        _isOn = YG2.saves.IsAutomaticallyNewLevel;
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
        YG2.saves.SetAutomaticTransition(false);
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
                YG2.saves.SetQuestIndex(_currentQuestIndex);
                YG2.saves.SetNewSprite(_next.Sprite);
            }
        }

        YG2.SaveProgress();

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