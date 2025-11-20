using System.Collections.Generic;
using UnityEngine;

public static class GridPositionHelper
{
    public static List<Vector2Int> GetAvailableGridPositionsCentered(GridSystem gridSystem)
    {
        List<Vector2Int> availablePositions = new();

        int maxRadius = Mathf.Min(
            gridSystem.GridSizeX / 2 - 1,
            gridSystem.GridSizeY / 2 - 1
        );

        Vector2Int center = GetCenter(gridSystem);

        for (int radius = 0; radius <= maxRadius; radius++)
        {
            for (int x = -radius; x <= radius; x++)
            {
                for (int y = -radius; y <= radius; y++)
                {
                    if (Mathf.Abs(x) != radius && Mathf.Abs(y) != radius) continue;

                    Vector2Int gridPos = center + new Vector2Int(x, y);

                    if (TryAvailablePosition(gridSystem, gridPos, availablePositions))
                    {
                        availablePositions.Add(gridPos);
                    }
                }
            }
        }

        return availablePositions;
    }

    private static bool TryAvailablePosition(GridSystem gridSystem, Vector2Int gridPos, List<Vector2Int> availablePositions)
    {
        bool isNotOnEdge = gridPos.x > 0 && gridPos.x < gridSystem.GridSizeX - 1 &&
                           gridPos.y > 0 && gridPos.y < gridSystem.GridSizeY - 1;

        if (isNotOnEdge &&
            gridSystem.IsValidGridPosition(gridPos) &&
            gridSystem.IsCellEmpty(gridPos) &&
            availablePositions.Contains(gridPos) == false)
        {
            return true;
        }

        return false;
    }

    private static Vector2Int GetCenter(GridSystem gridSystem)
    {
        return new Vector2Int(gridSystem.GridSizeX / 2, gridSystem.GridSizeY / 2);
    }
}