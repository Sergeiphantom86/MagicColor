using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BlockPool))]
public class BlockSpawner : MonoBehaviour
{
    [SerializeField] private GridSystem _gridSystem;
    [SerializeField] private int _blocksToSpawn;

    private BlockPool _blockPool;
    private List<Block> _spawnedBlocks;
    private List<Vector2Int> _availablePositions;
    private int _spawnCount;

    public List<Block> SpawnedBlocks => _spawnedBlocks;

    private void Awake()
    {
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

    private void Start()
    {
        SpawnBlocks();
    }

    private void SpawnBlocks()
    {
        _availablePositions = GridPositionHelper.GetAvailableGridPositionsCentered(_gridSystem);

        SetSpawnCount();

        for (int i = 0; i < _spawnCount; i++)
        {
            CreateBlock(_availablePositions[i]);
        }
    }

    private void SetSpawnCount()
    {
        _spawnCount = Mathf.Min(_blocksToSpawn, _availablePositions.Count);
    }

    private void CreateBlock(Vector2Int gridPosition)
    {
        Block block = _blockPool.Pool.Get();

        if (block != null)
        {
            ConfigureBlock(block, gridPosition);

            _spawnedBlocks.Add(block);

            _gridSystem.UpdateCell(gridPosition, block.gameObject);
        }
    }

    private void ConfigureBlock(Block block, Vector2Int gridPosition)
    {
        block.transform.SetParent(transform);
        block.transform.position = _gridSystem.GridToWorldPosition(gridPosition);
        block.transform.eulerAngles = new Vector3(150, 0f, 0f);

        block.SetGridPosition(gridPosition);
    }
}