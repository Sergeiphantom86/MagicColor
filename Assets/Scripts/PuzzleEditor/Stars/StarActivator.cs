using PuzzleEditor.UI;
using UnityEngine;

namespace PuzzleEditor.Stars
{
    public class StarActivator : MonoBehaviour
    {
        [SerializeField] private AnimatorPuzzle _animatorPuzzle;
        [SerializeField] private StarsController _controller;

        private void OnEnable()
        {
            _animatorPuzzle.AnimationComplete += OnSetActive;
        }

        private void OnDisable()
        {
            _animatorPuzzle.AnimationComplete -= OnSetActive;
        }

        private void OnSetActive()
        {
            _controller.SetActive(true);
        }
    }
}