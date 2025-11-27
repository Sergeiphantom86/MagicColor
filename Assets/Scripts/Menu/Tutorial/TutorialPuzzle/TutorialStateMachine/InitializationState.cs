using YG;

public class InitializationState : TutorialState
{
    public InitializationState(TutorialStateMachine stateMachine, TutorialContext context)
        : base(stateMachine, context) { }

    public override void Enter()
    {
        Context.Visualizer.gameObject.SetActive(false);
        Context.Hints.gameObject.SetActive(false);

        Context.Rotator.OnRotated += OnRotated;
    }

    public override void Update() { }

    public override void Exit()
    {
        Context.Rotator.OnRotated -= OnRotated;
    }

    private void OnRotated()
    {
        if (YG2.saves.IsTutorial)
        {
            StateMachine.ChangeState(new KeyTutorialState(StateMachine, Context));
        }
        else
        {
            StateMachine.ChangeState(new BlockTutorialState(StateMachine, Context));
        }
    }
}