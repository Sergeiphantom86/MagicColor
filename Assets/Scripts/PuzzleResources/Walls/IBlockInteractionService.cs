using PuzzleResources.Walls.WallEngineResource;
using UnityEngine;

namespace PuzzleResources.Walls
{
    public interface IBlockInteractionService
    {
        void ProcessColoredObject(IColorable colorable, Color color, IUnlockPolicy unlockPolicy);
    }
}