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
}