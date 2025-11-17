using System;
using UnityEngine;
using UnityEngine.UI;
using YG;

public class Quest : MonoBehaviour
{
    private LockImage _lockImage;
    private ActiveIndicator _activeIndicator;
    private bool _isTutorial;
    private int _reward;
    private string _name;
    private bool _isUnlocked;
    private bool _isCompleted;
    private Button _questButton;
    private PuzzleSelector _selector;

    public int Index { get; private set; }
    public bool IsUnlocked => _isUnlocked;
    public bool IsTutorial => _isTutorial;
    public Sprite Sprite => _selector.Sprite;

    public event Action<Quest> OnCompleted;

    private void Awake()
    {
        _reward = 100;
        _questButton = GetComponent<Button>();

        _lockImage = GetComponentInChildren<LockImage>();
        _selector = GetComponentInChildren<PuzzleSelector>();
        _activeIndicator = GetComponentInChildren<ActiveIndicator>();

        _questButton.onClick.AddListener(OnClicked);

        ResetState();
    }

    private void Start()
    {
        if (_selector != null)
        {
            _name = _selector.Name;
        }
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

    public void Complete()
    {
        if (_isUnlocked == false || _isCompleted) return;

        _isCompleted = true;
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

    private void OnClicked()
    {
        if (_isUnlocked == false || _isCompleted) return;

        //_gameSaveSystem.CurrentValue = _reward;
        YG2.saves.SetCurrentValue(_reward);
        OnCompleted?.Invoke(this);
    }
}