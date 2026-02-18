using UnityEngine;

public class GridGizmosDrawer : MonoBehaviour
{
    [Header("Gizmos")]
    [SerializeField] private Color _gridColor = Color.green;
    [SerializeField] private Color _occupiedColor = new Color(1f, 0f, 0f, 0.4f);
    [SerializeField] private float _yOffset = 0.01f;

    private Grid _grid;
    private GridSystem _gridSystem;

    private void OnEnable()
    {
        _grid = GetComponent<Grid>();
        _gridSystem = GetComponent<GridSystem>();
    }

    private void OnDrawGizmos()
    {
        if (_grid == null || _gridSystem == null)
            return;

        //DrawGrid();
        DrawOccupiedCells();
    }

    private void DrawGrid()
    {
        Gizmos.color = _gridColor;

        float cellSize = _grid.cellSize.x;

        for (int x = 0; x < _gridSystem.GridSizeX; x++)
        {
            for (int y = 0; y < _gridSystem.GridSizeY; y++)
            {
                Vector3 center = _grid.GetCellCenterWorld(
                    new Vector3Int(x, y, 0)
                );

                Gizmos.DrawWireCube(
                    center + Vector3.up * _yOffset,
                    new Vector3(cellSize, 0.001f, cellSize)
                );
            }
        }
    }

    private void DrawOccupiedCells()
    {
        if (Application.isPlaying == false)
            return;

        Gizmos.color = _occupiedColor;

        float cellSize = _grid.cellSize.x;

        foreach (var occupant in FindObjectsOfType<MonoBehaviour>())
        {
            if (occupant is not IGridOccupant gridOccupant)
                continue;

            Vector2Int origin = gridOccupant.GridPosition;
            Vector2Int size = gridOccupant.SizeInCells;

            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    Vector2Int pos = origin + new Vector2Int(x, y);

                    if (!_gridSystem.IsValidGridPosition(pos))
                        continue;

                    Vector3 center = _grid.GetCellCenterWorld(
                        new Vector3Int(pos.x, pos.y, 0)
                    );

                    Gizmos.DrawCube(
                        center + Vector3.up * _yOffset,
                        new Vector3(cellSize, 0.001f, cellSize)
                    );
                }
            }
        }
    }
}