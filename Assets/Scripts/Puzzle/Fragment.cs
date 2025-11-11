using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Fragment : MonoBehaviour
{
    private SpriteRenderer _renderer;
    private float _startAlpha;
    private int _finalAlpha;
    private Color _newColor;

    public SpriteRenderer Renderer => _renderer;

    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _startAlpha = 0.3f;
        _finalAlpha = 1;
        _renderer.sortingOrder = 1;
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
        transform.SetParent(transform);
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    public void SetLocalScale(float scale)
    {
        transform.localScale = Vector3.one * scale;
    }
    public void SetRotation(Quaternion quaternion)
    {
        transform.rotation = quaternion;
    }

    private void CustomizeColor(float alpha)
    {
        _newColor = _renderer.color;
        _newColor.a = alpha;
        _renderer.color = _newColor;
    }
}