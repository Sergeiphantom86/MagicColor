using System.Collections;
using UnityEngine;
using YG;

[RequireComponent(typeof(Renderer))]
public class ColorableObject : MonoBehaviour, IColorable
{
    private const string Emission = "_EMISSION";
    private const string EmissionColor = "_EmissionColor";
    private const string EmissionIntensity = "_EmissionIntensity";

    [SerializeField] private bool _isTransparent;

    private Color _originalColor;
    private float _delay;
    private bool _isRepainted;
    private float _valueTransparency;
    private Material _material;
    private Indicator _indicator;
    private Coroutine _coroutine;
    private Renderer _renderer;
    private WaitForSeconds _waitForSeconds;
    private float _fadeDuration;

    public bool IsRepainted => _isRepainted;

    public void InitializeComponents()
    {
        _delay = 10;
        _fadeDuration = 1f;
        _valueTransparency = 0.3f;
        _indicator = GetComponent<Indicator>();
        _waitForSeconds = new WaitForSeconds(_delay);
        InitializeRenderer();
        ValidateRenderer();

        if (_indicator != null)
        {
            _indicator.TurnOffSpriteRenderer();
        }

        _material = _renderer.material;

        //if (_isTransparent)
        //{
        //    SetAlpha( _valueTransparency);
        //}
    }


    public void TurnOffRender()
    {
        _renderer.enabled = false;
    }

    public void TurnOnRender()
    {
        _renderer.enabled = true;
    }

    public void InstallRepainted()
    {
        _isRepainted = true;
    }

    public void SetColor(Color color)
    {
        if (color == null) return;

        InitializeRenderer();
        ValidateRenderer();

        if (_renderer.material != null)
        {
            SetOriginalColor(color);

            if (this is Drop)
            {
                if (_material == null)
                {
                    _material = _renderer.material;
                }

                _material.color = _originalColor;

                return;
            }

            if (_indicator != null)
            {
                _indicator.TurnOnSpriteRenderer();
            }
        }
    }

    private void InitializeRenderer()
    {
        if (_renderer == null)
        {
            _renderer = GetComponent<Renderer>();

            if (_renderer == null) return;
        }
    }

    public void SetActive(bool state) =>
        gameObject.SetActive(state);

    public Color GetColor()
    {
        if (_renderer != null)
        {
            return _material.color;
        }

        return Color.red;
    }

    public void SetRender()
    {
        //_material.renderQueue = 2000;
    }

    public void SetAlpha(float alpha)
    {
        _ = new Color();
        Color color = Color.white;
        color.a = alpha;

        _material.color = color;
    }

    public void AssignOriginal()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
        }

        if (_isRepainted)
        {
            _material.color = _originalColor;
            return;
        }

        if (_material == null)
        {
            Debug.Log("AssignOriginal");
        }

        _material.color = Color.white;
    }

    public void EnableEmission(Color emissionColor, float intensity = 0.01f, float brightness = 0.5f)
    {
        if (_renderer != null && _renderer.material != null)
        {
            _material.EnableKeyword(Emission);

            _material.SetFloat(EmissionIntensity, Mathf.Clamp01(intensity));

            _material.SetColor(EmissionColor, GetDimmedEmissionColor(emissionColor, brightness));
        }
    }

    public void DisableGlow()
    {
        if (_renderer != null && _renderer.material != null)
        {
            _material.DisableKeyword(Emission);

            if (_material.HasProperty(EmissionColor))
            {
                _material.SetColor(EmissionColor, Color.black);
            }
        }
    }

    public void Disable()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
        }

        _coroutine = StartCoroutine(WaitReturn());
    }

    public void TurnOffRenderer()
    {
        StartCoroutine(FadeOutAndDisable());
    }

    private IEnumerator FadeOutAndDisable()
    {
        float time = 0f;

        while (time < _fadeDuration)
        {
            time += Time.deltaTime;
            float t = time / _fadeDuration;
            _originalColor.a = Mathf.Lerp(_originalColor.a, 0f, t);
            _material.color = _originalColor;
            yield return null;
        }

        _originalColor.a = 0f;
        _material.color = _originalColor;

        _renderer.enabled = false;
    }

    protected float GetDistance()
    {
        float fenceHeight = _renderer != null ? _renderer.bounds.size.y : transform.localScale.y;
        
        return fenceHeight;
    }

    private Color GetDimmedEmissionColor(Color color, float brightness)
    {
        return color * Mathf.Clamp01(brightness);
    }

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

    private IEnumerator WaitReturn()
    {
        yield return _waitForSeconds;

        if (YG2.saves.IsTransparency)
        {
            _material.color = Color.white;
            SetAlpha( _valueTransparency);
        }

        _coroutine = null;
    }
}