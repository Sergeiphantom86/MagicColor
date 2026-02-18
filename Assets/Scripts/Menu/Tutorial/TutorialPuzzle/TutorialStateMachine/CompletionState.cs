using System.Collections;

public class CompletionState : TutorialStater
{
    public CompletionState(TutorialStateMachine stateMachine, TutorialContext context)
        : base(stateMachine, context) { }

    public override void Enter()
    {
        StateMachine.StartCoroutine(CompletionRoutine());
    }

    public override void Exit() { }

    private IEnumerator CompletionRoutine()
    {
        yield return Context.WaitFirstStop;

        Context.Hints.TurnOn(false);

        yield return Context.WaitFirstStop;
        yield return Context.WaitFirstStop;

        Context.Hints.TurnOff();
    }
}