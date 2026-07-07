using UnityEngine;

namespace PuzzleResources.MovingBlocks.GridLogic
{
    public interface IGridOccupant
    {
        public Vector2Int SizeInCells { get; }
        public GameObject GameObject { get; }
        public Vector2Int GridPosition { get; }
        public void SetGridPosition(Vector2Int origin);
    }
}