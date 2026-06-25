using UnityEngine;
namespace PuzzleEditor.MovingBlocks
{

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

    public void SetAlpha(Color color, float alpha)
    {
        color.a = alpha;
        _renderer.material.color = color;
    }
}
}