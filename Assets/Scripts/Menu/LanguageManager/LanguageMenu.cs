using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class LanguageMenu : MonoBehaviour
{
    [SerializeField] private GameObject _choice;
    [SerializeField] private ButtonSoundHandler _buttonSound;
    [SerializeField] private AudioClip _clickSound;

    private Button _button;
    private Vector2 _positionOnFlag;
    private LanguageBar _languageBar;
    private IProgressSaver _progressSaver;

    private void Awake()
    {
        _button = GetComponentInChildren<Button>();
        _languageBar = GetComponent<LanguageBar>();
        _progressSaver = new ProgressSaver();

        if (IsValidState() == false)
        {
            Debug.LogError("Не назначен в испекторе!");
        }

        SetDefaltPositionFlag();

        OnLanguageChanged(_progressSaver.GetTranslationLanguage());
    }

    private void SetDefaltPositionFlag()
    {
        float positionX = 36;
        float positionY = -22;
        
        _positionOnFlag = new Vector2(positionX, positionY);
    }

    private void OnEnable()
    {
        _progressSaver.SubscribeSwitchLang(OnLanguageChanged);

        if (_button == null) 
        {
            Debug.LogError("Не назначен в испекторе!");
        }

        _button.onClick.AddListener(() => 
        ToggleLanguagePanel(_clickSound));

        ClickOnSelectionButton();
    }

    private void OnDestroy()
    {
        _progressSaver.UnsubscribeSwitchLang(OnLanguageChanged); 
    }

    private void ClickOnSelectionButton()
    {
        foreach (Button button in _languageBar.Buttons)
        {
            button.onClick.AddListener(() =>
            ChangeLanguage(button.name.ToLower()));
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
        if (_progressSaver.GetTranslationLanguage() != langCode)
        {
            _progressSaver.SwitchLanguage(langCode);
        }

        TurnOn();
    }

    private void TurnOff()
    {
        _buttonSound.PlayButtonSound(_clickSound);
    }
    private void TurnOn()
    {
        _buttonSound.PlayButtonSound(_clickSound);
    }

    private void OnLanguageChanged(string language)
    {
        ApplyLanguageSelection(FindButtonForLanguage(language));
    }

    private void ApplyLanguageSelection(Button targetButton)
    {
        MoveChoiceToButton(targetButton);
    }

    private Button FindButtonForLanguage(string language)
    {
        return _languageBar.Buttons
        .Where(button => button.name != null)
        .FirstOrDefault(button => button.name
        .Equals(language, StringComparison.OrdinalIgnoreCase));
    }

    private void MoveChoiceToButton(Button targetButton)
    {
        _choice.transform.SetParent(targetButton.transform);
        _choice.transform.localPosition = _positionOnFlag;
    }
}