using System.Collections;
using UnityEngine;
using DG.Tweening;
using PuzzleEditor.Walls;
using Game.SaveEditor;
using PuzzleEditor.MovingBlocks.BlockEditor;
using PuzzleEditor.InkEditor;

namespace PuzzleEditor
{
    [RequireComponent(typeof(Renderer))]
    public class ColorableObject : MonoBehaviour, IColorable
    {
        private const string Emission = "_EMISSION";
        private const string EmissionColor = "_EmissionColor";
        private const string EmissionIntensity = "_EmissionIntensity";

        [SerializeField] private bool _isTransparent;

        private float _delay;
        private bool _isRepainted;
        private int _renderQueue;
        private int _startRenderQueue;
        private int _selectedItemRenderQueue;
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
            _selectedItemRenderQueue = 3001;
            _valueTransparency = 0.6f;

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

            if (this is Block)
            {
                _startRenderQueue = _material.renderQueue;
            }
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

        public void SetStartRenderQueueSelectedItem()
        {
            _material.renderQueue = _startRenderQueue;
        }

        public void SetColor(Color color)
        {
            if (_renderer.material == null)
                return;

            SetOriginalColor(color);

            if (_indicator != null)
            {
                _indicator.TurnOnSpriteRenderer();
            }

            if (this is not Drop)
                return;

            if (_material == null)
            {
                _material = _renderer.material;
            }

            _material.color = _originalColor;

            return;
        }

        private void InitializeRenderer()
        {
            if (_renderer != null)
                return;

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

        public void SetRenderQueueSelectedItem()
        {
            _material.renderQueue = _selectedItemRenderQueue;
        }

        public void SetAlpha(float alpha)
        {
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
            if (_material == null && _material == null)
                return;

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

        public Tween TurnOffRenderer()
        {
            if (_renderer == null || _material == null)
                return null;

            _renderer.enabled = true;

            Color startColor = _material.color;
            startColor.a = 1f;
            _material.color = startColor;

            return DOTween.To(
                    () => _material.color,
                    color => _material.color = color,
                    new Color(_originalColor.r, _originalColor.g, _originalColor.b, _valueTransparency),
                    _fadeDuration)
                .SetEase(Ease.Linear);
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
}