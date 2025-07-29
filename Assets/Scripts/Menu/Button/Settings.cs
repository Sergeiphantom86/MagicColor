using UnityEngine;
using TMPro;
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
           Press(button.name, handlerButtonWindowInteraction, buttonSound, audioClip));
    }

    public override void Press(string name,
        HandlerButtonWindowInteraction handlerButtonWindowInteraction,
        ButtonSoundHandler buttonSound, AudioClip audioClip)
    {
        handlerButtonWindowInteraction.Show(name);
        buttonSound.PlayButtonSound(audioClip);
    }
}