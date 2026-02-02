using UnityEngine;

public class GridBlock
{
    private Vector2Int _gridPosition;
    private Vector2Int _sizeInCells;

    public Vector2Int SizeInCells => _sizeInCells;
    public Vector2Int GridPosition => _gridPosition;

    public GridBlock(Vector2Int sizeInCells)
    {
        _sizeInCells = sizeInCells;
        _gridPosition = Vector2Int.zero;
    }

    public void SetGridPosition(Vector2Int gridPosition)
    {
        _gridPosition = gridPosition;
    }

    public void ResetState()
    {
        _gridPosition = Vector2Int.zero;
    }
}