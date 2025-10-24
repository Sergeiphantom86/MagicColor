using UnityEngine;

[RequireComponent(typeof(Grid))]
public class GridSystem : MonoBehaviour
{
    [SerializeField] private BlocksContainer _blocksContainer;
    public static GridSystem Instance { get; private set; }

    [SerializeField] private int _gridSizeX;
    [SerializeField] private int _gridSizeY;

    private Grid _unityGrid;
    private GameObject[,] _grid;

    public float CellSize => _unityGrid.cellSize.x;
    public int GridSizeX => _gridSizeX;
    public int GridSizeY => _gridSizeY;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            _unityGrid = GetComponent<Grid>();
        }
        else
        {
            Destroy(gameObject);
        }

        _grid = new GameObject[_gridSizeX, _gridSizeY];
    }

    public Vector2Int WorldToGridPosition(Vector3 worldPosition)
    {
        Vector3Int cellPos = _unityGrid.WorldToCell(worldPosition);
        return new Vector2Int(cellPos.x, cellPos.y);
    }

    public Vector3 GridToWorldPosition(Vector2Int gridPosition)
    {
        return _unityGrid.GetCellCenterWorld(new Vector3Int(gridPosition.x, gridPosition.y, 0));
    }

    public bool IsCellEmpty(Vector2Int gridPosition)
    {
        return IsValidGridPosition(gridPosition) && _grid[gridPosition.x, gridPosition.y] == null;
    }

    public void UpdateCell(Vector2Int gridPosition, GameObject block)
    {
        if (IsValidGridPosition(gridPosition))
        {
            Block gridBlock = block.GetComponent<Block>();

            gridBlock.SetGridPosition(gridPosition);

            _grid[gridPosition.x, gridPosition.y] = block;
        }
    }

    public void ClearCell(Vector2Int gridPosition)
    {
        if (IsValidGridPosition(gridPosition))
            _grid[gridPosition.x, gridPosition.y] = null;
    }

    public bool IsValidGridPosition(Vector2Int gridPosition)
    {
        return gridPosition.x >= 0 &&
               gridPosition.x < _gridSizeX &&
               gridPosition.y >= 0 &&
               gridPosition.y < _gridSizeY;
    }

    private void OnDrawGizmos()
    {
        // Получаем компонент Grid
        Grid grid = GetComponent<Grid>();
        if (grid == null) return;

        // Рассчитываем общий размер сетки
        float totalWidth = _gridSizeX * grid.cellSize.x;
        float totalDepth = _gridSizeY * grid.cellSize.z; // Используем Z для глубины

        // Определяем начальную точку (левый нижний угол сетки в плоскости XZ)
        Vector3 startPos = transform.position;

        // Цвет для отрисовки сетки
        Gizmos.color = Color.cyan;

        // Рисуем линии вдоль оси X
        for (int z = 0; z <= _gridSizeY; z++)
        {
            Vector3 lineStart = startPos + new Vector3(0, 0, z * grid.cellSize.z);
            Vector3 lineEnd = lineStart + new Vector3(totalWidth, 0, 0);
            Gizmos.DrawLine(lineStart, lineEnd);
        }

        // Рисуем линии вдоль оси Z
        for (int x = 0; x <= _gridSizeX; x++)
        {
            Vector3 lineStart = startPos + new Vector3(x * grid.cellSize.x, 0, 0);
            Vector3 lineEnd = lineStart + new Vector3(0, 0, totalDepth);
            Gizmos.DrawLine(lineStart, lineEnd);
        }

        // Дополнительно: рисуем занятые ячейки (только во время игры)
        if (Application.isPlaying && _grid != null)
        {
            Gizmos.color = new Color(1, 0, 0, 1f); // Полупрозрачный красный
            for (int x = 0; x < _gridSizeX; x++)
            {
                for (int z = 0; z < _gridSizeY; z++)
                {
                    if (_grid[x, z] != null)
                    {
                        Vector3 cellCenter = startPos + new Vector3(
                            x * grid.cellSize.x + grid.cellSize.x / 2,
                            0,
                            z * grid.cellSize.z + grid.cellSize.z / 2
                        );
                        Gizmos.DrawCube(cellCenter, new Vector3(grid.cellSize.x, 0.1f, grid.cellSize.z));
                    }
                }
            }
        }
    }

}