using UnityEngine;
using PuzzleResources.MovingBlocks;
using PuzzleResources.Walls.WallEngineResources;
using PuzzleResources.ColoringObjects;

namespace PuzzleResources.Walls
{
    public class CollisionProcessor : ICollisionProcessor
    {
        private readonly IColorMatchService _colorMatch;
        private readonly IBlockInteractionService _blockInteraction;
        private readonly IUnlockPolicy _unlockPolicy;

        public CollisionProcessor(
            IColorMatchService colorMatch,
            IBlockInteractionService blockInteraction,
            IUnlockPolicy unlockPolicy)
        {
            _colorMatch = colorMatch;
            _unlockPolicy = unlockPolicy;
            _blockInteraction = blockInteraction;
        }

        public void ProcessEnter(Collider other)
        {
            if (other.TryGetComponent(out Block block) == false)
                return;

            if (block.TryGetComponent(out ITouchDragInput touchDragInput) == false)
                return;

            if (block.TryGetComponent(out IColorModifiable colorable) == false)
                return;

            if (touchDragInput.IsSelected == false)
                return;

            if (_colorMatch.Match(colorable, out Color color) == false)
                return;

            _blockInteraction.ProcessColoredObject(colorable, color, _unlockPolicy);
        }

        public void ProcessExit(Collider other)
        {
            if (other.TryGetComponent(out Block _) == false)
                return;

            _colorMatch.Reset();
        }
    }
}