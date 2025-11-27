using System.Collections;

public class MirageMovementState : TutorialState
{
    public MirageMovementState(TutorialStateMachine stateMachine, TutorialContext context)
        : base(stateMachine, context) { }

    public override void Enter()
    {
        Context.Visualizer.gameObject.SetActive(false);
        Context.Mirage.gameObject.SetActive(true);

        Context.Mirage.transform.position = Context.HandMover.transform.position;

        Context.AdjustPositions(
           miragePosition: Context.HandMover.transform.position,
           yOffset: 0.5f
       );

        Context.HandMover.EnableMoveAnimationZ();
        Context.Mirage.EnableMoveAnimationZ();

        Context.Mirage.OnMovement += OnMirageMovement;
        Context.Mirage.OnCompleted += OnMirageCompleted;
    }

    public override void Update() { }

    public override void Exit()
    {
        Context.Mirage.OnMovement -= OnMirageMovement;
        Context.Mirage.OnCompleted -= OnMirageCompleted;
    }

    private void OnMirageMovement()
    {
        StateMachine.StartCoroutine(ShowHintsAndContinue());
    }

    private void OnMirageCompleted()
    {
        StateMachine.ChangeState(new CompletionState(StateMachine, Context));
    }

    private IEnumerator ShowHintsAndContinue()
    {
        Context.Hints.TurnOn(true);

        yield return Context.WaitForSeconds;
        yield return Context.WaitForSeconds;
        yield return Context.WaitForSeconds;

        Context.Hints.gameObject.SetActive(false);

        Context.HandMover.EnableMoveAnimationX();
        Context.Mirage.EnableMoveAnimationX();
    }
}