using System.Collections;

namespace Menu.TutorialEditor.TutorialPuzzle.TutorialStateMachine
{
    public class CompletionState : TutorialStater
    {
        private readonly TutorialContext _context;
        private readonly StateMachine _stateMachine;

        public CompletionState(StateMachine stateMachine, TutorialContext context)
        : base(stateMachine, context)
        {
            _context = context;
            _stateMachine = stateMachine;
        }

        public override void Enter()
        {
            _stateMachine.StartCoroutine(CompletionRoutine());
        }

        public override void Exit() { }

        private IEnumerator CompletionRoutine()
        {
            yield return _context.WaitFirstStop;

            _context.Hints.TurnOn(false);

            yield return _context.WaitFirstStop;
            yield return _context.WaitFirstStop;

            _context.Hints.TurnOff();
        }
    }
}