using PuzzleEditor.SoundEditor;
using UnityEngine;
using UnityEngine.UI;
namespace Menu.ButtonEditor
{

public class PlayButton : MenuButtonBase
{
    public override void Configure(Button button,
        HandlerButtonWindowInteraction handlerButtonWindowInteraction,
        ButtonSoundHandler buttonSound,
        AudioClip audioClip)
    {
        button.onClick.RemoveAllListeners();

        if (handlerButtonWindowInteraction == null)
        {
            Debug.LogError("HandlerButtonWindowInteraction is ����������� � ������������!");
            return;
        }

        button.onClick.AddListener(() =>
           Press(button, handlerButtonWindowInteraction, buttonSound, audioClip));
    }

    public override void Press(Button button,
        HandlerButtonWindowInteraction handlerButtonWindowInteraction,
        ButtonSoundHandler buttonSound, 
        AudioClip audioClip)
    {
        handlerButtonWindowInteraction.OnButtonClicked(button);
        buttonSound.PlayButtonSound(audioClip);
    }
}
}