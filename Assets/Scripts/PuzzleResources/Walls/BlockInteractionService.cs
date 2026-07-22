using UnityEngine;
using PuzzleResources.MinigamesRoulette;
using PuzzleResources.Walls.WallResources;
using PuzzleResources.Walls.WallEngineResources;
using PuzzleResources.ColoringObjects;

namespace PuzzleResources.Walls
{
    public class BlockInteractionService : IBlockInteractionService
    {
        private readonly IUnblocker _wall;
        private readonly IBlockDestroySequence _destroySequence;
        private readonly ILockFeedbackService _lockFeedbackService;

        private ErrorPanel _errorPanel;

        public BlockInteractionService(
            IUnblocker wall,
            IBlockDestroySequence destroySequence,
            ILockFeedbackService lockFeedbackService)
        {
            _wall = wall;
            _destroySequence = destroySequence;
            _lockFeedbackService = lockFeedbackService;

            if (_destroySequence == null)
            {
                Debug.LogError("IBlockDestroySequence == null");
                return;
            }

            if (_wall == null)
            {
                Debug.LogError("Wall == null");
                return;
            }

            if (_lockFeedbackService == null)
            {
                Debug.LogError("ILockFeedbackService == null");
                return;
            }
        }

        public void SetPanelError(ErrorPanel errorPanel)
        {
            if (errorPanel == null)
            {
                Debug.LogError("ErrorPanel == null");
                return;
            }

            _errorPanel = errorPanel;
        }

        public void ProcessColoredObject(IColorModifiable colorable, Color color, IUnlockPolicy unlockPolicy)
        {
            if (color == null)
            {
                Debug.LogError("Color == null");
                return;
            }

            if (colorable == null)
            {
                Debug.LogError("ErrorPanel == null");
                return;
            }

            if (_wall.IsBlocked && unlockPolicy.TryUnlock() == false)
            {
                _errorPanel.TurnOn();
                _lockFeedbackService.Play();

                return;
            }

            if (_wall.IsBlocked)
            {
                unlockPolicy.Use();
            }

            _destroySequence.WaitStart(colorable, color);
        }
    }
}