using System.Linq;
using PuzzleResources;
using PuzzleResources.MovingBlocks;
using PuzzleResources.MovingBlocks.GridLogic;
using PuzzleResources.Stars;
using UnityEngine;
using YG;

namespace Menu.Tutorials.TutorialPuzzle.TutorialStateMachine
{
    public class BlockTutorialState : TutorialStater
    {
        private const int BlockIndex = 4;

        private readonly StateMachine _stateMachine;
        private readonly StarsCounter _starsCounter;
        private readonly TutorialContext _context;
        private readonly float _yOffset;

        private Block _block;
        private ITouchDragInput _input;
        private GridDragMovement _movement;
        private bool _isAnimationChange;

        public BlockTutorialState(StateMachine stateMachine, TutorialContext context, StarsCounter starsCounter)
        : base(stateMachine, context)
        {
            _stateMachine = stateMachine;
            _context = context;
            _yOffset = 0.5f;
            _starsCounter = starsCounter;

            YG2.saves.IsTutorialBasics = true;
        }

        public override void Enter()
        {
            if (_context == null)
            {
                Debug.LogError("MovementState: context is null");
                return;
            }

            _starsCounter.EnableOneStar();

            ShowHintsAndContinue();
        }

        public override void Exit()
        {
            _input.Touched -= OnClick;
        }

        private void SetBlock(int index)
        {
            if (index < 0 || index >= _context.Container.SpawnedBlocks.Count)
            {
                Debug.LogError($"Invalid block index: {index} or SpawnedBlocks null");
                return;
            }

            _block = GetBlock();

            if (_block == null)
            {
                Debug.LogError("SpawnedBlocks[index] is null");
                return;
            }

            if (_block.TryGetComponent(out ITouchDragInput input) == false)
            {
                Debug.LogError("MovementState: _block missing ITouchDragInput");
                return;
            }

            if (_block.TryGetComponent(out GridDragMovement movement) == false)
            {
                Debug.LogError("MovementState: _block missing GridDragMovement");
                return;
            }

            _input = input;
            _movement = movement;
        }

        private Block GetBlock()
        {
            return _context.Container.SpawnedBlocks.FirstOrDefault(block =>
            {
                if (block.TryGetComponent<IColorable>(out var colorable))
                {
                    return colorable.IsRepainted;
                }

                return false;
            });
        }

        private void DisableUnnecessaryComponents()
        {
            _context.Lock.gameObject.SetActive(false);
            _context.Key.gameObject.SetActive(false);
        }

        private void ShowHintsAndContinue()
        {
            SetBlock(BlockIndex);

            _context.Visualizer.gameObject.SetActive(true);

            _context.AdjustPositions(_block.transform.position, _block.transform.position, yOffset: _yOffset);

            _context.HandMover.EnableScaleAnimation();

            DisableUnnecessaryComponents();

            _input.Touched += OnClick;
        }

        private void OnClick(Vector2 position)
        {
            if (_isAnimationChange == false)
            {
                _isAnimationChange = true;

                _stateMachine.ChangeState(new MovementState(_stateMachine, _context, _block, _movement, _input));
            }
        }
    }
}