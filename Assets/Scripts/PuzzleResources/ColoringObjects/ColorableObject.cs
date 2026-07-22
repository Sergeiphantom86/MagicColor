using DG.Tweening;
using PuzzleResources.MovingBlocks;
using System.Collections;
using UnityEngine;
using YG;

namespace PuzzleResources.ColoringObjects
{
    [RequireComponent(typeof(Renderer))]

    public class ColorableObject : MonoBehaviour, IColorable, IColorModifiable, IAlphaModifiable, IRepaintable, IRenderQueueConfigurable, IDisable
    {
        [SerializeField] private bool _isTransparent;
        [SerializeField] private float _delaySeconds = 10f;
        [SerializeField] private float _fadeDuration = 1f;
        [SerializeField] private int _renderQueue = 4000;
        [SerializeField] private int _selectedItemRenderQueue = 3001;
        [SerializeField] private float _targetTransparency = 0.6f;

        private bool _isRepainted;
        private Material _material;
        private Renderer _renderer;
        private Coroutine _coroutine;
        private WaitForSeconds _waitForSeconds;

        private MaterialColorController _colorController;
        private EmissionController _emissionController;
        private RenderQueueController _renderQueueController;
        private RendererController _rendererController;
        private TransparencyController _transparencyController;

        public bool IsRepainted => _isRepainted;

        public void InitializeComponents()
        {
            _waitForSeconds = new WaitForSeconds(_delaySeconds);

            _renderer = GetComponent<Renderer>();

            if (_renderer == null)
            {
                Debug.LogError($"Renderer not found on {name}", this);
                return;
            }

            _material = _renderer.material;

            if (_material == null)
            {
                Debug.LogError($"Material not found on {name}", this);
                return;
            }

            CreateComponents();

            if (this is Block)
            {
                _renderQueueController.SaveStartQueue();
            }
        }

        private void CreateComponents()
        {
            if (_colorController != null)
                return;

            _colorController = new MaterialColorController(_material);
            _emissionController = new EmissionController(_material);
            _renderQueueController = new RenderQueueController(_material);
            _rendererController = new RendererController(_renderer);

            _transparencyController = new TransparencyController(
                _material,
                _fadeDuration,
                _targetTransparency);
        }

        public void TurnOffRender() =>
            _rendererController.Hide();

        public void TurnOnRender() =>
            _rendererController.Show();

        public void InstallRepainted() =>
            _isRepainted = true;

        public void SetStartRenderQueueSelectedItem() =>
            _renderQueueController.Restore();

        public void SetOriginalColor(Color color) =>
            _colorController.SetOriginalColor(color);

        public void SetColor(Color color) =>
           _colorController.SetColor(color);

        public void SetActive(bool state) =>
            gameObject.SetActive(state);

        public Color GetColor() =>
            _colorController.CurrentColor;

        public void SetRenderQueue() =>
            _renderQueueController.Set(_renderQueue);

        public void SetRenderQueueSelectedItem() =>
            _renderQueueController.Set(_selectedItemRenderQueue);

        public void SetAlpha(float alpha) =>
            _transparencyController.SetAlpha(alpha);

        public void EnableEmission(Color color, float intensity = 0.01f, float brightness = 0.5f) =>
           _emissionController.Enable(color, intensity, brightness);

        public void AssignOriginal()
        {
            StopRecoloringCoroutine();

            _colorController.Restore(_isRepainted);
        }

        public void Disable()
        {
            StopRecoloringCoroutine();

            _coroutine = StartCoroutine(WaitReturn());
        }

        public Tween TurnOffRenderer()
        {
            return _transparencyController.FadeTo(_colorController.OriginalColor);
        }

        private void StopRecoloringCoroutine()
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
            }
        }

        private IEnumerator WaitReturn()
        {
            yield return _waitForSeconds;

            if (YG2.saves.IsTransparency)
            {
                _material.color = Color.white;

                SetAlpha(_targetTransparency);
            }

            _coroutine = null;
        }
    }
}