using Menu.TutorialEditor.TutorialPuzzle;
using PuzzleEditor.LockEditor;
using PuzzleEditor.PenEditor;
using PuzzleEditor.RouletteEditor;
using PuzzleEditor.SoundEditor;
using PuzzleEditor.Walls.WallEditor;
using UnityEngine;
using Wallets.WalletEditor;

namespace PuzzleEditor.Walls.WallEngineEditor
{
    [RequireComponent(typeof(Wall), typeof(WallMovement))]

    public class WallEngine : MonoBehaviour, IWallInteractor
    {
        private WallMovement _movement;
        private Rotator _rotation;
        private WallLayoutUpdater _layoutUpdater;
        private BlockDestroySequence _blockDestroySequence;
        private AudioClip _audioClip;
        private Voiceover _voiceover;

        private void Awake()
        {
            _movement = GetComponent<WallMovement>();
            _layoutUpdater = GetComponent<WallLayoutUpdater>();
            _voiceover = GetComponent<Voiceover>();
        }

        private void OnDisable()
        {
            _blockDestroySequence.IsTouched -= OnMove;

            if (_rotation != null)
                _rotation.Rotated -= _movement.CacheStartPosition;
        }

        public bool Initialize(
            IColorPrecision colorPrecision,
            BagKey bag,
            Rotator rotator,
            Messager hintKey,
            Lock @lock,
            ErrorPanel errorPanel,
            Activator activator,
            AudioClip audioClip)
        {
            if (ValidateDependencies(colorPrecision, bag, rotator, hintKey, @lock) == false)
                return false;

            if (ValidateComponents(
                out ColorCollisionHandler collisionHandler,
                out WallInteractionController interactionController,
                out LockFeedbackService lockFeedback,
                out BlockDestroySequence blockDestroySequence) == false)
            {
                return false;
            }

            _rotation = rotator;

            _blockDestroySequence = blockDestroySequence;

            InitMovement();

            InitSystems(
                collisionHandler,
                interactionController,
                lockFeedback, 
                colorPrecision,
                bag,
                hintKey,
                @lock,
                errorPanel,
                activator);

            _audioClip = audioClip;
            return true;
        }

        public void PushMovement()
        {
            _movement.Push();

            if (_voiceover != null && _audioClip != null)
                _voiceover.PlayOneShot(_audioClip);
        }

        private void OnMove()
        {
            PushMovement();
        }

        private void InitSystems(
            ColorCollisionHandler collisionHandler,
            WallInteractionController interactionController,
            LockFeedbackService lockFeedback,
            IColorPrecision colorPrecision,
            BagKey bag, Messager hintKey,
            Lock @lock,
            ErrorPanel errorPanel,
            Activator activator)
        {
            _layoutUpdater.Initialize(_rotation);

            BagUnlockPolicy bagUnlockPolicy = new(bag, 1);
            lockFeedback.InitializComponents(@lock, hintKey);
            interactionController.Initialize(bagUnlockPolicy, this);
            collisionHandler.Initialize(colorPrecision, hintKey, errorPanel, bagUnlockPolicy);

            _blockDestroySequence.Initialize(activator);
            _blockDestroySequence.IsTouched += OnMove;
        }

        private void InitMovement()
        {
            if (_movement == null)
                return;

            _movement.CacheStartPosition();
            _rotation.Rotated += _movement.CacheStartPosition;
        }

        private bool ValidateDependencies(IColorPrecision colorPrecision, BagKey bag, Rotator rotator, Messager hintKey, Lock @lock)
        {
            if (colorPrecision == null)
                return LogNull(nameof(colorPrecision));

            if (bag == null)
                return LogNull(nameof(bag));

            if (rotator == null)
                return LogNull(nameof(rotator));

            if (hintKey == null)
                return LogNull(nameof(hintKey));

            if (@lock == null)
                return LogNull(nameof(@lock));

            return true;
        }

        private bool ValidateComponents(
            out ColorCollisionHandler collisionHandler,
            out WallInteractionController interactionController,
            out LockFeedbackService lockFeedback,
            out BlockDestroySequence blockDestroySequence)
        {
            collisionHandler = GetComponent<ColorCollisionHandler>();
            interactionController = GetComponent<WallInteractionController>();
            lockFeedback = GetComponent<LockFeedbackService>();
            blockDestroySequence = GetComponent<BlockDestroySequence>();

            if (collisionHandler == null)
                return LogNull(nameof(ColorCollisionHandler));

            if (interactionController == null)
                return LogNull(nameof(WallInteractionController));

            if (_layoutUpdater == null)
                return LogNull(nameof(WallLayoutUpdater));

            if (lockFeedback == null)
                return LogNull(nameof(LockFeedbackService));

            if (blockDestroySequence == null)
                return LogNull(nameof(BlockDestroySequence));

            return true;
        }

        private bool LogNull(string dependencyName)
        {
            Debug.LogError($"[{nameof(WallEngine)}] Initialization failed: {dependencyName} missing", this);

            return false;
        }
    }
}