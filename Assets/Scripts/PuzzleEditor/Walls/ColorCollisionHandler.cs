using System;
using Menu.TutorialEditor;
using Menu.TutorialEditor.TutorialPuzzle;
using PuzzleEditor.MovingBlocks.BlockEditor;
using PuzzleEditor.RouletteEditor;
using PuzzleEditor.Walls.WallEditor;
using PuzzleEditor.Walls.WallEngineEditor;
using UnityEngine;

namespace PuzzleEditor.Walls
{
    [RequireComponent(typeof(Wall), typeof(IColorMatchService), typeof(ILockFeedbackService))]
    [RequireComponent(
        typeof(IColorMatchService),
        typeof(ICollisionHandler),
        typeof(IBlockDestroySequence)
    )]
    public class ColorCollisionHandler : MonoBehaviour
    {
        private IUnblocker _wall;
        private LockInteractionHandler _lockHandler;
        private BlockInteractionService _blockInteraction;
        private ICollisionProcessor _collisionProcessor;
        private IColorMatchService _colorMatch;
        private IBlockDestroySequence _destroySequence;
        private ILockFeedbackService _lockFeedback;
        private ICollisionHandler _collisionHandler;
        private IUnlockPolicy _unlockPolicy;

        public event Action<Block> IsTouched;

        private void Awake()
        {
            _wall = GetComponent<IUnblocker>();
            _colorMatch = GetComponent<ColorMatchService>();
            _lockFeedback = GetComponent<LockFeedbackService>();
            _collisionHandler = GetComponent<CollisionHandler>();
            _destroySequence = GetComponent<BlockDestroySequence>();
            _blockInteraction = new BlockInteractionService(_wall, _destroySequence, _lockFeedback);
            _lockHandler = new LockInteractionHandler();
        }

        private void OnEnable()
        {
            _collisionHandler.OnEnter += Enter;
            _collisionHandler.OnExit += Exit;
            _destroySequence.IsTouched += UnblockWall;
        }

        private void OnDisable()
        {
            _collisionHandler.OnEnter -= Enter;
            _collisionHandler.OnExit -= Exit;
            _destroySequence.IsTouched -= UnblockWall;
        }

        public bool Initialize(
            IColorPrecision colorPrecision,
            Messager hintKey,
            ErrorPanel errorPanel,
            IUnlockPolicy unlockPolicy
        )
        {
            if (Validate(colorPrecision, hintKey, errorPanel, _lockHandler, unlockPolicy) == false)
                return false;

            _lockHandler.SetHint(hintKey);
            _blockInteraction.SetPanelError(errorPanel);
            _colorMatch.Initialize(colorPrecision);
            _unlockPolicy = unlockPolicy;
            _collisionProcessor = new CollisionProcessor(
                _colorMatch,
                _blockInteraction,
                _unlockPolicy
            );

            return true;
        }

        private bool Validate(
            IColorPrecision colorPrecision,
            Messager hintKey,
            ErrorPanel errorPanel,
            LockInteractionHandler lockHandler,
            IUnlockPolicy bagUnlockPolicy
        )
        {
            if (_colorMatch == null)
                return Log(nameof(_colorMatch));

            if (_lockFeedback == null)
                return Log(nameof(_lockFeedback));

            if (_collisionHandler == null)
                return Log(nameof(_collisionHandler));

            if (_destroySequence == null)
                return Log(nameof(_destroySequence));

            if (colorPrecision == null)
                return Log(nameof(colorPrecision));

            if (hintKey == null)
                return Log(nameof(hintKey));

            if (errorPanel == null)
                return Log(nameof(errorPanel));

            if (lockHandler == null)
                return Log(nameof(lockHandler));

            if (bagUnlockPolicy == null)
                return Log(nameof(bagUnlockPolicy));

            return true;
        }

        private bool Log(string name)
        {
            Debug.LogError($"{nameof(ColorCollisionHandler)} missing dependency: {name}", this);
            return false;
        }

        private void Enter(Collider other)
        {
            _collisionProcessor.ProcessEnter(other);

            _lockHandler.Set(other);

            if (other.TryGetComponent(out Block block))
            {
                IsTouched?.Invoke(block);
            }
        }

        private void Exit(Collider other)
        {
            _collisionProcessor.ProcessExit(other);
        }

        public void UnblockWall()
        {
            if (_wall.IsBlocked)
            {
                _unlockPolicy.Use();
                _wall.Unblock();
                _lockHandler.Unblock();
            }
        }
    }
}