using Fireworks;
using PuzzleEditor.PenEditor;
using UnityEngine;

namespace PuzzleEditor.UI
{
    [RequireComponent(typeof(Canvas))]
    public class AnimatorPuzzle : MonoBehaviour
    {
        [SerializeField]
        private Puzzle1 _puzzle;

        [SerializeField]
        private Activator _activator;

        [SerializeField]
        private VictoryPlaque _victoryPlaque;

        [SerializeField]
        private FireworksController _fireworks;

        public event System.Action OnAnimationComplete;

        private void Awake()
        {
            if (_activator == null)
                Debug.LogError("Activator component not found on Pen object!", this);
        }

        private void OnEnable()
        {
            if (_activator == null)
                return;

            _activator.PuzzleCompleted += LaunchFinal;
            _activator.Approached += Remove;
        }

        private void OnDisable()
        {
            _activator.PuzzleCompleted -= LaunchFinal;
            _activator.Approached -= Remove;
        }

        public void StartGame()
        {
            _puzzle.StartRotation();
        }

        private void Remove(float time)
        {
            _puzzle.Return(time);
        }

        private void LaunchFinal()
        {
            OnAnimationComplete?.Invoke();
            _victoryPlaque.TurnOn();
            _fireworks.Play();
        }
    }
}