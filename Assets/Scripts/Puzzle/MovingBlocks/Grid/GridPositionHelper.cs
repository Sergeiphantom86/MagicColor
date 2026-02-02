using System.Collections.Generic;
using UnityEngine;

public class GridPositionHelper
{
    private readonly GridSystem _gridSystem;

    public GridPositionHelper(GridSystem gridSystem)
    {
        _gridSystem = gridSystem;
    }

    public List<Vector2Int> GetAvailableCenters(Vector2Int blockSize)
    {
        List<Vector2Int> available = new();

        int totalCellsX = _gridSystem.GridSizeX;
        int totalCellsY = _gridSystem.GridSizeY;

        Vector2Int offset = GetCenterOffset(blockSize);

        int minX = offset.x;
        int maxX = totalCellsX - (blockSize.x - offset.x) - 1;
        int minY = offset.y;
        int maxY = totalCellsY - (blockSize.y - offset.y) - 1;

        int total = (maxX - minX + 1) * (maxY - minY + 1);
        for (int i = 0; i < total; i++)
        {
            int x = minX + i % (maxX - minX + 1);
            int y = minY + i / (maxX - minX + 1);

            Vector2Int center = new (x, y);
            Vector2Int origin = _gridSystem.GetOriginFromCenter(center, blockSize);

            if (_gridSystem.CanPlaceBlock(origin, blockSize))
                available.Add(center);
        }

        return available;
    }

    private Vector2Int GetCenterOffset(Vector2Int size)
    {
        return new Vector2Int(Mathf.FloorToInt((size.x - 1) / 2f), Mathf.FloorToInt((size.y - 1) / 2f));
    }
}