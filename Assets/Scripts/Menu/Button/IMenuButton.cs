using UnityEngine;
using UnityEngine.UI;

public interface IMenuButton
{
    void Configure(Button uiButton, HandlerButtonWindowInteraction manager, ButtonSoundHandler buttonSoundHandler, AudioClip audioClip);
}