using System.Collections;

public class CompletionState : TutorialState
{
    public CompletionState(TutorialStateMachine stateMachine, TutorialContext context)
        : base(stateMachine, context) { }

    public override void Enter()
    {
        StateMachine.StartCoroutine(CompletionRoutine());
    }

    public override void Update() { }

    public override void Exit() { }

    private IEnumerator CompletionRoutine()
    {
        yield return Context.WaitForSeconds;
        yield return Context.WaitForSeconds;

        Context.Hints.TurnOn(false);

        yield return Context.WaitForSeconds;
        yield return Context.WaitForSeconds;
        yield return Context.WaitForSeconds;

        Context.Hints.TurnOff();
    }

    private void TurnOffVisualDisplay()
    {
        //Context.Mirage.gameObject.SetActive(false);
        Context.HandMover.gameObject.SetActive(false);
    }
}