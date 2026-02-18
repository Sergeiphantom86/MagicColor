using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Renderer))]
public class ColorableObject : MonoBehaviour, IColorable
{
    private const string Emission = "_EMISSION";
    private const string EmissionColor = "_EmissionColor";
    private const string EmissionIntensity = "_EmissionIntensity";

    [SerializeField] private bool _isTransparent;

    private float _time;
    private float _delay;
    private bool _isRepainted;
    private int _renderQueue;
    private int _startRenderQueue;
    private float _fadeDuration;
    private float _valueTransparency;
    private Material _material;
    private Renderer _renderer;
    private Color _originalColor;
    private Indicator _indicator;
    private Coroutine _coroutine;
    private WaitForSeconds _waitForSeconds;
    private IProgressSaver _progressSaver;

    public bool IsRepainted => _isRepainted;

    public void InitializeComponents()
    {
        _delay = 10;
        _fadeDuration = 1f;
        _renderQueue = 4000;
        _valueTransparency = 0.3f;

        _indicator = GetComponent<Indicator>();

        _progressSaver = new ProgressSaver();
        _waitForSeconds = new WaitForSeconds(_delay);

        InitializeRenderer();
        ValidateRenderer();

        if (_indicator != null)
        {
            _indicator.TurnOffSpriteRenderer();
        }

        _material = _renderer.material;

        _startRenderQueue = _renderer.material.renderQueue;
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

        if (_renderer.material == null) return;

        SetOriginalColor(color);

        if (_indicator != null)
        {
            _indicator.TurnOnSpriteRenderer();
        }

        if (this is not Drop) return;

        if (_material == null)
        {
            _material = _renderer.material;
        }

        _material.color = _originalColor;

        return;
    }

    private void InitializeRenderer()
    {
        if (_renderer != null) return;

        _renderer = GetComponent<Renderer>();
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

    public void SetRenderQueue()
    {
        _material.renderQueue = _renderQueue;
    }

    public void ReturnRenderQueue()
    {
        _material.renderQueue = _startRenderQueue;
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

        _material.color = Color.white;
    }

    public void EnableEmission(Color emissionColor, float intensity = 0.01f, float brightness = 0.5f)
    {
        if (_material == null && _material == null) return;

        _material.EnableKeyword(Emission);

        _material.SetFloat(EmissionIntensity, Mathf.Clamp01(intensity));

        _material.SetColor(EmissionColor, GetDimmedEmissionColor(emissionColor, brightness));
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
        while (_time < _fadeDuration)
        {
            _originalColor.a = Mathf.Lerp(_originalColor.a, 0f, GetTime(_time) / _fadeDuration);

            _material.color = _originalColor;

            yield return null;
        }

        _originalColor.a = 0f;
        _material.color = _originalColor;

        _renderer.enabled = false;
    }

    private float GetTime(float time)
    {
        return time += Time.deltaTime;
    }

    private Color GetDimmedEmissionColor(Color color, float brightness)
    {
        return color * Mathf.Clamp01(brightness);
    }

    private void ValidateRenderer()
    {
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

        if (_progressSaver.Saves.IsTransparency)
        {
            _material.color = Color.white;
            SetAlpha(_valueTransparency);
        }

        _coroutine = null;
    }
}