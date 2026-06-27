using System;
using System.Collections.Generic;
using Menu.ButtonEditor;
using PuzzleEditor;
using PuzzleEditor.SoundEditor;
using UnityEngine;
using UnityEngine.UI;
using YG;

namespace Menu.LanguageManager
{
    [RequireComponent(typeof(Image))]

    public class LanguageMenu : MonoBehaviour, IActivatable
    {
        [SerializeField] private ButtonSoundHandler _buttonSound;
        [SerializeField] private AudioClip _clickSound;

        private LanguageBar _languageBar;
        private List<LanguageButton> _buttons;
        private bool _uiInitialized;
        private string _title;

        public event Action Initialized;

        private void Awake()
        {
            _languageBar = GetComponent<LanguageBar>();

            SetButtons();

            YG2.saves.CurrentLanguage = YG2.lang;

            FindButtonForLanguage(YG2.lang);
        }

        private void Start()
        {
            if (_uiInitialized == false)
            {
                _uiInitialized = true;
                Initialized?.Invoke();
            }
        }

        private void OnEnable()
        {
            if (_languageBar == null)
            {
                Debug.LogError("LanguageMenu: Не удалось найти языковую панель!");
                return;
            }

            YG2.onSwitchLang += FindButtonForLanguage;

            ClickOnSelectionButton();
        }

        private void OnDisable()
        {
            YG2.onSwitchLang -= FindButtonForLanguage;
        }

        public void Activate() => gameObject.SetActive(true);

        public void Deactivate() => gameObject.SetActive(false);

        private void SetButtons()
        {
            if (_languageBar == null && _languageBar.Buttons.Count > 0)
            {
                Debug.LogError("LanguageBar == null || Buttons.Count <= 0");
                return;
            }

            _buttons = _languageBar.Buttons;
        }

        private void FindButtonForLanguage(string language)
        {
            LanguageButton languageButton = null;

            foreach (LanguageButton button in _buttons)
            {
                button.TurnOffChoice();

                if (button != null && string.Equals(button.name, language, StringComparison.OrdinalIgnoreCase))
                {
                    languageButton = button;
                }
            }

            languageButton.TurnOnChoice();
        }

        private void ClickOnSelectionButton()
        {
            if (_buttons.Count <= 0 && _buttons[0] == null)
                return;

            foreach (LanguageButton button in _buttons)
            {
                if (button == null)
                    continue;

                string lang = button.name.ToLower();

                button.ChoiceButton.onClick.AddListener(() =>
                {
                    _buttonSound.PlayButtonSound(_clickSound);
                    ChangeLanguage(lang);
                });
            }
        }

        private void ChangeLanguage(string langCode)
        {
            if (_title == langCode)
                return;

            YG2.SwitchLanguage(langCode);
            YG2.SaveProgress();

            _title = langCode;
        }
    }
}