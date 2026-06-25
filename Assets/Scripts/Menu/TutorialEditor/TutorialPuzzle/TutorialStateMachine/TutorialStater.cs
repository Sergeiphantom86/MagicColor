namespace Menu.TutorialEditor.TutorialPuzzle.TutorialStateMachine
{
public abstract class TutorialStater : ITutorialState
{
    protected StateMachine StateMachine;
    protected TutorialContext Context;

    protected TutorialStater(StateMachine stateMachine, TutorialContext context)
    {
        StateMachine = stateMachine;
        Context = context;
    }

    public abstract void Enter();

    public abstract void Exit();
}
}