using UnityEngine;

public class FloorSpawner : BaseSpawner<FloorBlock>
{
    [SerializeField] private Vector2Int _floorSize;
    [SerializeField] private float _yOffset = -0.1f;

    private void Start()
    {
        SpawnFloor();
    }

    private void SpawnFloor()
    {
        GridSystem grid = GridSystem.Instance;

        int fullBlocksX = Mathf.FloorToInt(grid.GridSizeX / (float)_floorSize.x);
        int fullBlocksY = Mathf.FloorToInt(grid.GridSizeY / (float)_floorSize.y);

        int startX = Mathf.FloorToInt((grid.GridSizeX - fullBlocksX * _floorSize.x) / 2f);
        int startY = Mathf.FloorToInt((grid.GridSizeY - fullBlocksY * _floorSize.y) / 2f);

        for (int x = startX; x + _floorSize.x <= grid.GridSizeX; x += _floorSize.x)
        {
            for (int y = startY; y + _floorSize.y <= grid.GridSizeY; y += _floorSize.y)
            {
                Vector2Int origin = new(x, y);

                if (grid.CanPlaceBlock(origin, _floorSize) == false)
                    continue;

                Vector2Int center = origin + _floorSize / 2;

                Vector3 pos = grid.GridToWorldPosition(center);
                pos.y += _yOffset;

                SpawnObject(pos, transform);
            }
        }
    }
}