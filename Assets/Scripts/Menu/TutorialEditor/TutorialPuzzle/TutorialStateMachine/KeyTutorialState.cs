using DG.Tweening;
using Game.SaveEditor;
using PuzzleEditor.LockEditor;
namespace Menu.TutorialEditor.TutorialPuzzle.TutorialStateMachine
{

public class KeyTutorialState : TutorialStater
{
    private readonly StateMachine _stateMachine;
    private readonly StateTutorial _stateTutorial;
    private readonly TouchVisualizer _visualizer;
    private readonly TutorialContext _context;
    private readonly HandMover _handMover;
    private readonly Lock _lock;
    private readonly Key _key;
    private readonly IProgressSaver _progressSaver;

    public KeyTutorialState(StateMachine stateMachine, TutorialContext context)
        : base(stateMachine, context)
    {
        _context = context;
        _stateMachine = stateMachine;

        _stateTutorial = _context.StateTutorial;
        _visualizer = _context.Visualizer;
        _handMover = _context.HandMover;
        _lock = _context.Lock;
        _key = _context.Key;
        _progressSaver = new ProgressSaver();
        _progressSaver.SetUnblockingTutorial();
    }

    public override void Enter()
    {
        _handMover.Pivot.transform.position = _key.transform.position;

        _stateTutorial.Initialization(_handMover, _visualizer, _key, _lock);

        _stateTutorial.OnCompleted += OnTutorialCompleted;
    }

    public override void Exit()
    {
        _stateTutorial.OnCompleted -= OnTutorialCompleted;
        DOTween.Kill(this);
    }

    private void OnTutorialCompleted()
    {
        _stateMachine.ChangeState(new CompletionState(StateMachine, Context));
    }
}
}