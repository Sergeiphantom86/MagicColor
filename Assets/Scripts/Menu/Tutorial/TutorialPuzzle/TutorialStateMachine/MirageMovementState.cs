using System.Collections;
using UnityEngine;

public class MirageMovementState : TutorialState
{
    public MirageMovementState(TutorialStateMachine stateMachine, TutorialContext context)
        : base(stateMachine, context) { }

    public override void Enter()
    {
        Context.Visualizer.TurnOff();

        Context.AdjustPositions(
           miragePosition: Context.HandMover.transform.position,
           yOffset: 0.5f
       );

        Context.HandMover.EnableMoveAnimationZ();
        Context.GridDragMovement.Moved += OnMirageMovement;
        Context.CurrentBlock.OnDestroyed += OnMirageCompleted;
    }

    public override void Update() { }

    public override void Exit()
    {
        Context.GridDragMovement.Moved -= OnMirageMovement;
        Context.CurrentBlock.OnDestroyed -= OnMirageCompleted;
    }

    private void Stop()
    {
        StateMachine.StartCoroutine(WaitFirst());
    }

    private void OnMirageMovement()
    {
        Context.Hints.TurnOn(true);
        Context.HandMover.TurnOff();
    }

    private void OnMirageCompleted(Block block)
    {
        StateMachine.ChangeState(new CompletionState(StateMachine, Context));
    }

    private IEnumerator ShowHintsAndContinue()
    {
        Context.Hints.TurnOn(true);
        Context.HandMover.TurnOff();

        yield return Context.WaitForSeconds;
        yield return Context.WaitForSeconds;
        yield return Context.WaitForSeconds;

        //Context.Hints.TurnOn(false);
        //Context.Mirage.EnableMoveAnimationX();
    }

    private IEnumerator WaitFirst()
    {
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        
        //Context.Mirage.Stop();
        Context.HandMover.Stop();
    }
}