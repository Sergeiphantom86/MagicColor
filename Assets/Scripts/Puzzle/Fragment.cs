using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Fragment : MonoBehaviour
{
    [SerializeField] private ParticleSystem _particleSystem;
    private SpriteRenderer _renderer;
    private float _startAlpha;
    private int _finalAlpha;
    private Color _newColor;

    public Color Color => _renderer.color;

    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _startAlpha = 0.3f;
        _finalAlpha = 1;

        if (_particleSystem == null)
            _particleSystem = GetComponent<ParticleSystem>();
    }

    public bool IsValid()
    {
        return _renderer != null && _renderer.sprite != null;
    }

    public void SetColor(Color color)
    {
        _renderer.color = color;
    }

    public Color GetColor()
    {
        if (TryGetComponent<SpriteRenderer>(out var renderer))
        {
            return renderer.color;
        }

        return Color.white;
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