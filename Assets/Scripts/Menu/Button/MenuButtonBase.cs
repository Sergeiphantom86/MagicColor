using System;
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

        _soundHandler = buttonSound != null ? buttonSound : throw new ArgumentNullException(nameof(buttonSound));
        _audioClip = audioClip != null ? audioClip : throw new ArgumentNullException(nameof(audioClip));

        button.onClick.AddListener(() =>
          Press(button, handlerButtonWindowInteraction, buttonSound, audioClip));

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnButtonClicked);
    }

    public virtual void Press(Button button,
       HandlerButtonWindowInteraction handlerButtonWindowInteraction,
       ButtonSoundHandler buttonSound, AudioClip audioClip){ }

    private  void OnButtonClicked()
    {
        _soundHandler.PlayButtonSound(_audioClip);
    }
}