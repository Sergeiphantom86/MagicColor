using System;
using UnityEngine;
using UnityEngine.UI;

public class Quest : MonoBehaviour
{
    [SerializeField] private LockImage _lockImage;
    [SerializeField] private ActiveIndicator _activeIndicator;
    [SerializeField] private bool _isTutorial;

    private int _reward;
    private string _name;
    private bool _isUnlocked;
    private bool _isCompleted;
    private Button _questButton;
    private PuzzleSelector _selector;
    private GameSaveSystem _gameSaveSystem;

    public int Index { get; private set; }
    public bool IsUnlocked => _isUnlocked;
    public bool IsTutorial => _isTutorial;
    public Sprite Sprite => _selector.Sprite;

    public event Action<Quest> OnCompleted;

    private void Awake()
    {
        _reward = 100;
        _questButton = GetComponent<Button>();
        _selector = GetComponentInChildren<PuzzleSelector>();
        _gameSaveSystem = FindAnyObjectByType<GameSaveSystem>();

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

        _gameSaveSystem.CurrentValue = _reward;
        OnCompleted?.Invoke(this);
    }
}