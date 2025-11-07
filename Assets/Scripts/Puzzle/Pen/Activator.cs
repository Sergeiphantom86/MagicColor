using System;
using System.Collections;
using UnityEngine;

public class Activator : MonoBehaviour
{
    [SerializeField] private FragmentSpawner _spawner;
    [SerializeField] private BlocksContainer _blocksContainer;
    [SerializeField] private SequentialSpawner _sequentialSpawner;
    [SerializeField] private AudioClip _pixelActivation;
    [SerializeField] private AudioClip _winn;

    private FragmentQueueProcessor _queueProcessor;
    private Voiceover _voiceover;
    private IColorPrecision _colorPrecision;
    private int _totalCountPixel;
    private int _remainingPixels;
    private bool _isProcessing;
    private float _transitionReducing;
    private float _duration;
    private bool _isAccelerated;
    private WaitForSeconds _forSeconds;
    private float _delay;

    public event Action OnPuzzleComplete;

    private void Awake()
    {
        _transitionReducing = 0.25f;
        _delay = 2;
        _duration = 0.3f;
        IBlocksContainer blocksContainer = _blocksContainer;
        IMover mover = GetComponent<IMover>();
        IFragmentAnimator animator = GetComponent<IFragmentAnimator>();
        _voiceover = GetComponent<Voiceover>();
        _forSeconds = new WaitForSeconds(_delay);

        _colorPrecision = new ColorPrecision();
        _queueProcessor = new FragmentQueueProcessor(_voiceover, _pixelActivation, mover, animator, blocksContainer);

        _queueProcessor.OnFragmentActivated += HandleFragmentActivated;
    }

    private void OnEnable()
    {
        _queueProcessor.OnIncreaseSpeed += SpeedFillingProcess;
    }

    private void OnDisable()
    {
        _queueProcessor.OnIncreaseSpeed -= SpeedFillingProcess;
    }

    private void OnDestroy()
    {
        if (_queueProcessor != null)
        {
            _queueProcessor.OnFragmentActivated -= HandleFragmentActivated;
            _queueProcessor.Cleanup();
        }
    }

    public void EnqueueFragments(Color color)
    {
        if (_spawner == null) return;

        if (_totalCountPixel == 0)
        {
            _totalCountPixel = _spawner.TotalCount;
            _remainingPixels = _totalCountPixel;
        }

        var fragments = _spawner.GetFragmentsByColor(_colorPrecision.Reduce(color));

        _queueProcessor.EnqueueFragments(fragments);
        _sequentialSpawner.SpawnObject(color);

        if (_isProcessing == false)
        {
            StartCoroutine(ProcessingRoutine());
        }
    }

    private IEnumerator ProcessingRoutine()
    {
        _isProcessing = true;

        yield return _queueProcessor.ProcessQueueRoutine(transform.position, _duration, _transitionReducing);

        _isProcessing = false;
    }

    private void HandleFragmentActivated()
    {
        _remainingPixels--;
        CheckPuzzleComplete();
        _sequentialSpawner.Reduce();
    }

    private void CheckPuzzleComplete()
    {
        if (_remainingPixels <= 0)
        {
            _voiceover.PlaySfx(_winn);
            OnPuzzleComplete?.Invoke();
        }
    }

    private void SpeedFillingProcess()
    {
        if (_isAccelerated == false)
        {
            _isAccelerated = true;
            StartCoroutine(Wait());
        }
    }

    private IEnumerator Wait()
    {
        yield return _forSeconds;

        _queueProcessor.SpeedUpMovement();
    }
}