using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using YG;

public class LanguageMenu : MonoBehaviour
{
    [SerializeField] private LanguageBar _languageBar;
    [SerializeField] private Flag _buttonFlagImage;
    [SerializeField] private GameObject _choice;
    [SerializeField] private ButtonAnimator _animator;
    [SerializeField] private ButtonSoundHandler _buttonSound;
    [SerializeField] private AudioClip _clickSound;

    private Button _button;
    private Image _image;
    private Vector2 _positionOnFlag;

    private void Awake()
    {
        _button = GetComponentInChildren<Button>();

        _image = _buttonFlagImage.GetComponent<Image>();

        if (IsValidState() == false)
        {
            Debug.LogError("Не назначен в испекторе!");
        }

        SetDefaltPositionFlag();

        _languageBar.gameObject.SetActive(false);
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
        return _buttonFlagImage != null && _languageBar != null && _choice != null;
    }

    private void ToggleLanguagePanel(AudioClip audioClip)
    {
        _buttonSound.PlayButtonSound(audioClip);

        TurnOff();
        OpenLanguageBar();
    }

    private void ChangeLanguage(string langCode)
    {
        if (YG2.lang != langCode)
        {
            YG2.SwitchLanguage(langCode);
        }

        TurnOn();
        CloseLanguageBar();
    }

    private void CloseLanguageBar()
    {
        _animator.TurnOff();
        _languageBar.TurnOff();
    }

    private void OpenLanguageBar()
    {
        _animator.TurnOn();
        _languageBar.TurnOn();
    }

    private void TurnOff()
    {
        _buttonSound.PlayButtonSound(_clickSound);
        gameObject.SetActive(false);
    }
    private void TurnOn()
    {
        _buttonSound.PlayButtonSound(_clickSound);
        gameObject.SetActive(true);
    }

    private void OnLanguageChanged(string language)
    {
        ApplyLanguageSelection(FindButtonForLanguage(language));
    }

    private void ApplyLanguageSelection(Button targetButton)
    {
        MoveChoiceToButton(targetButton);
        UpdateSelectionVisual(targetButton);
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

    private void UpdateSelectionVisual(Button targetButton)
    {
        if (targetButton.TryGetComponent<Image>(out var buttonImage))
        {
            _image.sprite = buttonImage.sprite;
        }
    }
}