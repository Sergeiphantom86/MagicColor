using UnityEngine;

public interface IBlockInteractionService
{
    public bool IsOpen { get; }

    void TryHandle(IColorable colorable, Color color);
}