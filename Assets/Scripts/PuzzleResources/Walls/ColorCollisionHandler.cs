using Menu.Tutorials;
using Menu.Tutorials.TutorialPuzzle;
using PuzzleResources.MinigamesRoulette;
using PuzzleResources.Walls.WallResources;
using PuzzleResources.Walls.WallEngineResources;
using UnityEngine;

namespace PuzzleResources.Walls
{
    [RequireComponent(typeof(Wall), typeof(IColorMatchService), typeof(ILockFeedbackService))]
    [RequireComponent(typeof(IColorMatchService), typeof(ICollisionHandler), typeof(IBlockDestroySequence))]

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
            _collisionHandler.Entered += OnEnter;
            _collisionHandler.Exited += OnExit;
            _destroySequence.IsTouched += UnblockWall;
        }

        private void OnDisable()
        {
            _collisionHandler.Entered -= OnEnter;
            _collisionHandler.Exited -= OnExit;
            _destroySequence.IsTouched -= UnblockWall;
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

        public void Initialize(
            IColorPrecision colorPrecision,
            Messager hintKey,
            ErrorPanel errorPanel,
            IUnlockPolicy unlockPolicy)
        {
            if (ValidationHelper.AllNotNull(
                this, 
                (colorPrecision, nameof(colorPrecision)), 
                (hintKey, nameof(hintKey)), 
                (errorPanel, nameof(errorPanel)), 
                (_lockHandler, nameof(_lockHandler)), 
                (unlockPolicy, nameof(unlockPolicy))) == false)
            {
                return;
            }

            _lockHandler.SetHint(hintKey);
            _blockInteraction.SetPanelError(errorPanel);
            _colorMatch.Initialize(colorPrecision);
            _unlockPolicy = unlockPolicy;

            _collisionProcessor = new CollisionProcessor(_colorMatch, _blockInteraction, _unlockPolicy);
        }

        private void OnEnter(Collider other)
        {
            if (_collisionProcessor == null)
            {
                Debug.LogWarning($"[{name}] Not initialized, collision ignored", this);
                return;
            }

            _collisionProcessor.ProcessEnter(other);
            _lockHandler?.Set(other);
        }

        private void OnExit(Collider other)
        {
            if (_collisionProcessor == null) 
                return;

            _collisionProcessor.ProcessExit(other);
        }
    }
}