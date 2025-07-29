using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class MenuButtonBase : IMenuButton
{
    protected ButtonSoundHandler _soundHandler;
    protected AudioClip _audioClip;

    public virtual void Configure( Button button, HandlerButtonWindowInteraction handlerButtonWindowInteraction, ButtonSoundHandler buttonSound, AudioClip audioClip)
    {
        if (button == null) 
            throw new ArgumentNullException(nameof(button));

        if (handlerButtonWindowInteraction == null) 
            throw new ArgumentNullException(nameof(handlerButtonWindowInteraction));

        _soundHandler = buttonSound ?? throw new ArgumentNullException(nameof(buttonSound));
        _audioClip = audioClip ?? throw new ArgumentNullException(nameof(audioClip));

        button.onClick.AddListener(() =>
          Press(button.name, handlerButtonWindowInteraction, buttonSound, audioClip));

        SetButtonText(button);

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnButtonClicked);
    }

    private void SetButtonText(Button button)
    {
        string text = GetButtonText();

        if (text == null) return;

        TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();

        if (buttonText != null)
        {
            buttonText.text = text;
        }
    }

    public virtual void Press(string name,
       HandlerButtonWindowInteraction handlerButtonWindowInteraction,
       ButtonSoundHandler buttonSound, AudioClip audioClip){ }

    private string GetButtonText() => null;

    private  void OnButtonClicked()
    {
        _soundHandler.PlayButtonSound(_audioClip);
    }
}