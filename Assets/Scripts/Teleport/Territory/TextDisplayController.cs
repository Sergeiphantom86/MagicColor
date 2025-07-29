using TMPro;
using UnityEngine;

public class TextDisplayController : MonoBehaviour
{
    private PanelFader _fader;
    private CanvasGroup _canvasGroup;
    private TextMeshProUGUI _nameTerritory;
    private float _transparency;

    private void Awake()
    {
        _transparency = 0.5f;

        _fader = GetComponent<PanelFader>();
        _canvasGroup = GetComponent<CanvasGroup>();
        _nameTerritory = GetComponent<TextMeshProUGUI>();

        if (_fader == null) return;
        if (_canvasGroup == null) return;
        if (_nameTerritory == null) return;

        _canvasGroup.alpha = 0f;
    }

    public void SetText(string text)
    {
        _nameTerritory.text = text;
    }

    public void ShowText()
    {
        _fader.Fade(_transparency, true);
    }

    public void HideText()
    {
        _fader.FadeOut();
    }
}