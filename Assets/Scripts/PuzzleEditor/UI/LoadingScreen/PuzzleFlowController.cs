using PuzzleEditor.Counter;
using System;
using UnityEngine;
namespace PuzzleEditor.UI.LoadingScreen
{

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
}