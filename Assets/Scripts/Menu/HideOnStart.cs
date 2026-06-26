using Menu.LanguageManager;
using UnityEngine;

namespace Menu
{
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
            _languageMenu.Initialized += OnTurnOff;
        }

        private void OnDisable()
        {
            _languageMenu.Initialized -= OnTurnOff;
        }

        private void OnTurnOff()
        {
            gameObject.SetActive(false);
        }
    }
}