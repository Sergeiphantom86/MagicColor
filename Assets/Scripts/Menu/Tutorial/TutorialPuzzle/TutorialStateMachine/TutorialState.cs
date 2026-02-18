public abstract class TutorialStater : ITutorialState
{
    protected TutorialStateMachine StateMachine;
    protected TutorialContext Context;

    protected TutorialStater(TutorialStateMachine stateMachine, TutorialContext context)
    {
        StateMachine = stateMachine;
        Context = context;
    }

    public abstract void Enter();
    public abstract void Exit();
}