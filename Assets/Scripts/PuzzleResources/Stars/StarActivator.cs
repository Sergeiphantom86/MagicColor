using PuzzleResources.UI;
using UnityEngine;

namespace PuzzleResources.Stars
{
    public class StarActivator : MonoBehaviour
    {
        [SerializeField] private AnimatorPuzzle _animatorPuzzle;
        [SerializeField] private StarsController _controller;

        private void OnEnable()
        {
            _animatorPuzzle.AnimationCompleted += OnSetActive;
        }

        private void OnDisable()
        {
            _animatorPuzzle.AnimationCompleted -= OnSetActive;
        }

        private void OnSetActive()
        {
            _controller.SetActive(true);
        }
    }
}