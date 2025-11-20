using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

public class AnimatorPuzzle : MonoBehaviour
{
    [SerializeField] private ParticleSystem _fireworks;
    [SerializeField] private ParticleSystem _completionParticle;

    private Pen _pen;
    private Puzzle _puzzle;
    private Activator _activator;
    private FinalPicture _finalPicture;
    private VictoryPlaque _victoryPlaque;
    private RectTransform _rectTransform;
    private Canvas _canvas;

    public event Action PuzzleIsComplete;
    public event Action OnAnimationComplete;

    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
        if (_canvas == null) Debug.LogError("Canvas component not found in children!", this);

        _pen = GetComponentInChildren<Pen>();
        if (_pen == null) Debug.LogError("Pen component not found in children!", this);

        _puzzle = GetComponentInChildren<Puzzle>();
        if (_puzzle == null) Debug.LogError("Puzzle component not found in children!", this);

        _finalPicture = GetComponentInChildren<FinalPicture>();
        if (_finalPicture == null) Debug.LogError("FinalPicture component not found in children!", this);

        _victoryPlaque = GetComponentInChildren<VictoryPlaque>();
        if (_victoryPlaque == null) Debug.LogError("VictoryPlaque component not found in children!", this);

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
        _pen.Move(_rectTransform);
        _puzzle.StartRotation();
    }

    private void LaunchFinal()
    {
        _puzzle.Return();
        _pen.Return(_rectTransform);
        _finalPicture.Demonstrate(_rectTransform);
        _victoryPlaque.Move(_rectTransform).OnComplete(() => 
        OnAnimationComplete?.Invoke());

        TurnOnParticleSystem();

        PuzzleIsComplete?.Invoke();
    }

    private void TurnOnParticleSystem()
    {
        _completionParticle.Play();
        _fireworks.Play();
    }
}