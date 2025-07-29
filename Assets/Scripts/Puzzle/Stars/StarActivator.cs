using UnityEngine;

public class StarActivator : MonoBehaviour
{
    [SerializeField] private AnimatorPuzzle _animatorPuzzle;
    [SerializeField] private StarsController _controller;

    private void OnEnable()
    {
        _animatorPuzzle.OnAnimationComplete += SetActive;
    }

    private void OnDisable()
    {
        _animatorPuzzle.OnAnimationComplete -= SetActive;
    }

    private void SetActive()
    {
        _controller.SetActive(true);
    }
}