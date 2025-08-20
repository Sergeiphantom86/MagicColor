using UnityEngine;
using UnityEngine.UI;
using System;

public class ButtonController : MonoBehaviour
{
    [SerializeField] private Button _button;

    private Func<bool> _globalInteractableCondition;
    private Action _onClickAction;
    private bool _localBlock;

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
        _globalInteractableCondition = globalInteractableCondition;
        _onClickAction = onClickAction;
        UpdateState();
    }

    public void SetLocalBlock(bool block)
    {
        _localBlock = block;
        UpdateState();
    }

    public void UpdateState()
    {
        bool isInteractable = _localBlock == false && (_globalInteractableCondition?.Invoke() ?? false);

        _button.interactable = isInteractable;
    }

    private void HandleClick()
    {
        if (_button.interactable && _onClickAction != null)
        {
            _onClickAction.Invoke();
        }
    }

    private void OnDestroy()
    {
        _button.onClick.RemoveListener(HandleClick);
    }
}