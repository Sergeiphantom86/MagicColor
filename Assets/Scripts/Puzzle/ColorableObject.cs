using System;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class ColorableObject : MonoBehaviour, IColorable
{
    private Renderer _renderer;
    private Indicator _indicator;
    private Color _originalColor;
    private bool _isRepainted;

    private void Awake()
    {
        CacheComponents();
        ValidateRenderer();

        SettingRenderingMode(_renderer.material);
        SetAlpha(_renderer.material.color, 0.5f);

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
        _renderer = GetComponent<Renderer>();
        _indicator = GetComponent<Indicator>();
    }

    public void InstallRepainted()
    {
        _isRepainted = true;
    }

    public void SetColor(Color color)
    {
        if (_renderer.material != null)
        {
            if (this is Drop)
            {
                _renderer.material.color = color;
                return;
            }

            SetOriginalColor(color);

            if (_indicator != null)
            {
                _indicator.TurnOnSpriteRenderer();
            }
        }
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

    private void SetOriginalColor(Color color)
    {
        _originalColor = color;
    }

    public void SetAlpha(Color color, float alpha)
    {
        color.a = alpha;

        _renderer.material.color = color;
    }

    public void AssignOriginal()
    {
        if (_isRepainted)
        {
            _renderer.material.color = _originalColor;
            return;
        }

        _renderer.material.color = Color.white;
    }

    public void Disable()
    {
        _renderer.material.color = Color.white;
        SetAlpha(_renderer.material.color, 0.5f);
    }

    private void SettingRenderingMode(Material material)
    {
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.DisableKeyword("_ALPHATEST_ON");
        material.EnableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
    }
}