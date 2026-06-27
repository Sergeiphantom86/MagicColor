namespace Menu.TutorialEditor.TutorialPuzzle.TutorialStateMachine
{
    public abstract class TutorialStater : ITutorialState
    {
        private StateMachine _stateMachine;
        private TutorialContext _context;

        protected TutorialStater(StateMachine stateMachine, TutorialContext context)
        {
            _stateMachine = stateMachine;
            _context = context;
        }

        public abstract void Enter();

        public abstract void Exit();
    }
}