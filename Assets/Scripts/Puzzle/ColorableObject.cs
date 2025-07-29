using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class ColorableObject : MonoBehaviour, IColorable
{
    private Renderer _renderer;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();

        ValidateRenderer();
    }

    public void SetColor(Color color)
    {
        if (_renderer == null)
        {
            Debug.LogError($"Cannot set color - no renderer on {name}", this);
            return;
        }

        if (_renderer.material != null)
            _renderer.material.color = color;
        else
            Debug.LogError($"Material missing on {name}'s renderer", this);
    }

    public void SetActive(bool state) => 
        gameObject.SetActive(state);

    public Color GetColor() => 
        _renderer.material.color;

    private void ValidateRenderer()
    {
        if (_renderer != null) return;

        if (_renderer == null)
            Debug.LogError($"Renderer not found on {name}", this);
    }
}