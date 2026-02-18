using UnityEngine;

public interface IGridOccupant
{
    Vector2Int SizeInCells { get; }
    void SetGridPosition(Vector2Int origin);
    GameObject GameObject { get; }
    public Vector2Int GridPosition { get; }
}