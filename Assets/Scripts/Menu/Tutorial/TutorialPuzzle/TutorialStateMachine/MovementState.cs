using DG.Tweening;
using System.Collections;
using UnityEngine;

public class MovementState : TutorialStater
{
    private readonly Block _block;
    private readonly ITouchDragInput _input;
    private readonly IProgressSaver _progressSaver;
    private readonly TutorialContext _context;
    private readonly TutorialStateMachine _stateMachine;
    private readonly GridDragMovement _gridDragMovement;
    private readonly float _scaleMultiplier;
    private readonly float _duration;
    private readonly float _yOffset;

    private Coroutine _coroutine;
    private Vector3 _startScale;
    private bool _isMoving;

    public MovementState(TutorialStateMachine stateMachine, TutorialContext context, Block block, GridDragMovement gridDragMovement, ITouchDragInput input)
        : base(stateMachine, context)
    {
        _block = block;
        _input = input;
        _context = context;
        _stateMachine = stateMachine;
        _gridDragMovement = gridDragMovement;

        _startScale = _block.transform.localScale;
        _scaleMultiplier = 1.1f;
        _duration = 0.1f;
        _yOffset = 0.5f;
        _progressSaver = new ProgressSaver();
    }

    public override void Enter()
    {
        if (ValidateReferences() == false) return;

        _context.Visualizer.TurnOff();

        _context.AdjustPositions(
           miragePosition: _context.HandMover.transform.position,
           yOffset: _yOffset
       );

        _context.HandMover.EnableMoveAnimationZ();
        _gridDragMovement.Moved += OnMirageMovement;
        _input.OnTouchClick += StopPulsation;
        _input.OnDropped += StartPulsation;
        _block.OnDestroyed += OnMirageCompleted;
    }

    public override void Exit()
    {
        _gridDragMovement.Moved -= OnMirageMovement;
        _block.OnDestroyed -= OnMirageCompleted;
        _input.OnTouchClick -= StopPulsation;
        _input.OnDropped -= StartPulsation; 
    }

    private void OnMirageMovement()
    {
        _context.Hints.TurnOn(true);
        _context.HandMover.TurnOff();
    }

    private void OnMirageCompleted(Block block)
    {
        _progressSaver.SetTutorialBasics();
        _stateMachine.ChangeState(new CompletionState(_stateMachine, _context));
        StopPulsation(Vector2.zero);
    }

    private void StopPulsation(Vector2 vector2)
    {
        _isMoving = true;

        if (_coroutine != null)
        {
            _stateMachine.StopCurrentCoroutine(_coroutine);
            _block.transform.localScale = _startScale;
        }
    }

    private void StartPulsation()
    {
        _isMoving = false;
        _coroutine = _stateMachine.StartCoroutine(WaitForOneStarLost());
    }

    private IEnumerator WaitForOneStarLost()
    {
        while (_isMoving == false)
        {
            _block.transform.DOScale(_startScale * _scaleMultiplier, _duration);

            yield return _context.WaitForSeconds;
          
            _block.transform.DOScale(_startScale, _duration);

            yield return _context.WaitForSeconds;
        }

        _coroutine = null;
    }

    private bool ValidateReferences()
    {
        bool isValid = true;

        if (_context == null)
        {
            Log(nameof(_context));
            return false;
        }

        if (_context.HandMover == null)
        {
            Log(nameof(_context.HandMover));
            return false;
        }

        if (_gridDragMovement == null)
        {
            Log(nameof(_gridDragMovement));
            return false;
        }

        if (_block == null)
        {
            Log(nameof(_block));
            return false;
        }

        return isValid;
    }

    private void Log(string name)
    {
        Debug.LogError($"{name} is not assigned in");
    }
}