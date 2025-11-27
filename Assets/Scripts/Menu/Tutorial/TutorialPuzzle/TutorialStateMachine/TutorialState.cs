public abstract class TutorialState : ITutorialState
{
    protected TutorialStateMachine StateMachine;
    protected TutorialContext Context;

    protected TutorialState(TutorialStateMachine stateMachine, TutorialContext context)
    {
        StateMachine = stateMachine;
        Context = context;
    }

    public abstract void Enter();
    public abstract void Update();
    public abstract void Exit();
}