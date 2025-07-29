using YG;
using UnityEngine;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(ButtonKeeper))]
public class HandlerButtonWindowInteraction : MonoBehaviour
{
    [SerializeField] private Tutorial _tutorial;
    [SerializeField] private AudioClip _clickSound;

    private ButtonKeeper _buttonKeeper;
    private ButtonSoundHandler _buttonSoundHandler;
    private WindowInitializer _windowInitializer;

    private void Awake()
    {
        _buttonSoundHandler = GetComponent<ButtonSoundHandler>();
        _buttonKeeper = GetComponent<ButtonKeeper>();
        _windowInitializer = GetComponent<WindowInitializer>();

        _windowInitializer.Initialize();

        YG2.StartInit();
    }

    private void Start()
    {
        CreateButtons();
    }

    public void Show(string windowName)
    {
        if (_tutorial != null && _tutorial.IsTutorialActive && !_tutorial.IsClickAllowed)
            return;

        _tutorial?.CompleteClickStep();

        if (_windowInitializer.WindowActions.TryGetValue(windowName, out Action action) == false)
        {
            Debug.LogError($"Unknown window action: {windowName}");
            return;
        }

        action.Invoke();
    }

    private void CreateButtons()
    {
        foreach (Button button in _buttonKeeper.Buttons)
        {
            IMenuButton btn = ButtonFactory.CreateButton(button.name);

            if (btn != null)
            {
                btn.Configure(button, this, _buttonSoundHandler, _clickSound);
            }
            else
            {
                Debug.LogError($"Failed to create button: {button.name}");
            }
        }
    }
}