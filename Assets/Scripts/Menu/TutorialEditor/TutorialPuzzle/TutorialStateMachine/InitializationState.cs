public class InitializationState : TutorialStater
{
    private readonly IProgressSaver _progressSaver;
    private readonly TutorialStateMachine _stateMachine;
    private readonly TutorialContext _context;

    public InitializationState(TutorialStateMachine stateMachine, TutorialContext context)
        : base(stateMachine, context)
    {
        _progressSaver = new ProgressSaver();

        _context = context;
        _stateMachine = stateMachine;
    }

    public override void Enter()
    {
        _context.Visualizer.gameObject.SetActive(false);
        _context.Hints.gameObject.SetActive(false);

        _context.Rotator.OnRotated += OnRotated;
    }

    public override void Exit()
    {
        _context.Rotator.OnRotated -= OnRotated;
    }

    private void OnRotated()
    {
        if (_progressSaver.Saves.IsTutorialBasics == false)
        {
            _stateMachine.ChangeState(new UITutorialState(_stateMachine, _context));
        }
        else if (_progressSaver.Saves.IsUnblockingTutorial == false)
        {
            _stateMachine.ChangeState(new KeyTutorialState(_stateMachine, _context));
        }
        else if (_progressSaver.Saves.IsAbilityTutorial == false)
        {
            _stateMachine.ChangeState(_context.TutorialAbilities);
        }
    }
}