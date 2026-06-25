using PuzzleEditor.Walls.WallEngineEditor;
using UnityEngine;
namespace PuzzleEditor.Walls
{

public interface IBlockInteractionService
{
    void TryHandle(IColorable colorable, Color color, IUnlockPolicy unlockPolicy);
}
}