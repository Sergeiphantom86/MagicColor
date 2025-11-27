using System;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class AnimatorPuzzle : MonoBehaviour
{
    [SerializeField] private ParticleSystem _fireworks;
    [SerializeField] private ParticleSystem _completionParticle;
    [SerializeField] private VictoryPlaque _victoryPlaque;
    [SerializeField] private Pen _pen;
    [SerializeField] private RectTransform _penHolder;
    [SerializeField] private Puzzle _puzzle;

    private Canvas _canvas;
    private Activator _activator;
    private RectTransform _rectTransform;

    public event Action PuzzleIsComplete;
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
        _activator.OnPuzzleComplete += LaunchFinal;
    }

    private void OnDisable()
    {
        _activator.OnPuzzleComplete -= LaunchFinal;
    }

    public void StartGame()
    {
        _puzzle.StartRotation();
    }

    private void LaunchFinal()
    {
        _pen.transform.SetParent(_penHolder);
        _puzzle.Return();
        _victoryPlaque.gameObject.SetActive(true);
        OnAnimationComplete?.Invoke();
        TurnOnParticleSystem();

        PuzzleIsComplete?.Invoke();
    }

    private void TurnOnParticleSystem()
    {
        _completionParticle.Play();
        _fireworks.Play();
    }
}