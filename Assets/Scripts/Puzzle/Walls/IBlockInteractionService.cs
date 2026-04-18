using UnityEngine;

public interface IBlockInteractionService
{
    void TryHandle(IColorable colorable, Color color, IUnlockPolicy unlockPolicy);
}