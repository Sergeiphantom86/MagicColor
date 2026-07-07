using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Menu.Tutorials.TutorialPuzzle
{
    public class TextSwitcher : MonoBehaviour
    {
        [SerializeField] private Rewards _awardText;

        private TextMeshProUGUI _notificationText;
        private Image _image;

        private void Awake()
        {
            _notificationText = GetComponentInChildren<TextMeshProUGUI>(true);
            _image = GetComponent<Image>();
        }

        public void TurnOffDesiredOne(bool isOn)
        {
            if (isOn == false)
            {
                _image.enabled = false;
                _notificationText.gameObject.SetActive(false);
                _awardText.gameObject.SetActive(true);
                return;
            }

            _image.enabled = true;
            _awardText.gameObject.SetActive(false);
            _notificationText.gameObject.SetActive(true);
        }
    }
}