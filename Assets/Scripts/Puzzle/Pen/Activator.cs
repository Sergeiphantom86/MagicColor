using System;
using System.Collections;
using UnityEngine;

public class Activator : MonoBehaviour
{
    [SerializeField] private FragmentSpawner _spawner;
    [SerializeField] private BlocksContainer _blocksContainer;
    [SerializeField] private EffectsHandler _effectsHandler;

    private FragmentQueueProcessor _queueProcessor;
    private IColorPrecision _colorPrecision;
    private int _totalCountPixel;
    private int _remainingPixels;
    private bool _isProcessing;
    private float _transitionReducing;
    private float _duration;

    public event Action OnPuzzleComplete;

    private void Awake()
    {
        _transitionReducing = 0.25f;
        _duration = 0.3f;

        IEffectsHandler effects = _effectsHandler;
        IBlocksContainer blocksContainer = _blocksContainer;

        IMover mover = GetComponent<IMover>();
        IFragmentAnimator animator = GetComponent<IFragmentAnimator>();

        _colorPrecision = new ColorPrecision();
        _queueProcessor = new FragmentQueueProcessor(effects, mover, animator, blocksContainer);

        _queueProcessor.OnFragmentActivated += HandleFragmentActivated;
    }

    private void OnEnable()
    {
        _blocksContainer.StoppingTimer += SpeedFillingProcess;
    }

    private void OnDisable()
    {
        _blocksContainer.StoppingTimer -= SpeedFillingProcess;
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

        CheckPuzzleComplete();
    }

    private void HandleFragmentActivated()
    {
        _remainingPixels--;
        CheckPuzzleComplete();
    }

    private void CheckPuzzleComplete()
    {
        if (_remainingPixels <= 0)
        {
            _effectsHandler.PlayWinSound();
            OnPuzzleComplete?.Invoke();
        }
    }

    private void SpeedFillingProcess(string name)
    {
        _queueProcessor?.RequestSpeedBoost();
    }
}