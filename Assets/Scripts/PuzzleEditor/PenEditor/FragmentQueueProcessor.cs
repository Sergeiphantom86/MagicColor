using System;
using System.Collections;
using System.Collections.Generic;
using PuzzleEditor.SoundEditor;
using UnityEngine;

namespace PuzzleEditor.PenEditor
{
    public class FragmentQueueProcessor
    {
        private readonly IMover _mover;
        private readonly Voiceover _voiceover;
        private readonly AudioClip _pixelActivation;
        private readonly IBlocksContainer _blocksContainer;
        private readonly IFragmentAnimator _fragmentAnimator;
        private readonly Queue<Fragment> _fragmentsQueue;
        private readonly float _minDuration;

        private bool _isSoundOn;
        private bool _needSpeedBoost;
        private float _currentDuration;
        private float _durationStep;
        private Color _currentColor;
        private Fragment _currentFragment;

        public FragmentQueueProcessor(
        Voiceover voiceover,
        AudioClip pixelActivation,
        IMover mover,
        IFragmentAnimator fragmentAnimator,
        IBlocksContainer blocksContainer)
        {
            _mover = mover;
            _voiceover = voiceover;
            _pixelActivation = pixelActivation;
            _blocksContainer = blocksContainer;
            _fragmentAnimator = fragmentAnimator;
            _isSoundOn = true;
            _minDuration = 0.01f;
            _fragmentsQueue = new();

            if (_blocksContainer != null)
            {
                _blocksContainer.EverythDestroyed += OnRequestSpeedBoost;
            }
        }

        public event Action FragmentActivated;

        public event Action<float> IncreaseSpeed;

        public event Action<Color> ColorHasChanged;

        public void Cleanup()
        {
            if (_blocksContainer != null)
            {
                _blocksContainer.EverythDestroyed -= OnRequestSpeedBoost;
            }
        }

        public void EnqueueFragments(IEnumerable<Fragment> fragments)
        {
            foreach (var fragment in fragments)
            {
                if (fragment != null && _fragmentsQueue.Contains(fragment) == false)
                {
                    _fragmentsQueue.Enqueue(fragment);
                }
            }
        }

        public IEnumerator ProcessQueueRoutine(float initialDuration, float durationStep)
        {
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

                FragmentActivated?.Invoke();
            }
        }

        public void SpeedUpMovement()
        {
            _currentDuration = Mathf.Max(_minDuration, _currentDuration - _durationStep);
        }

        private void OnRequestSpeedBoost()
        {
            _needSpeedBoost = true;
            _isSoundOn = false;
        }

        private void TryRequestSpeedIncrease()
        {
            if (_needSpeedBoost == false)
            return;

            _needSpeedBoost = false;

            IncreaseSpeed?.Invoke(CalculateRemainingTime());
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
                _voiceover.PlayOneShot(_pixelActivation);
            }
        }
    }
}
