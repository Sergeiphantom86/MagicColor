using System;
using PuzzleResources.Counter;
using UnityEngine;

namespace PuzzleResources.UI.LoadingScreen
{
    public class PuzzleFlowController : MonoBehaviour
    {
        [SerializeField] private AnimatorPuzzle _animator;
        [SerializeField] private Timer _timer;

        public event Action PuzzleCompleted;

        private void OnEnable()
        {
            _animator.AnimationCompleted += OnComplete;
        }

        private void OnDisable()
        {
            _animator.AnimationCompleted -= OnComplete;
        }

        private void OnComplete()
        {
            _timer.gameObject.SetActive(false);
            PuzzleCompleted?.Invoke();
        }
    }
}