using System;
using UnityEngine;

[RequireComponent(typeof(Grid))]
public class GridSystem : MonoBehaviour
{
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
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        _unityGrid = GetComponent<Grid>();
    }

    public void SetGridSize(int gridSizeX, int gridSizeY)
    {
        _gridSizeX = gridSizeX;
        _gridSizeY = gridSizeY;
        _grid = new GameObject[_gridSizeX, _gridSizeY];
    }

    public Vector3 GridToWorldPosition(Vector2Int gridPosition)
    {
        return _unityGrid.GetCellCenterWorld(
            new Vector3Int(gridPosition.x, gridPosition.y, 0)
        );
    }

    public bool IsValidGridPosition(Vector2Int pos)
    {
        return pos.x >= 0 && pos.y >= 0 &&
               pos.x < _gridSizeX &&
               pos.y < _gridSizeY;
    }

    public Vector2Int GetOriginFromCenter(Vector2Int center, Vector2Int size)
    {
        Vector2Int offset = GetCenterToOriginOffset(size);
        return center - offset;
    }

    private Vector2Int GetCenterToOriginOffset(Vector2Int size)
    {
        return new Vector2Int(
            GetHalfSize(size.x),
            GetHalfSize(size.y)
        );
    }

    private int GetHalfSize(int size)
    {
        return Mathf.FloorToInt((size - 1) / 2f);
    }


    public bool CanPlaceBlock(Vector2Int origin, Vector2Int size)
    {
        return ForEachCell(origin, size, pos =>
            IsValidGridPosition(pos) && _grid[pos.x, pos.y] == null
        );
    }

    public void PlaceBlock(Vector2Int origin, Block block)
    {
        ForEachCell(origin, block.SizeInCells, pos =>
        {
            if (IsValidGridPosition(pos) == false)
                return false;

            _grid[pos.x, pos.y] = block.gameObject;
           
            return true;
        });

        block.SetGridPosition(origin);
    }

    public void ClearBlock(Block block)
    {
        Vector2Int origin = block.GridPosition;

        ForEachCell(origin, block.SizeInCells, pos => 
        {
            if (IsValidGridPosition(pos))
                _grid[pos.x, pos.y] = null;
            return true;
        });
    }

    private bool ForEachCell(Vector2Int origin, Vector2Int size, Func<Vector2Int, bool> check)
    {

        for (int i = 0; i < GetTotalCells(size); i++)
        {
            if (check(GetPosition(origin, size, i)) == false)
                return false;
        }

        return true;
    }

    private int GetTotalCells(Vector2Int size)
    {
        return size.x * size.y;
    }

    private Vector2Int GetPosition(Vector2Int origin, Vector2Int size, int index)
    {
        return origin + new Vector2Int(GetCellX(index, size.x), GetCellY(index, size.y));
    }

    private int GetCellX(int index, int width) => 
        index % width;

    private int GetCellY(int index, int width) => 
        index / width;
}