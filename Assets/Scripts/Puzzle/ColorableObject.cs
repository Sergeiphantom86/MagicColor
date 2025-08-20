using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class ColorableObject : MonoBehaviour, IColorable
{
    private Renderer _renderer;
    private Indicator _indicator;

    private void Awake()
    {
        CacheComponents();

        ValidateRenderer();

        if (_indicator != null)
        {
            _indicator.TurnOffSpriteRenderer();
        }
    }

    private void OnEnable()
    {
        if (_renderer == null)
        {
            CacheComponents();
        }
    }

    private void CacheComponents()
    {
        _renderer = GetComponentInChildren<Renderer>(true); 
        _indicator = GetComponentInChildren<Indicator>(true);
    }

    public void SetColor(Color color)
    {
        if (_renderer == null)
        {
            Debug.LogError($"Cannot set color - no renderer on {name}", this);
            return;
        }

        if (_renderer.material != null)
        {
            _renderer.material.color = color;

            if (_indicator != null)
            {
                _indicator.TurnOnSpriteRenderer();
            }
        }

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