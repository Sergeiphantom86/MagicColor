using DG.Tweening;

public class KeyTutorialState : TutorialState
{
    public KeyTutorialState(TutorialStateMachine stateMachine, TutorialContext context)
        : base(stateMachine, context) { }

    public override void Enter()
    {
        Context.HandMover.Pivot.transform.position = Context.Key.transform.position;
        
        Context.StateTutorial.Initialization(Context.HandMover, Context.Visualizer, Context.Key, Context.Lock);

        Context.StateTutorial.OnCompleted += OnTutorialCompleted;
    }

    public override void Update() { }

    public override void Exit()
    {
        Context.StateTutorial.OnCompleted -= OnTutorialCompleted;
        DOTween.Kill(this);
    }

    private void OnTutorialCompleted()
    {
        StateMachine.ChangeState(new CompletionState(StateMachine, Context));
    }
}