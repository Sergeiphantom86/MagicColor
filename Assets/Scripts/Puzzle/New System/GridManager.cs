using UnityEngine;
using System.Collections.Generic;

public class GridManagerXZ : MonoBehaviour
{
    public static GridManagerXZ Instance;

    [Header("Grid Settings")]
    public float gridSize = 1f;
    public int gridWidth = 10;
    public int gridDepth = 10;

    [Header("Visualization")]
    public bool showGrid = true;
    public Color gridColor = Color.gray;
    public GameObject gridCellPrefab;

    private bool[,,] gridOccupancy; // 3D матрица занятости сетки (x, y, z)
    private List<GameObject> gridVisuals = new List<GameObject>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        InitializeGrid();
    }

    void InitializeGrid()
    {
        gridOccupancy = new bool[gridWidth, 1, gridDepth];

        if (showGrid && gridCellPrefab != null)
        {
            CreateGridVisualization();
        }
    }

    void CreateGridVisualization()
    {
        // Удаляем старые визуализации если есть
        foreach (GameObject visual in gridVisuals)
        {
            Destroy(visual);
        }
        gridVisuals.Clear();

        // Создаем новую визуализацию сетки
        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridDepth; z++)
            {
                Vector3 position = GridToWorldPosition(x, z);
                position.y = 0.01f; // Немного выше поверхности чтобы избежать z-fighting

                GameObject cell = Instantiate(gridCellPrefab, position, Quaternion.identity, transform);
                cell.transform.localScale = new Vector3(gridSize, 1, gridSize);
                gridVisuals.Add(cell);
            }
        }
    }

    // Преобразование координат сетки в мировые координаты
    public Vector3 GridToWorldPosition(int x, int z)
    {
        return new Vector3(
            transform.position.x + x * gridSize,
            transform.position.y,
            transform.position.z + z * gridSize
        );
    }

    // Преобразование мировых координат в координаты сетки
    public Vector2Int WorldToGridPosition(Vector3 worldPosition)
    {
        int x = Mathf.FloorToInt((worldPosition.x - transform.position.x) / gridSize);
        int z = Mathf.FloorToInt((worldPosition.z - transform.position.z) / gridSize);

        return new Vector2Int(
            Mathf.Clamp(x, 0, gridWidth - 1),
            Mathf.Clamp(z, 0, gridDepth - 1)
        );
    }

    // Проверка, доступна ли позиция для блока заданного размера
    public bool IsAreaAvailable(int x, int y, int z, int width, int depth)
    {
        if (x < 0 || z < 0 || x + width > gridWidth || z + depth > gridDepth)
            return false;

        for (int i = x; i < x + width; i++)
        {
            for (int j = z; j < z + depth; j++)
            {
                if (gridOccupancy[i, y, j])
                    return false;
            }
        }

        return true;
    }

    // Занимаем область в сетке
    public void OccupyArea(int x, int y, int z, int width, int depth)
    {
        for (int i = x; i < x + width; i++)
        {
            for (int j = z; j < z + depth; j++)
            {
                if (i >= 0 && i < gridWidth && j >= 0 && j < gridDepth)
                {
                    gridOccupancy[i, y, j] = true;
                }
            }
        }
    }

    // Освобождаем область в сетке
    public void ReleaseArea(int x, int y, int z, int width, int depth)
    {
        for (int i = x; i < x + width; i++)
        {
            for (int j = z; j < z + depth; j++)
            {
                if (i >= 0 && i < gridWidth && j >= 0 && j < gridDepth)
                {
                    gridOccupancy[i, y, j] = false;
                }
            }
        }
    }

    // Визуализация сетки в редакторе
    void OnDrawGizmos()
    {
        if (!showGrid) return;

        Gizmos.color = gridColor;

        Vector3 startPos = transform.position;

        // Рисуем линии вдоль оси X
        for (int z = 0; z <= gridDepth; z++)
        {
            Vector3 lineStart = startPos + new Vector3(0, 0, z * gridSize);
            Vector3 lineEnd = lineStart + new Vector3(gridWidth * gridSize, 0, 0);
            Gizmos.DrawLine(lineStart, lineEnd);
        }

        // Рисуем линии вдоль оси Z
        for (int x = 0; x <= gridWidth; x++)
        {
            Vector3 lineStart = startPos + new Vector3(x * gridSize, 0, 0);
            Vector3 lineEnd = lineStart + new Vector3(0, 0, gridDepth * gridSize);
            Gizmos.DrawLine(lineStart, lineEnd);
        }
    }
}