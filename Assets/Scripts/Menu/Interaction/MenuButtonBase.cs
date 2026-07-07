using System;
using PuzzleResources.Audio;
using UnityEngine;
using UnityEngine.UI;

namespace Menu.Interaction
{
    public abstract class MenuButtonBase : IMenuButton
    {
        private Button _button;
        private HandlerButtonWindowInteraction _handler;
        private ButtonSoundHandler _soundHandler;
        private AudioClip _audioClip;

        public virtual void Configure(Button button,
        HandlerButtonWindowInteraction handlerButtonWindowInteraction,
        ButtonSoundHandler buttonSound,
        AudioClip audioClip)
        {
            if (button == null)
            throw new ArgumentNullException(nameof(button));
            if (handlerButtonWindowInteraction == null)
            throw new ArgumentNullException(nameof(handlerButtonWindowInteraction));
            if (buttonSound == null)
            throw new ArgumentNullException(nameof(buttonSound));
            if (audioClip == null)
            throw new ArgumentNullException(nameof(audioClip));

            _button = button;
            _handler = handlerButtonWindowInteraction;
            _soundHandler = buttonSound;
            _audioClip = audioClip;

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnButtonClick);
        }

        public virtual void Press(Button button,
        HandlerButtonWindowInteraction handlerButtonWindowInteraction,
        ButtonSoundHandler buttonSound,
        AudioClip audioClip)
        {
            handlerButtonWindowInteraction.HandleButtonClick(button);
            buttonSound.PlayButtonSound(audioClip);
        }

        private void OnButtonClick()
        {
            Press(_button, _handler, _soundHandler, _audioClip);
        }
    }
}