using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class LanguageMenu : MonoBehaviour
{
    [SerializeField] private GameObject _choice;
    [SerializeField] private ButtonSoundHandler _buttonSound;
    [SerializeField] private AudioClip _clickSound;

    private string _current;
    private Vector2 _positionOnFlag;
    private LanguageBar _languageBar;
    private IProgressSaver _progressSaver;
    private float _flagPositionX;
    private float _flagPositionY;

    public bool IsInitialized { get; private set; }

    private void Awake()
    {
        _progressSaver = new ProgressSaver();
        _flagPositionX = 36;
        _flagPositionY = -22;
        _current = _progressSaver.Saves.CurrentLanguage;

        _positionOnFlag = new Vector2(_flagPositionX, _flagPositionY);
    }

    private void Start()
    {
        OnLanguageChanged(_current);
        ChangeLanguage(_current);

        IsInitialized = true;
    }

    private void OnEnable()
    {
        _languageBar = GetComponent<LanguageBar>();

        if (IsValidState() == false)
        {
            Debug.LogError("Не назначен в испекторе!");
        }

        ClickOnSelectionButton();
    }

    private void ClickOnSelectionButton()
    {
        foreach (Button button in _languageBar.Buttons)
        {
            string lang = button.name.ToLower();

            button.onClick.AddListener(() =>
            {
                ChangeLanguage(lang);
                ToggleLanguagePanel(_clickSound);
            });
        }
    }

    private bool IsValidState()
    {
        return _languageBar != null && _choice != null;
    }

    private void ToggleLanguagePanel(AudioClip audioClip)
    {
        _buttonSound.PlayButtonSound(audioClip);

        TurnOff();
    }

    private void ChangeLanguage(string langCode)
    {
        StartCoroutine(ChangeLanguageRoutine(langCode));
    }

    private IEnumerator ChangeLanguageRoutine(string langCode)
    {
        if (_progressSaver.GetTranslationLanguage() != langCode)
        {
            _progressSaver.SwitchLanguage(langCode);
            _progressSaver.Saves.SetCurrentLanguage(langCode);
        }

        yield return null;

        OnLanguageChanged(langCode);
    }

    private void TurnOff()
    {
        _buttonSound.PlayButtonSound(_clickSound);
    }

    private void OnLanguageChanged(string language)
    {
        ApplyLanguageSelection(FindButtonForLanguage(language));
    }

    private void ApplyLanguageSelection(Button targetButton)
    {
        if (targetButton == null)
        {
            Debug.LogWarning($"Language button not found or destroyed");
            return;
        }

        if (_choice == null)
            return;

        _choice.transform.SetParent(targetButton.transform);
        _choice.transform.localPosition = _positionOnFlag;
    }

    private Button FindButtonForLanguage(string language)
    {
        List<Button> buttons = _languageBar.Buttons;

        foreach (Button button in buttons)
        {
            if (button == null)
                continue;

            if (string.Equals(button.name,
                language,
                StringComparison.OrdinalIgnoreCase))
            {
                return button;
            }
        }

        return null;
    }
}