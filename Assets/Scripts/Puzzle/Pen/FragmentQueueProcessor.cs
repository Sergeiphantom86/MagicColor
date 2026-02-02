using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragmentQueueProcessor
{
    private readonly Queue<Fragment> _fragmentsQueue = new();
    private readonly IMover _mover;
    private readonly Voiceover _voiceover;
    private readonly AudioClip _pixelActivation;
    private readonly IBlocksContainer _blocksContainer;
    private readonly IFragmentAnimator _fragmentAnimator;

    private Fragment _currentFragment;
    private Vector3 _startPosition;

    private bool _isProcessing;
    private bool _needSpeedBoost;
    private bool _isSoundOn;

    private float _currentDuration;
    private float _durationStep;
    private float _minDuration;

    private Color _currentColor;

    public event Action OnFragmentActivated;
    public event Action<float> OnIncreaseSpeed;
    public event Action<Color> ColorHasChanged;

    public FragmentQueueProcessor(Voiceover voiceover,AudioClip pixelActivation,IMover mover,IFragmentAnimator fragmentAnimator,IBlocksContainer blocksContainer)
    {
        _mover = mover;
        _voiceover = voiceover;
        _pixelActivation = pixelActivation;
        _blocksContainer = blocksContainer;
        _fragmentAnimator = fragmentAnimator;
        _isSoundOn = true;
        _minDuration = 0.01f;

        if (_blocksContainer != null)
        {
            _blocksContainer.BlockDestroyed += RequestSpeedBoost;
        }
    }

    public void Cleanup()
    {
        if (_blocksContainer != null)
        {
            _blocksContainer.BlockDestroyed -= RequestSpeedBoost;
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

    public IEnumerator ProcessQueueRoutine(Vector3 startPosition,float initialDuration,float durationStep)
    {
        _isProcessing = true;
        _startPosition = startPosition;
        _currentDuration = initialDuration;
        _durationStep = durationStep;

        while (_fragmentsQueue.Count > 0)
        {
            _currentFragment = _fragmentsQueue.Dequeue();

            if (_currentFragment == null)
                continue;

            NotifyColorChangeIfNeeded(_currentFragment);

            yield return _mover.MoveToPosition(
                _currentFragment.transform.position,
                _currentDuration
            );

            _fragmentAnimator.ActivateFragment(_currentFragment);
            PlayActivationSound();

            TryRequestSpeedIncrease();

            OnFragmentActivated?.Invoke();
        }

        yield return _mover.MoveToPosition(_startPosition, _currentDuration);

        _isProcessing = false;
    }

    public void SpeedUpMovement()
    {
        _currentDuration = Mathf.Max(
            _minDuration,
            _currentDuration - _durationStep
        );
    }

    private void RequestSpeedBoost()
    {
        if (_isProcessing)
        {
            _needSpeedBoost = true;
            _isSoundOn = false;
        }
    }

    private void TryRequestSpeedIncrease()
    {
        if (_needSpeedBoost == false)
            return;

        _needSpeedBoost = false;

        OnIncreaseSpeed?.Invoke(CalculateRemainingTime());
    }

    private float CalculateRemainingTime()
    {
        return _fragmentsQueue.Count * _currentDuration;
    }

    private void NotifyColorChangeIfNeeded(Fragment fragment)
    {
        var fragmentColor = fragment.GetColor();
        if (_currentColor == fragmentColor)
            return;

        _currentColor = fragmentColor;

        ColorHasChanged?.Invoke(_currentColor);
    }

    private void PlayActivationSound()
    {
        if (_pixelActivation != null && _isSoundOn)
        {
            _voiceover.Play(_pixelActivation);
        }
    }
}