using UnityEngine;
using UnityEngine.UI;
using System;

public class ButtonController : MonoBehaviour
{
    [SerializeField] private Button _button;

    private bool _localBlock;
    private bool _isSpin;
    public event Func<bool> GlobalInteractableCondition;
    public event Action OnTurned;

    public bool IsSpin => _isSpin;

    private void Awake()
    {
        if (_button == null)
        {
            Debug.LogError("Button не назначен!!!");
        }

        _button.onClick.AddListener(HandleClick);
    }

    public void Initialize(Func<bool> globalInteractableCondition, Action onClickAction = null)
    {
        GlobalInteractableCondition = globalInteractableCondition;
        OnTurned = onClickAction;

        UpdateState();
    }

    public void SetLocalBlock(bool block)
    {
        _localBlock = block;

        UpdateState();
    }

    public void UpdateState()
    {
        bool isInteractable = _localBlock == false && (GlobalInteractableCondition?.Invoke() ?? false);

        _button.interactable = isInteractable;
    }

    private void HandleClick()
    {
        if (_button.interactable)
        {
            OnTurned?.Invoke();
            _isSpin = true;
        }
    }
}