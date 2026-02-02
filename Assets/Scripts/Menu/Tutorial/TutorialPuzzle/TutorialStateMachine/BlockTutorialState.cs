using System.Collections;
using UnityEngine;

public class BlockTutorialState : TutorialState
{
    private const int BLOCK_INDEX = 4;

    public BlockTutorialState(TutorialStateMachine stateMachine, TutorialContext context)
        : base(stateMachine, context) { }

    public override void Enter()
    {
        StateMachine.StartCoroutine(ShowHintsAndContinue());
    }

    public override void Update() { }

    public override void Exit()
    {
        Context.CurrentTouchInput.OnTouchClick -= OnClick;
    }

    private void SetBlock(int index)
    {
        if (Context.Container.SpawnedBlocks == null || index < 0 || index >= Context.Container.SpawnedBlocks.Count)
        {
            Debug.LogError($"Invalid block index: {index}");
            return;
        }

        Block block = Context.Container.SpawnedBlocks[index];


        Context.CurrentBlock = block;

        if (block.TryGetComponent(out ITouchDragInput touchDragInput))
        {
            Context.CurrentTouchInput = touchDragInput;
        }

        if (block.TryGetComponent(out GridDragMovement gridDragMovement))
        {
            Context.GridDragMovement = gridDragMovement;
        }
    }

    private void DisableUnnecessaryComponents()
    {
        Context.Lock.gameObject.SetActive(false);
        Context.Key.gameObject.SetActive(false);
    }

    private void OnClick(Vector2 position)
    {
        
        if (Context.IsAnimationChange == false)
        {
            Context.IsAnimationChange = true;
            StateMachine.ChangeState(new MirageMovementState(StateMachine, Context));
        }
    }

    private IEnumerator ShowHintsAndContinue()
    {
        yield return Context.WaitForSeconds;
        yield return Context.WaitForSeconds;
        yield return Context.WaitForSeconds;
        yield return Context.WaitForSeconds;


        SetBlock(BLOCK_INDEX);

        Context.Visualizer.gameObject.SetActive(true);

        Context.AdjustPositions(
          handPosition: Context.CurrentBlock.transform.position,
          visualizerPosition: Context.CurrentBlock.transform.position,
          yOffset: 0.5f
      );

        Context.HandMover.EnableScaleAnimation();

        DisableUnnecessaryComponents();

        Context.CurrentTouchInput.OnTouchClick += OnClick;
    }
}