using Game.SaveEditor;

namespace Menu.TutorialEditor.TutorialPuzzle.TutorialStateMachine
{
    public class InitializationState : TutorialStater
    {
        private readonly IProgressSaver _progressSaver;
        private readonly StateMachine _stateMachine;
        private readonly TutorialContext _context;

        public InitializationState(StateMachine stateMachine, TutorialContext context)
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

            _context.Rotator.Rotated += OnRotated;
        }

        public override void Exit()
        {
            _context.Rotator.Rotated -= OnRotated;
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
}