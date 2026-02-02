using UnityEngine;
using UnityEngine.UI;

public class SettingsButton : MenuButtonBase
{
    public override void Configure(Button button,
        HandlerButtonWindowInteraction handlerButtonWindowInteraction,
        ButtonSoundHandler buttonSound, AudioClip audioClip)
    {
        if (handlerButtonWindowInteraction == null)
        {
            Debug.LogError("HandlerButtonWindowInteraction is отсутствует в конфигурации!");
            return;
        }

        button.onClick.AddListener(() =>
           Press(button, handlerButtonWindowInteraction, buttonSound, audioClip));
    }

    public override void Press(Button button,
        HandlerButtonWindowInteraction handlerButtonWindowInteraction,
        ButtonSoundHandler buttonSound, AudioClip audioClip)
    {
        handlerButtonWindowInteraction.OnButtonClicked(button);
        buttonSound.PlayButtonSound(audioClip);
    }
}