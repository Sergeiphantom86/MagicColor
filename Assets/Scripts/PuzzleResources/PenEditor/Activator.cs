using System;
using System.Collections;
using PuzzleResources.PenEditor.Placeholder;
using PuzzleResources.Audio;
using UnityEngine;

namespace PuzzleResources.PenEditor
{
    public class Activator : MonoBehaviour, IActivatable
    {
        private const float Delay = 3;
        private const float Duration = 0.3f;
        private const float TransitionReducing = 0.295f;
        private const float DelayBeforeProcessing = 1.2f;

        [SerializeField] private AudioClip _winn;
        [SerializeField] private AudioClip _pixelSound;
        [SerializeField] private Transform _transformPenHolder;
        [SerializeField] private BlocksContainer _blocksContainer;
        [SerializeField] private SequentialSpawner _sequentialSpawner;
        [SerializeField] private TextureInitializer _textureInitializer;

        private bool _isProcessing;
        private int _totalCountPixel;
        private Voiceover _voiceover;
        private WaitForSeconds _delayWait;
        private IColorPrecision _colorPrecision;
        private FragmentQueueProcessor _queueProcessor;
        private PuzzleProgressTracker _progressTracker;
        private FillSpeedController _speedController;
        private WaitForSeconds _waitBeforeProcessing;

        public event Action PuzzleCompleted;

        public event Action<float> Approached;

        public event Action<Color> ColorHasChanged;

        private void Awake()
        {
            _delayWait = new WaitForSeconds(Delay);
            _waitBeforeProcessing = new WaitForSeconds(DelayBeforeProcessing);

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
            _queueProcessor.SpeedIncreased += OnSpeedIncreaseRequested;
            _queueProcessor.FragmentActivated += _progressTracker.OnFragmentActivated;

            _progressTracker.PuzzleCompleted += OnPuzzleFinished;
        }

        private void OnDisable()
        {
            _queueProcessor.ColorHasChanged -= ColorHasChanged;
            _queueProcessor.SpeedIncreased -= OnSpeedIncreaseRequested;
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
            yield return _waitBeforeProcessing;
            yield return _queueProcessor.ProcessQueueRoutine(Duration, TransitionReducing);

            _isProcessing = false;
        }

        private IEnumerator SpeedRoutine(float remainingTime)
        {
            yield return _delayWait;

            _speedController.Increase(remainingTime, Approached, _queueProcessor.SpeedUpMovement);
        }

        private void OnSpeedIncreaseRequested(float remainingTime)
        {
            StartCoroutine(SpeedRoutine(remainingTime));
        }

        private void OnPuzzleFinished()
        {
            _voiceover.PlayOneShot(_winn);
            PuzzleCompleted?.Invoke();

            Deactivate();
        }
    }
}