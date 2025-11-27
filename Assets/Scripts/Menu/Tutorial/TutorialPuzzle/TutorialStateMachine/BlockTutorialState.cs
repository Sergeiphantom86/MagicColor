using UnityEngine;

public class BlockTutorialState : TutorialState
{
    private const int BLOCK_INDEX = 4;

    public BlockTutorialState(TutorialStateMachine stateMachine, TutorialContext context)
        : base(stateMachine, context) { }

    public override void Enter()
    {
        SetBlock(BLOCK_INDEX);

        Context.Visualizer.gameObject.SetActive(true);

        Context.AdjustPositions(
          handPosition: Context.CurrentBlock.transform.position,
          visualizerPosition: Context.HandMover.transform.position,
          yOffset: 0.5f
      );

        Context.HandMover.EnableScaleAnimation();

        DisableUnnecessaryComponents();

        Context.CurrentTouchInput.OnTouchClick += OnBlockClick;
    }

    public override void Update() { }

    public override void Exit()
    {
        Context.CurrentTouchInput.OnTouchClick -= OnBlockClick;
    }

    private void SetBlock(int index)
    {
        if (Context.Container.SpawnedBlocks == null || index < 0 || index >= Context.Container.SpawnedBlocks.Count)
        {
            Debug.LogError($"Invalid block index: {index}");
            return;
        }

        Context.CurrentBlock = Context.Container.SpawnedBlocks[index];

        Context.CurrentTouchInput = Context.CurrentBlock.GetComponent<TouchDragInput>();
    }

    private void DisableUnnecessaryComponents()
    {
        Context.Lock.gameObject.SetActive(false);
        Context.Key.gameObject.SetActive(false);
    }

    private void OnBlockClick(Vector2 position)
    {
        if (Context.IsAnimationChange == false)
        {
            Context.IsAnimationChange = true;
            StateMachine.ChangeState(new MirageMovementState(StateMachine, Context));
        }
    }
}