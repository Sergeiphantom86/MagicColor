using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class Quest : MonoBehaviour
{
    [TextArea][SerializeField] private string _questDescription;

    [Header("Puzzle Unlock")]
    [SerializeField] private string _unlockPuzzleId;

    [SerializeField] private LockImage _lockImage;
    [SerializeField] private ActiveIndicator _activeIndicator;
    [SerializeField] private Button _questButton;

    private bool _isCompleted;
    private bool _isUnlocked;
    private int _reward;
    private PuzzleSelector _selector;
    private GameSaveSystem _gameSaveSystem;

    public int Index {  get; private set; }
    public string QuestName { get; private set; }

    public bool IsUnlocked => _isUnlocked;

    public bool IsCompleted => _isCompleted;

    public event Action<Quest> OnCompleted;

    private void Awake()
    {
        _reward = 100;
        _selector = GetComponentInChildren<PuzzleSelector>();
        _gameSaveSystem = FindAnyObjectByType<GameSaveSystem>();
       
        TryGetComponent(out _questButton);
        
        _questButton.onClick.AddListener(OnClicked);

        ResetState();
    }

    private void Start()
    {
        if (_selector != null)
        {
            QuestName = _selector.GetName();
        }
    }

    public void SetIndex(int index)
    {
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