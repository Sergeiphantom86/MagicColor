using PuzzleEditor.Walls.WallEngineResource;
using UnityEngine;

namespace PuzzleEditor.Walls
{
    public interface IBlockInteractionService
    {
        void ProcessColoredObject(IColorable colorable, Color color, IUnlockPolicy unlockPolicy);
    }
}