using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Fragment : MonoBehaviour
{
    private SpriteRenderer _renderer;
    private float _startAlpha;
    private int _finalAlpha;
    private Color _newColor;

    public SpriteRenderer Renderer => _renderer;
    public Color Color { get; private set; }

    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _startAlpha = 0.3f;
        _finalAlpha = 1;
    }

    private void Start()
    {
        Color = _renderer.color;
    }

    public void SetColor(Color color)
    {
        _renderer.color = color;
    }

    public void TurnOnTransparency()
    {
        _newColor = _renderer.color;
        _newColor.a = _startAlpha;
        _renderer.color = _newColor;
    }

    public void TurnOffTransparency()
    {
        _newColor = _renderer.color;
        _newColor.a = _finalAlpha;
        _renderer.color = _newColor;
    }

    public void SetSprite(Sprite sprite)
    {
        if (_renderer != null)
            _renderer.sprite = sprite;
    }
}