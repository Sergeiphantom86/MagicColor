using UnityEngine;
using PuzzleResources.Walls.WallEngineResources;
using PuzzleResources.ColoringObjects;

namespace PuzzleResources.Walls
{
    public interface IBlockInteractionService
    {
        void ProcessColoredObject(IColorModifiable colorable, Color color, IUnlockPolicy unlockPolicy);
    }
}