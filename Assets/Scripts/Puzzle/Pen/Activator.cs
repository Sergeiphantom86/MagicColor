using System;
using System.Collections;
using UnityEngine;

public class Activator : MonoBehaviour
{
    [SerializeField] private AudioClip _winn;
    [SerializeField] private AudioClip _pixelActivation;
    [SerializeField] private Transform _transformPenHolder;
    [SerializeField] private BlocksContainer _blocksContainer;
    [SerializeField] private SequentialSpawner _sequentialSpawner;
    [SerializeField] private TextureInitializer _textureInitializer;

    private float _delay;
    private float _duration;
    private bool _isProcessing;
    private bool _isAccelerated;
    private int _remainingPixels;
    private int _totalCountPixel;
    private float _transitionReducing;
    private Voiceover _voiceover;
    private WaitForSeconds _forSeconds;
    private IColorPrecision _colorPrecision;
    private FragmentQueueProcessor _queueProcessor;

    public event Action OnPuzzleComplete;

    private void Awake()
    {
        _delay = 2;
        _duration = 0.3f;
        _transitionReducing = 0.25f;

        _forSeconds = new WaitForSeconds(_delay);
        _voiceover = GetComponent<Voiceover>();
        IMover mover = GetComponent<IMover>();
        IBlocksContainer blocksContainer = _blocksContainer;
        IFragmentAnimator animator = GetComponent<IFragmentAnimator>();

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
        _queueProcessor.OnFragmentActivated -= HandleFragmentActivated;
    }

    private void OnDestroy()
    {
        _queueProcessor?.Cleanup();
    }

    public void EnqueueFragments(Color color)
    {
        if (_textureInitializer == null) return;

        if (_totalCountPixel == 0)
        {
            _totalCountPixel = _textureInitializer.TotalCount;
            _remainingPixels = _totalCountPixel;
        }

        var fragments = _textureInitializer.GetFragmentsByColor(_colorPrecision.Reduce(color));

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
       
        yield return _queueProcessor.ProcessQueueRoutine(_transformPenHolder.position, _duration, _transitionReducing);

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
            _voiceover.Play(_winn);
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