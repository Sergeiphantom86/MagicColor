using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using YG;

[RequireComponent(typeof(Image))]
public class LanguageMenu : MonoBehaviour
{
    [SerializeField] private GameObject _choice;
    [SerializeField] private ButtonSoundHandler _buttonSound;
    [SerializeField] private AudioClip _clickSound;

    private Button _button;
    private Vector2 _positionOnFlag;
    private LanguageBar _languageBar;

    private void Awake()
    {
        _button = GetComponentInChildren<Button>();
        _languageBar = GetComponent<LanguageBar>();

        if (IsValidState() == false)
        {
            Debug.LogError("Не назначен в испекторе!");
        }

        SetDefaltPositionFlag();

        OnLanguageChanged(YG2.lang);
    }

    private void SetDefaltPositionFlag()
    {
        float positionX = 36;
        float positionY = -22;
        
        _positionOnFlag = new Vector2(positionX, positionY);
    }

    private void OnEnable()
    {
        YG2.onSwitchLang += OnLanguageChanged;

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
        YG2.onSwitchLang -= OnLanguageChanged;
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
        if (YG2.lang != langCode)
        {
            YG2.SwitchLanguage(langCode);
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