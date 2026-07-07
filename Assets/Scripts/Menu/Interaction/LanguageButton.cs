using UnityEngine;
using UnityEngine.UI;

namespace Menu.Interaction
{
    public class LanguageButton : MonoBehaviour
    {
        [SerializeField] private Image _choice;

        private Button _choiceButton;

        public Button ChoiceButton => _choiceButton;

        private void Awake()
        {
            _choiceButton = GetComponent<Button>();
        }

        public void TurnOffChoice()
        {
            _choice.enabled = false;
        }

        public void TurnOnChoice()
        {
            _choice.enabled = true;
        }
    }
}