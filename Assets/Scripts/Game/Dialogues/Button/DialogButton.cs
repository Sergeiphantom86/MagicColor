using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class DialogButton : MonoBehaviour
{
    private Button _button;
    
    public event Action<bool> OnChoiceMade;

    private void Awake()
    {
        _button.onClick.AddListener(HandleCancel);
    }

    private void OnDestroy()
    {
        _button.onClick.RemoveListener(HandleCancel);
    }

    private void HandleCancel()
    {
        OnChoiceMade?.Invoke(false);
    }
}