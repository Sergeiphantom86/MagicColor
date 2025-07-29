using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FragmentQueueProcessor
{
    private readonly Queue<Fragment> _fragmentsQueue = new Queue<Fragment>();
    private readonly IMover _mover;
    private readonly IEffectsHandler _effectsHandler;
    private readonly IBlocksContainer _blocksContainer;
    private readonly IFragmentAnimator _fragmentAnimator;

    Fragment _point;
    Vector3 _startPosition;
    private bool _isProcessing;
    private bool _needSpeedBoost;
    private float _currentDuration;
    private float _transitionReducing;

    public event Action OnFragmentActivated;

    public FragmentQueueProcessor(IEffectsHandler effectsHandler, IMover mover, IFragmentAnimator fragmentAnimator, IBlocksContainer blocksContainer)
    {
        _mover = mover;
        _effectsHandler = effectsHandler;
        _blocksContainer = blocksContainer;
        _fragmentAnimator = fragmentAnimator;

        _blocksContainer.BlockDestroyed += RequestSpeedBoost;
    }

    public void Cleanup()
    {
        if (_blocksContainer != null)
        {
            _blocksContainer.BlockDestroyed -= RequestSpeedBoost;
        }
    }

    public void RequestSpeedBoost()
    {
        if (_isProcessing)
        {
            _needSpeedBoost = true;
        }
    }

    public void EnqueueFragments(IEnumerable<Fragment> fragments)
    {
        foreach (var fragment in fragments)
        {
            if (fragment != null && !_fragmentsQueue.Contains(fragment))
            {
                _fragmentsQueue.Enqueue(fragment);
            }
        }
    }

    public IEnumerator ProcessQueueRoutine(Vector3 startPosition, float initialDuration, float transitionReducing)
    {
        _isProcessing = true;
        _startPosition = startPosition;
        _currentDuration = initialDuration;
        _transitionReducing = transitionReducing;

        while (_fragmentsQueue.Count > 0)
        {
            _point = _fragmentsQueue.Dequeue();

            if (_point == null) continue;

            IncreaseSpeed();

            yield return _mover.MoveToPosition(_point.transform.position, _currentDuration);

            _fragmentAnimator.ActivateFragment(_point);
            _effectsHandler.PlayPixelAppearSound();

            OnFragmentActivated?.Invoke();
        }

        yield return _mover.MoveToPosition(_startPosition, _currentDuration);

        _isProcessing = false;
    }

    private void IncreaseSpeed()
    {
        if (_needSpeedBoost)
        {
            _currentDuration -= _transitionReducing;
            _needSpeedBoost = false;
        }
    }
}