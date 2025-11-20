using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Fragment : MonoBehaviour
{
    private SpriteRenderer _renderer;
    private float _startAlpha;
    private int _finalAlpha;
    private Color _newColor;
    private Transform _transform;
    private Color _originalColor;

    public SpriteRenderer Renderer => _renderer;

    private void Awake()
    {
        _transform = transform;
        _renderer = GetComponent<SpriteRenderer>();
        _startAlpha = 0.3f;
        _finalAlpha = 1;
        _renderer.sortingOrder = 1;

        if (_renderer != null)
        {
            _originalColor = _renderer.color;
        }
    }

    public void SetColor(Color color)
    {
        _renderer.color = color;
    }

    public void TurnOnTransparency()
    {
        CustomizeColor(_startAlpha);
    }

    public void TurnOffTransparency()
    {
        CustomizeColor(_finalAlpha);
    }

    public void SetSprite(Sprite sprite)
    {
        if (_renderer != null)
            _renderer.sprite = sprite;
    }

    public void SetParent(Transform transform)
    {
        _transform.SetParent(transform);
    }

    public void SetPosition(Vector3 position)
    {
        _transform.position = position;
    }

    public void SetLocalScale(float scale)
    {
        _transform.localScale = Vector3.one * scale;
    }
    public void SetRotation(Quaternion quaternion)
    {
        _transform.rotation = quaternion;
    }

    public Color GetColor()
    {
        return _renderer != null ? _renderer.color : Color.white;
    }

    public void ResetToOriginal()
    {
        SetColor(_originalColor);
    }

    public void SetScale(Vector3 scale)
    {
        _transform.localScale = scale;
    }

    public Vector3 GetPosition()
    {
        return _transform.position;
    }

    private void CustomizeColor(float alpha)
    {
        _newColor = _renderer.color;
        _newColor.a = alpha;
        _renderer.color = _newColor;
    }
}