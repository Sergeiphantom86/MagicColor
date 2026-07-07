using PuzzleResources.MinigamesRoulette;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Menu
{
    [RequireComponent(typeof(Image))]

    public class Warner : MonoBehaviour
    {
        [SerializeField] private ErrorPanel _errorPanel;
        [SerializeField] private ButtonController _buttonController;

        private Image _image;
        private TextMeshProUGUI _textMeshProUGUI;

        private void Awake()
        {
            _image = GetComponent<Image>();
            _textMeshProUGUI = GetComponentInChildren<TextMeshProUGUI>();

            if (_image == null)
            {
                Debug.LogError("Image: компонент не найден!!!");
                return;
            }

            if (_textMeshProUGUI == null)
            {
                Debug.LogError("TextMeshProUGUI: компонент не найден!!!");
                return;
            }

            TurnOff();
        }

        public void TurnOff()
        {
            _image.gameObject.SetActive(false);
            _textMeshProUGUI.gameObject.SetActive(false);
        }
    }
}