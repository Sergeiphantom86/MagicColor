using System;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class AnimatorPuzzle : MonoBehaviour
{
    [SerializeField] private Puzzle _puzzle;
    [SerializeField] private Activator _activator;
    [SerializeField] private VictoryPlaque _victoryPlaque;
    [SerializeField] private FireworksController _fireworks;

    public event Action OnAnimationComplete;

    private void Awake()
    {
        if (_activator == null) Debug.LogError("Activator component not found on Pen object!", this);
    }

    private void OnEnable()
    {
        if (_activator == null)
            return;

        _activator.OnPuzzleComplete += LaunchFinal;
        _activator.OnApproach += Remove;
    }

    private void OnDisable()
    {
        _activator.OnPuzzleComplete -= LaunchFinal;
        _activator.OnApproach -= Remove;
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