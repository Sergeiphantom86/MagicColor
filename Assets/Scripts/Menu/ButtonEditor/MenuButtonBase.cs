using PuzzleEditor.SoundEditor;
using System;
using UnityEngine;
using UnityEngine.UI;
namespace Menu.ButtonEditor
{

public abstract class MenuButtonBase : IMenuButton
{
    protected ButtonSoundHandler SoundHandler;
    protected AudioClip AudioClip;

    public virtual void Configure(Button button,
        HandlerButtonWindowInteraction handlerButtonWindowInteraction,
        ButtonSoundHandler buttonSound,
        AudioClip audioClip)
    {
        if (button == null)
        {
            throw new ArgumentNullException(nameof(button));
        }

        if (handlerButtonWindowInteraction == null)
        {
            throw new ArgumentNullException(nameof(handlerButtonWindowInteraction));
        }

        SoundHandler = buttonSound != null ? buttonSound : throw new ArgumentNullException(nameof(buttonSound));
        AudioClip = audioClip != null ? audioClip : throw new ArgumentNullException(nameof(audioClip));

        button.onClick.AddListener(() =>
          Press(button, handlerButtonWindowInteraction, buttonSound, audioClip));

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnButtonClicked);
    }

    public virtual void Press(Button button,
        HandlerButtonWindowInteraction handlerButtonWindowInteraction,
        ButtonSoundHandler buttonSound,
        AudioClip audioClip)
    {
    }

    private void OnButtonClicked()
    {
        SoundHandler.PlayButtonSound(AudioClip);
    }
}
}