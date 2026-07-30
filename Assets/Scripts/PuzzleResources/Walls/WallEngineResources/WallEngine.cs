using Menu.Tutorials.TutorialPuzzle;
using PuzzleResources.LockMechanics;
using PuzzleResources.PenEditor;
using PuzzleResources.MinigamesRoulette;
using PuzzleResources.Audio;
using UnityEngine;
using Wallets.WalletEconomy;

namespace PuzzleResources.Walls.WallEngineResources
{
    [RequireComponent(typeof(WallLayoutUpdater), typeof(WallMovement), typeof(Voiceover))]
    [RequireComponent(typeof(ColorCollisionHandler), typeof(WallInteractionController), typeof(LockFeedbackService))]
    [RequireComponent(typeof(BlockDestroySequence))]

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
            if (ValidationHelper.AllNotNull(
                this,
                (bag, nameof(bag)),
                (@lock, nameof(@lock)),
                (hintKey, nameof(hintKey)),
                (rotator, nameof(rotator)),
                (colorPrecision, nameof(colorPrecision))) == false)
            {
                return false;
            }

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

        private bool InitSystems(
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
            lockFeedback.InitializeComponents(@lock, hintKey);
            interactionController.Initialize(bagUnlockPolicy, this);
            collisionHandler.Initialize(colorPrecision, hintKey, errorPanel, bagUnlockPolicy);

            _blockDestroySequence.Initialize(activator);
            _blockDestroySequence.IsTouched += OnMove;

            return true;
        }

        private void InitMovement()
        {
            if (_movement == null)
                return;

            _movement.CacheStartPosition();
            _rotation.Rotated += _movement.CacheStartPosition;
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

            return ValidationHelper.AllNotNull(
                this,
                (collisionHandler, nameof(collisionHandler)),
                (interactionController, nameof(interactionController)),
                (_layoutUpdater, nameof(_layoutUpdater)),
                (lockFeedback, nameof(lockFeedback)),
                (blockDestroySequence, nameof(blockDestroySequence)));
        }

        private void OnMove()
        {
            PushMovement();
        }
    }
}