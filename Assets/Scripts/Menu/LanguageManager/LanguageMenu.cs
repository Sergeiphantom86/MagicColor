using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class LanguageMenu : MonoBehaviour, IActivatable
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
    private List<Button> _buttons;

    private bool _isInitialized;

    public event Action Initialized;

    private void Awake()
    {
        _progressSaver = new ProgressSaver();
        _languageBar = GetComponent<LanguageBar>();
        _buttons = new List<Button>();
        _flagPositionX = 36;
        _flagPositionY = -22;

        _current = _progressSaver.Saves.CurrentLanguage;

        _positionOnFlag = new Vector2(_flagPositionX, _flagPositionY);

        _buttons = _languageBar.Buttons;
    }

    private void Start()
    {
        ChangeLanguage(_current);
    }

    private void OnEnable()
    {
        if (IsValidState() == false)
        {
            Debug.LogError("Не назначен в испекторе!");
        }

        ClickOnSelectionButton();
    }

    private void ClickOnSelectionButton()
    {
        foreach (Button button in _buttons)
        {
            string lang = button.name.ToLower();

            button.onClick.AddListener(() =>
            {
                ToggleLanguagePanel(_clickSound);
                ChangeLanguage(lang);
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

        if (_isInitialized == false)
        {
            _isInitialized = true;

            Initialized?.Invoke();
        }
    }

    private Button FindButtonForLanguage(string language)
    {
        foreach (Button button in _buttons)
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

    public void Activate()
    {
        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
}