using UnityEngine;

[RequireComponent(typeof(Renderer), typeof(ColorableObject))]
public class TouchColorTransparency : MonoBehaviour
{
    private Color _originalColor;
    private Renderer _renderer;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        SetOriginalColor(Color.white);
        SetAlpha(_renderer.material.color, 0.5f);
    }

    public void SetOriginalColor(Color color)
    {
        _originalColor = color;
    }

    public void SetWhiteColor()
    {
        Color color = _renderer.material.color;
        color = Color.white;
        color.a = 0.5f;
        _renderer.material.color = color;
    }

    public void ReturnColor()
    {
        Color color = _renderer.material.color;
        color = _originalColor;
        color.a = 1;
        _renderer.material.color = color;
    }

    public void SetAlpha(Color color, float alpha)
    {
        color.a = alpha;
        _renderer.material.color = color;
    }
}