using System;
using System.Collections;
using PuzzleEditor.PenEditor.Placeholder;
using PuzzleEditor.SoundEditor;
using UnityEngine;

namespace PuzzleEditor.PenEditor
{
    public class Activator : MonoBehaviour, IActivatable
    {
        [SerializeField] private AudioClip _winn;
        [SerializeField] private AudioClip _pixelSound;
        [SerializeField] private Transform _transformPenHolder;
        [SerializeField] private BlocksContainer _blocksContainer;
        [SerializeField] private SequentialSpawner _sequentialSpawner;
        [SerializeField] private TextureInitializer _textureInitializer;

        private float _delay;
        private float _duration;
        private float _transitionReducing;
        private bool _isProcessing;
        private int _totalCountPixel;
        private Voiceover _voiceover;
        private WaitForSeconds _delayWait;
        private IColorPrecision _colorPrecision;
        private FragmentQueueProcessor _queueProcessor;
        private PuzzleProgressTracker _progressTracker;
        private FillSpeedController _speedController;

        public event Action PuzzleCompleted;

        public event Action<float> Approached;

        public event Action<Color> ColorHasChanged;

        private void Awake()
        {
            _delay = 3;
            _duration = 0.3f;
            _transitionReducing = 0.295f;
            _delayWait = new WaitForSeconds(_delay);

            _voiceover = GetComponent<Voiceover>();
            _colorPrecision = new ColorPrecision();

            _progressTracker = new PuzzleProgressTracker();
            _speedController = new FillSpeedController();

            IMover mover = GetComponent<IMover>();
            IFragmentAnimator animator = GetComponent<IFragmentAnimator>();

            _queueProcessor = new FragmentQueueProcessor(_voiceover, _pixelSound, mover, animator, _blocksContainer);
        }

        private void OnEnable()
        {
            _queueProcessor.ColorHasChanged += ColorHasChanged;
            _queueProcessor.IncreaseSpeed += OnSpeedIncreaseRequested;
            _queueProcessor.FragmentActivated += _progressTracker.OnFragmentActivated;

            _progressTracker.PuzzleCompleted += OnPuzzleFinished;
        }

        private void OnDisable()
        {
            _queueProcessor.ColorHasChanged -= ColorHasChanged;
            _queueProcessor.IncreaseSpeed -= OnSpeedIncreaseRequested;
            _queueProcessor.FragmentActivated -= _progressTracker.OnFragmentActivated;

            _progressTracker.PuzzleCompleted -= OnPuzzleFinished;
        }

        private void OnDestroy()
        {
            _queueProcessor?.Cleanup();
        }

        public void Activate()
        {
            gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
        }

        public void EnqueueFragments(Color color)
        {
            if (_textureInitializer == null)
                return;

            InitProgressIfNeeded();

            var fragments = _textureInitializer.GetFragmentsByColor(_colorPrecision.Reduce(color));

            _queueProcessor.EnqueueFragments(fragments);

            if (_isProcessing == false)
            {
                StartCoroutine(ProcessingRoutine());
            }
        }

        private void InitProgressIfNeeded()
        {
            if (_totalCountPixel > 0)
                return;

            _totalCountPixel = _textureInitializer.TotalCount;
            _progressTracker.Init(_totalCountPixel);
        }

        private IEnumerator ProcessingRoutine()
        {
            _isProcessing = true;
            yield return new WaitForSeconds(_duration * 4);
            yield return _queueProcessor.ProcessQueueRoutine(_duration, _transitionReducing);

            _isProcessing = false;
        }

        private void OnSpeedIncreaseRequested(float remainingTime)
        {
            StartCoroutine(SpeedRoutine(remainingTime));
        }

        private IEnumerator SpeedRoutine(float remainingTime)
        {
            yield return _delayWait;

            _speedController.TryAccelerate(remainingTime, Approached, _queueProcessor.SpeedUpMovement);
        }

        private void OnPuzzleFinished()
        {
            _voiceover.PlayOneShot(_winn);
            PuzzleCompleted?.Invoke();

            Deactivate();
        }
    }
}