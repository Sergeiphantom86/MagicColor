using UnityEngine;

public class HideOnStart : MonoBehaviour
{
    private LanguageMenu _languageMenu;

    private void Awake()
    {
        _languageMenu = GetComponentInChildren<LanguageMenu>();

        if (_languageMenu == null)
        {
            Debug.LogError($"{nameof(LanguageMenu)} == null", this);
            return;
        }
    }

    private void OnEnable()
    {
        _languageMenu.Initialized += TurnOff;
    }

    private void OnDisable()
    {
        _languageMenu.Initialized -= TurnOff;
    }

    private void TurnOff()
    {
        gameObject.SetActive(false);
    }
}