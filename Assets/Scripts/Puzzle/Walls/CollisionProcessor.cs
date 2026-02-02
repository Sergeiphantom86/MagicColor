using UnityEngine;

public class CollisionProcessor : ICollisionProcessor
{
    private readonly IColorMatchService _colorMatch;
    private readonly IBlockInteractionService _blockInteraction;

    public CollisionProcessor(IColorMatchService colorMatch, IBlockInteractionService blockInteraction)
    {
        _colorMatch = colorMatch;
        _blockInteraction = blockInteraction;
    }

    public void ProcessEnter(Collider other)
    {
        if (other.TryGetComponent(out Block block) == false)
            return;

        if (block.TryGetComponent(out ITouchDragInput touchDragInput) == false)
            return;

        if (block.TryGetComponent(out IColorable colorable) == false)
            return;

        if (touchDragInput.IsSelected == false) 
            return;

        if (_colorMatch.Match(colorable, out Color color) == false) 
            return;

        _blockInteraction.TryHandle(colorable, color);
    }

    public void ProcessExit(Collider other)
    {
        if (other.TryGetComponent(out Block block) == false)
            return;

        _colorMatch.Reset();
    }
}