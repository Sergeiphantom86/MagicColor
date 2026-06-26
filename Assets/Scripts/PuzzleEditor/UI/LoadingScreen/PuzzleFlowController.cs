using System;
using PuzzleEditor.Counter;
using UnityEngine;

namespace PuzzleEditor.UI.LoadingScreen
{
    public class PuzzleFlowController : MonoBehaviour
    {
        [SerializeField]
        private AnimatorPuzzle _animator;

        [SerializeField]
        private Timer _timer;

        public event Action PuzzleCompleted;

        private void OnEnable()
        {
            _animator.AnimationComplete += OnComplete;
        }

        private void OnDisable()
        {
            _animator.AnimationComplete -= OnComplete;
        }

        private void OnComplete()
        {
            _timer.gameObject.SetActive(false);
            PuzzleCompleted?.Invoke();
        }
    }
}