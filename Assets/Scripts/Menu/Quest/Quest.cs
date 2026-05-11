using System;
using UnityEngine;
using UnityEngine.UI;

public class Quest : MonoBehaviour
{
    [SerializeField] private int _indexPuzzle;
    
    private LockImage _lockImage;
    private ActiveIndicator _activeIndicator;
    private int _reward;
    private bool _isUnlocked;
    private bool _isCompleted;
    private bool _isTutorial;
    private Button _questButton;
    private PuzzleSelector _selector;
    private IProgressSaver _progressSaver;

    public int Index { get; private set; }

    public bool IsUnlocked => _isUnlocked;

    public bool IsTutorial => _isTutorial;

    public Sprite Sprite => _selector.Sprite;

    public event Action<Quest> Selected;

    private void Awake()
    {
        _reward = 20;
        _questButton = GetComponent<Button>();
        _lockImage = GetComponentInChildren<LockImage>();
        _selector = GetComponentInChildren<PuzzleSelector>();
        _activeIndicator = GetComponentInChildren<ActiveIndicator>();
        _progressSaver = new ProgressSaver();

        _questButton.onClick.AddListener(OnClick);
        _isTutorial = true;
        ResetState();
    }

    public void SetIndex(int index)
    {
        Index = index;

        if (index > 0)
        {
            _reward *= index;
        }
    }

    public void SetTutorial(bool isOn)
    {
        _isTutorial = isOn;
    }

    public void ResetState()
    {
        _isCompleted = false;
        _isUnlocked = false;

        UpdateVisualState();
        SetActiveIndicator(false);
    }

    public void Unlock()
    {
        _isUnlocked = true;

        UpdateVisualState();
    }

    public void SetActiveIndicator(bool active)
    {
        if (_activeIndicator != null)
            _activeIndicator.gameObject.SetActive(active);
    }

    private void UpdateVisualState()
    {
        _lockImage.gameObject.SetActive(!_isUnlocked);

        _questButton.interactable = _isUnlocked && _isCompleted == false;
    }

    public void OnClick()
    {
        if (_isUnlocked == false || _isCompleted) return;

        _progressSaver.SetReward(_reward);
        _progressSaver.Saves.IndexPuzzle = _indexPuzzle;

        Selected?.Invoke(this);
    }
}