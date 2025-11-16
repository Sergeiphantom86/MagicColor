using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
            Debug.LogError("Image отсутствует!!!");
            return;
        }

        if (_textMeshProUGUI == null)
        {
            Debug.LogError("TextMeshProUGUI отсутствует!!!");
            return;
        }

        TurnOff();
    }

    public void TurnOff()
    {
        _image.gameObject.SetActive(false);
        _textMeshProUGUI.gameObject.SetActive(false);
    }

    public void TurnOn()
    {
        _image.gameObject.SetActive(true);
        _textMeshProUGUI.gameObject.SetActive(true);
        _errorPanel.TurnOn();
    }
}