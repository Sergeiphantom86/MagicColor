using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Fragment : MonoBehaviour
{
    private Color _newColor;
    private int _finalAlpha;
    private float _startAlpha;
    private Transform _transform;
    private SpriteRenderer _renderer;

    public SpriteRenderer Renderer => _renderer;

    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();

        _finalAlpha = 1;
        _startAlpha = 0.3f;

        _transform = transform;
    }

    public void SetColor(Color color)
    {
        _renderer.color = color;
    }

    public Color GetColor()
    {
        return _renderer.color;
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

    public void TurnOff()
    {
        gameObject.SetActive(false);
    }

    public void TurnOn()
    {
        gameObject.SetActive(true);
    }

    private void CustomizeColor(float alpha)
    {
        _newColor = _renderer.color;
        _newColor.a = alpha;
        _renderer.color = _newColor;
    }
}