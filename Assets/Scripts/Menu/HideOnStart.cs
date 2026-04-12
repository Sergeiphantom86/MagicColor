using System.Collections;
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

    private IEnumerator Start()
    {
        // ждём пока LanguageMenu завершит инициализацию
        while (_languageMenu.IsInitialized == false)
        {
            yield return null;
        }

        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
       // _languageMenu.OnInitialized += TurnOff;
    }

    private void OnDisable()
    {
        //_languageMenu.OnInitialized -= TurnOff;
    }

    private void TurnOff()
    {
        gameObject.SetActive(false);
    }
}