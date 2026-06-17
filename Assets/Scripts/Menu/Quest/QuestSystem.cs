using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TransitionChooser))]
public class QuestSystem : MonoBehaviour
{
    [SerializeField] private Button _button;

    private Quest _next;
    private Quest _active;
    private ZoomChanger _zoomChanger;
    private List<Quest> _subscribedQuests;
    private IReadOnlyList<Quest> _quests;
    private IProgressSaver _progressSaver;
    private TransitionChooser _transitionChooser;
    private SpriteTransmitter _spriteTransmitter;
    private bool _isActivate;
    private bool _isOn;

    private void Awake()
    {
        _transitionChooser = GetComponent<TransitionChooser>();
        _subscribedQuests = new List<Quest>();
        _zoomChanger = new ZoomChanger();

        if (_transitionChooser == null)
        {
            Debug.LogError("TransitionChooser not found!");
            return;
        }
    }

    public void Initialize(IReadOnlyList<Quest> quests, IProgressSaver progressSaver, SpriteTransmitter spriteTransmitter)
    {
        if (quests == null || quests.Count == 0)
            return;

        if (progressSaver.Saves == null)
        {
            Debug.LogError("IProgressSaver not found!");
            return;
        }

        if (spriteTransmitter == null)
        {
            Debug.LogError("SpriteTransmitter not found!");
            return;
        }

        _spriteTransmitter = spriteTransmitter;
        _progressSaver = progressSaver;
        _quests = quests;

        _isOn = spriteTransmitter.IsAutomaticallyNewLevel;

        _transitionChooser.Initialize(progressSaver, _zoomChanger, spriteTransmitter);

        ProcessSavedProgress();

        if (_isActivate)
        {
            _button.onClick.Invoke();
            return;
        }

        TurnOff();
    }

    private void TurnOff()
    {
        gameObject.SetActive(false);
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

        _next = GetNextQuest(lastIndex);
    }

    private void SetupQuest(int index)
    {
        _active = _quests[index];

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
        quest.Selected += OnSelect;
        _subscribedQuests.Add(quest);
    }

    private void SetActiveQuestIndicator()
    {
        if (_active != null)
            _active.SetActiveIndicator(true);
    }

    public void TryAutoTransition()
    {
        if (_isOn == false || _active == null)
            return;
        _isActivate = true;
        _active.OnClick();
    }

    private int GetIndex()
    {
        return Mathf.Clamp(_progressSaver.Saves.MaxReachedQuestIndex, 0, _quests.Count - 1);
    }

    private void OnSelect(Quest quest)
    {
        if (_active == quest)
        {
            if (_next != null)
            {
                _spriteTransmitter.SetNew(_next.Sprite);
            }
        }

        _progressSaver.SetQuestIndex(quest.Index);

        if (_transitionChooser != null)
        {
            _transitionChooser.ChoosePuzzle(quest, _isOn);
            _spriteTransmitter.SetAutomaticTransition(false);
        }
    }

    private void OnDestroy()
    {
        foreach (var quest in _subscribedQuests)
        {
            if (quest != null)
            {
                quest.Selected -= OnSelect;
            }
        }

        _isActivate = false;
        _subscribedQuests.Clear();
        _progressSaver.SaveProgress();
    }
}