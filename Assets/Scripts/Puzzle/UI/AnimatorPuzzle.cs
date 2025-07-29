using DG.Tweening;
using System;
using UnityEngine;
using YG;

public class AnimatorPuzzle : MonoBehaviour
{
    [SerializeField] private Pen _pen;
    [SerializeField] private Panel _panel;
    [SerializeField] private Camera _cameraPuzzle;
    [SerializeField] private Activator _activator;
    [SerializeField] private GameObject _victoryPlaque;
    [SerializeField] private ParticleSystem _completionParticle;
    [SerializeField] private ParticleSystem _fireworks;
    [SerializeField] private FinalPicture _finalPicture;
    [SerializeField] private Timer _timer;
    [SerializeField] private float _duration;

    private float _positionImageY;
    private float _scaleImage;
    private float _positionScreenX;
    private float _positionScreenZ;
    private Canvas _canvas;
    private MoverUI _moverUI;
    private Sequence _sequence;

    public event Action PuzzleIsComplete;
    public event Action OnAnimationComplete;

    private void Awake()
    {
        _moverUI = new MoverUI();
        _canvas = GetComponent<Canvas>();
        _cameraPuzzle = Camera.main;
        _positionScreenZ = _canvas.planeDistance;
        _positionImageY = 0.5f;
        _positionScreenX = 0.5f;
        _scaleImage = 1.5f;
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
        float startPositionPanelY = 0.75f;
        float startPositionImageY = 0.7f;
        float startPositionPenX = 0.8f;
        float startPositionPenY = 0.7f;

        _sequence = DOTween.Sequence();

        _moverUI.EnableMotionAnimation(_panel.transform, _duration, _cameraPuzzle, _sequence, _positionScreenX, startPositionPanelY, _positionScreenZ);
        _moverUI.EnableMotionAnimation(_pen.transform, _duration, _cameraPuzzle, _sequence, startPositionPenX, startPositionPenY, _positionScreenZ);
        _moverUI.EnableMotionAnimation(_finalPicture.transform, _duration, _cameraPuzzle, _sequence, _positionScreenX, startPositionImageY, _positionScreenZ);
    }

    private void LaunchFinal()
    {
        float startPositionTimerY = 2;
        float startPositionVictoryPlaqueY = 0.8f;

        _sequence = DOTween.Sequence();

        _moverUI.EnableMotionAnimation(_panel.transform, _duration, _cameraPuzzle, _sequence, _positionScreenX, startPositionTimerY, _positionScreenZ);
        _moverUI.EnableMotionAnimation(_victoryPlaque.transform, _duration, _cameraPuzzle, _sequence, _positionScreenX, startPositionVictoryPlaqueY, _positionScreenZ)
            .OnComplete(() => OnAnimationComplete?.Invoke());
        _moverUI.EnableMotionAnimation(_pen.transform, _duration, _cameraPuzzle, _sequence, startPositionTimerY, startPositionTimerY, _positionScreenZ);
        _moverUI.EnableMotionAnimation(_finalPicture.transform, _duration, _cameraPuzzle, _sequence, _positionScreenX, _positionImageY, _positionScreenZ);

        TurnOnParticleSystem();

        PuzzleIsComplete?.Invoke();

        _moverUI.EnableAnimationResizing(_finalPicture.transform, _duration, _sequence, _scaleImage);
    }

    private void TurnOnParticleSystem()
    {
        _completionParticle.Play();
        _fireworks.Play();
    }
}