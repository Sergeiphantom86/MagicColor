using System;
using UnityEngine;
using UnityEngine.UI;

public class Quest : MonoBehaviour
{
    [Header("Quest Settings")]
    [SerializeField] private int _questID;
    [SerializeField] private string _questName;
    [TextArea][SerializeField] private string _questDescription;

    [Header("Puzzle Unlock")]
    [SerializeField] private string _unlockPuzzleId;

    [SerializeField] private LockImage _lockImage;
    [SerializeField] private ActiveIndicator _activeIndicator;
    [SerializeField] private Button _questButton;

    private bool _isCompleted;
    private bool _isUnlocked;

    public bool IsUnlocked => _isUnlocked;

    public event Action<Quest> OnCompleted;

    private void Awake()
    {
        TryGetComponent(out _questButton);

        _questButton.onClick.AddListener(OnQuestClicked);

        ResetState();
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

    private void OnQuestClicked()
    {
        if (_isUnlocked == false || _isCompleted) return;

        OnCompleted?.Invoke(this);
    }
}