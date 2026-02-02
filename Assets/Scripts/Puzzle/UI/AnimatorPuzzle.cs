using System;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class AnimatorPuzzle : MonoBehaviour
{
    [SerializeField] private FireworksController _fireworks;
    [SerializeField] private VictoryPlaque _victoryPlaque;
    [SerializeField] private GameObject _starRatingPanel;
    [SerializeField] private Pen _pen;
    [SerializeField] private RectTransform _penHolder;
    [SerializeField] private Puzzle _puzzle;
    [SerializeField] private BlockSpawner _blockSpawner;

    private Canvas _canvas;
    private Activator _activator;
    private RectTransform _rectTransform;

    public event Action OnAnimationComplete;

    private void Awake()
    {
        _canvas = GetComponent<Canvas>();

        if (_canvas == null) Debug.LogError("Canvas component not found in children!", this);

        if (_pen != null) _activator = _pen.GetComponent<Activator>();

        if (_activator == null) Debug.LogError("Activator component not found on Pen object!", this);

        _rectTransform = _canvas.GetComponent<RectTransform>();

        if(_rectTransform == null) Debug.LogError("Activator component not found on Pen object!", this);
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