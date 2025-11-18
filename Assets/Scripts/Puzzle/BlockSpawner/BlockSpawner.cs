using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BlockPool))]
public class BlockSpawner : MonoBehaviour
{
    [SerializeField] private GridSystem _gridSystem;

    private BlockPool _blockPool;
    private List<Block> _spawnedBlocks;
    private int _blocksToSpawn;
    private int _spawnCount;
    List<Vector2Int> _availablePositions;

    public List<Block> SpawnedBlocks => _spawnedBlocks;

    private void Awake()
    {
        _blocksToSpawn = 12;
        _spawnedBlocks = new List<Block>();
        _blockPool = GetComponent<BlockPool>();

        if (_blockPool == null)
        {
            Debug.LogError($"BlockPool не назначен! {this}");
        }

        if (_gridSystem == null)
        {
            Debug.LogError($"GridSystem не найден! {this}");
        }
    }

    public void SpawnBlocks(AudioClip destructionClip,AudioClip draggClip, AudioClip takingClip, AudioClip throwOffClip)
    {
        _availablePositions = GetAvailableGridPositionsCentered();

        SetSpawnCount();

        for (int i = 0; i < _spawnCount; i++)
        {
            CreateBlock(_availablePositions[i], destructionClip, draggClip, takingClip, throwOffClip);
        }
    }

    private void SetSpawnCount()
    {
        _spawnCount = Mathf.Min(_blocksToSpawn, _availablePositions.Count);
    }

    public void CreateBlock(Vector2Int gridPosition, AudioClip destructionClip, AudioClip draggClip, AudioClip takingClip, AudioClip throwOffClip)
    {
        Block block = _blockPool.Pool.Get();

        if (block != null)
        {
            ConfigureBlock(block, gridPosition, destructionClip, draggClip, takingClip, throwOffClip);

            _spawnedBlocks.Add(block);

            _gridSystem.UpdateCell(gridPosition, block.gameObject);
        }
    }

    private void ConfigureBlock(Block block, Vector2Int gridPosition,AudioClip destructionClip, AudioClip draggClip,AudioClip takingClip, AudioClip throwOffClip)
    {
        block.transform.SetParent(transform);
        block.transform.position = _gridSystem.GridToWorldPosition(gridPosition);
        block.transform.eulerAngles = new Vector3(150, 0f, 0f);

        block.Initialize(destructionClip);
        block.SetGridPosition(gridPosition);

        if (block.TryGetComponent(out TouchDragInput touchDragInput))
        {
            touchDragInput.SetAudioClip(draggClip, takingClip, throwOffClip);
        }
    }

    private List<Vector2Int> GetAvailableGridPositionsCentered()
    {
        List<Vector2Int> availablePositions = new List<Vector2Int>();
        Vector2Int center = new Vector2Int(_gridSystem.GridSizeX / 2, _gridSystem.GridSizeY / 2);

        // Собираем позиции по спирали от центра
        for (int radius = 0; radius < Mathf.Max(_gridSystem.GridSizeX, _gridSystem.GridSizeY); radius++)
        {
            for (int x = -radius; x <= radius; x++)
            {
                for (int y = -radius; y <= radius; y++)
                {
                    if (Mathf.Abs(x) != radius && Mathf.Abs(y) != radius) continue;

                    Vector2Int gridPos = center + new Vector2Int(x, y);

                    if (_gridSystem.IsValidGridPosition(gridPos) &&
                        _gridSystem.IsCellEmpty(gridPos) &&
                        !availablePositions.Contains(gridPos))
                    {
                        availablePositions.Add(gridPos);
                    }
                }
            }
        }

        return availablePositions;
    }
}