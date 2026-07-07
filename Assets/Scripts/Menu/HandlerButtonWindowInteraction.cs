using Menu.Interaction;
using Menu.Tutorials;
using PuzzleEditor.Audio;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using YG;

namespace Menu
{
    [RequireComponent(typeof(ButtonKeeper), typeof(ButtonSoundHandler), typeof(WindowInitializer))]

    public class HandlerButtonWindowInteraction : MonoBehaviour
    {
        [SerializeField] private Tutorial _tutorial;
        [SerializeField] private AudioClip _clickSound;

        private ButtonKeeper _buttonKeeper;
        private ButtonSoundHandler _buttonSoundHandler;
        private WindowInitializer _windowInitializer;
        private ICarousel _carousel;
        private WaitForSeconds _waitForSeconds;

        private void Awake()
        {
            _buttonSoundHandler = GetComponent<ButtonSoundHandler>();
            _buttonKeeper = GetComponent<ButtonKeeper>();
            _windowInitializer = GetComponent<WindowInitializer>();
            _carousel = GetComponent<ICarousel>();
            _waitForSeconds = new WaitForSeconds(_carousel.ScrollDuration);

            _windowInitializer.Initialize();

            YG2.StartInit();
        }

        private void Start()
        {
            CreateButtons();
        }

        public void HandleButtonClick(Button button)
        {
            int buttonIndex = GetButtonIndex(button);
            string windowName = button.name;

            if (buttonIndex == -1)
            {
                Debug.LogError("Button not found in ButtonKeeper!");
                return;
            }

            if (buttonIndex != _carousel.CurrentIndex)
            {
                StartCoroutine(WaitScroll(windowName));
                _carousel.ScrollToButton(buttonIndex);
                return;
            }

            ExecuteButtonAction(windowName);
        }

        private IEnumerator WaitScroll(string windowName)
        {
            yield return _waitForSeconds;
            ExecuteButtonAction(windowName);
        }

        private void ExecuteButtonAction(string windowName)
        {
            _tutorial.CompleteClickStep();

            if (string.IsNullOrEmpty(windowName))
            {
                Debug.LogError("Window name is null or empty!");
                return;
            }

            if (_windowInitializer.WindowActions.TryGetValue(windowName, out Action action) == false)
            {
                Debug.LogError($"Unknown window action: {windowName}");
                return;
            }

            action.Invoke();
        }

        private int GetButtonIndex(Button button)
        {
            for (int i = 0; i < _buttonKeeper.Buttons.Length; i++)
            {
                if (_buttonKeeper.Buttons[i] == button)
                return i;
            }

            return -1;
        }

        private void CreateButtons()
        {
            foreach (Button button in _buttonKeeper.Buttons)
            {
                IMenuButton menuButton = ButtonFactory.CreateButton(button.name);

                if (menuButton != null)
                {
                    menuButton.Configure(button, this, _buttonSoundHandler, _clickSound);
                }
                else
                {
                    Debug.LogError($"Failed to create button: {button.name}");
                }
            }

            _tutorial.SetPositionButton(_buttonKeeper.Buttons[0].transform.position);
        }
    }
}