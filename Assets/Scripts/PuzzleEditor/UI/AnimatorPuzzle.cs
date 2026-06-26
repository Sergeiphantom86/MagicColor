using Fireworks;
using PuzzleEditor.PenEditor;
using UnityEngine;

namespace PuzzleEditor.UI
{
    [RequireComponent(typeof(Canvas))]

    public class AnimatorPuzzle : MonoBehaviour
    {
        [SerializeField] private Puzzle1 _puzzle;
        [SerializeField] private Activator _activator;
        [SerializeField] private VictoryPlaque _victoryPlaque;
        [SerializeField] private FireworksController _fireworks;

        public event System.Action AnimationComplete;

        private void Awake()
        {
            if (_activator == null)
            Debug.LogError("Activator component not found on Pen object!", this);
        }

        private void OnEnable()
        {
            if (_activator == null)
            return;

            _activator.PuzzleCompleted += OnLaunchFinal;
            _activator.Approached += OnRemove;
        }

        private void OnDisable()
        {
            _activator.PuzzleCompleted -= OnLaunchFinal;
            _activator.Approached -= OnRemove;
        }

        public void StartGame()
        {
            _puzzle.StartRotation();
        }

        private void OnRemove(float time)
        {
            _puzzle.Return(time);
        }

        private void OnLaunchFinal()
        {
            AnimationComplete?.Invoke();
            _victoryPlaque.TurnOn();
            _fireworks.Play();
        }
    }
}