using PuzzleResources.Audio;
using UnityEngine;
using UnityEngine.UI;

namespace Menu.Interaction
{
    public interface IMenuButton
    {
        void Configure(Button uiButton,
            HandlerButtonWindowInteraction manager,
            ButtonSoundHandler buttonSoundHandler,
            AudioClip audioClip);
    }
}