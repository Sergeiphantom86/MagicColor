using System;
using UnityEngine;

public class PuzzleFlowController : MonoBehaviour
{
    [SerializeField] private AnimatorPuzzle _animator;
    [SerializeField] private Timer _timer;

    public event Action OnPuzzleCompleted;

    private void OnEnable()
    {
        _animator.OnAnimationComplete += OnComplete;
    }

    private void OnDisable()
    {
        _animator.OnAnimationComplete -= OnComplete;
    }

    private void OnComplete()
    {
        _timer.gameObject.SetActive(false);
        OnPuzzleCompleted?.Invoke();
    }
}