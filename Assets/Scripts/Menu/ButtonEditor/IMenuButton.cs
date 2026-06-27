using PuzzleEditor.SoundEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Menu.ButtonEditor
{
    public interface IMenuButton
    {
        void Configure(Button uiButton,
            HandlerButtonWindowInteraction manager,
            ButtonSoundHandler buttonSoundHandler,
            AudioClip audioClip);
    }
}