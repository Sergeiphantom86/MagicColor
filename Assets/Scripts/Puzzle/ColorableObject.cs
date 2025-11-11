using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class ColorableObject : MonoBehaviour, IColorable
{
    private Renderer _curentRenderer;
    private Indicator _indicator;
    private Color _originalColor;
    private bool _isRepainted;

    public bool IsRepainted => _isRepainted;

    public void InitializeComponents()
    {
        _curentRenderer = GetComponent<Renderer>();

        ValidateRenderer();

        _indicator = GetComponent<Indicator>();

        if (_indicator != null)
        {
            _indicator.TurnOffSpriteRenderer();
        }

        if (_curentRenderer == null)
        {
            Debug.Log($"Renderer = null {this}");
        }

        SettingRenderingMode(_curentRenderer.material);
        SetAlpha(_curentRenderer.material.color, 0.5f);
    }

    public void InstallRepainted()
    {
        _isRepainted = true;
    }

    public void SetColor(Color color)
    {
        if (_curentRenderer.material != null)
        {
            if (this is Drop)
            {
                _curentRenderer.material.color = color;
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
        _curentRenderer.material.color;

    private void ValidateRenderer()
    {
        if (_curentRenderer != null) return;

        if (_curentRenderer == null)
            Debug.LogError($"Renderer not found on {name}", this);
    }

    private void SetOriginalColor(Color color)
    {
        _originalColor = color;
    }

    public void SetAlpha(Color color, float alpha)
    {
        color.a = alpha;

        _curentRenderer.material.color = color;
    }

    public void AssignOriginal()
    {
        if (_isRepainted)
        {
            _curentRenderer.material.color = _originalColor;
            return;
        }

        _curentRenderer.material.color = Color.white;
    }

    public void Disable()
    {
        _curentRenderer.material.color = Color.white;
        SetAlpha(_curentRenderer.material.color, 0.5f);
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