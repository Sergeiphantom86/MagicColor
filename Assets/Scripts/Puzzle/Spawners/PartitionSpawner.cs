using System.Collections.Generic;
using UnityEngine;

public class PartitionSpawner : BaseSpawner<Partition>
{
    [SerializeField] private GridSystem _gridSystem;
    [SerializeField] private int _count;
    [SerializeField] private int _marginFromBorder = 1;
    [SerializeField] private int _chainCount = 3;
    [SerializeField] private int _chainSpacing = 2;
    [SerializeField] private ChainSpawnDirection _chainDirection;

    private PartitionChainSpawner _chainSpawner;
    private GridPositionHelper _gridHelper;
    private ChainSpawnData _chainSpawnData;
    private IProgressSaver _progressSaver;

    protected override void Awake()
    {
        base.Awake();

        if (_gridSystem == null)
            _gridSystem = GridSystem.Instance;

        _gridHelper = new GridPositionHelper(_gridSystem);
        _progressSaver = new ProgressSaver();
        _chainSpawner = new PartitionChainSpawner(_gridSystem);
    }

    private void Start()
    {
        if (_gridSystem == null || _pooler == null)
        {
            Debug.LogError("PartitionSpawner: dependencies missing");
            return;
        }

        if (_progressSaver.Saves.IsUnlockAbilities == false) return;

        SpawnRandom();
    }

    private void SpawnRandom()
    {
        for (int i = 0; i < _chainCount; i++)
        {
            TrySpawnSingle();
        }
    }

    private void TrySpawnSingle()
    {
        Partition partition = SpawnObjectWithCurrentIndex(Vector3.zero, transform);

        if (partition == null)
            return;

        if (TryGetAvailableCenters(partition.SizeInCells, out var centers) == false)
        {
            Despawn(partition);
            return;
        }

        Vector2Int center = GetCentr(centers);
        Vector2Int origin = _gridSystem.GetOriginFromCenter(center, partition.SizeInCells);

        PlacePartition(partition, origin);

        _chainSpawnData = new ChainSpawnData
        {
            StartOrigin = origin,
            Size = partition.SizeInCells,
            Direction = _chainDirection,
            Count = _chainCount,
            Spacing = _chainSpacing
        };

        _chainSpawner.TrySpawnChain(_chainSpawnData, () =>
        SpawnObjectWithCurrentIndex(Vector3.zero, transform), PlacePartition);
    }

    private void ClearCell(IGridOccupant gridOccupant)
    {
        _gridSystem.ClearCell(gridOccupant);

        if (gridOccupant is Partition partition)
        {
            partition.Destroyed -= ClearCell;
        }
    }

    private Vector2Int GetCentr(List<Vector2Int> availableCenters)
    {
        return availableCenters[Random.Range(0, availableCenters.Count)];
    }

    private bool TryGetAvailableCenters(Vector2Int size, out List<Vector2Int> availableCenters)
    {
        availableCenters = _gridHelper.GetAvailableCenters(size, _marginFromBorder);

        return availableCenters != null && availableCenters.Count > 0;
    }


    private void PlacePartition(Partition partition, Vector2Int origin)
    {
        Vector3 worldPos = _gridSystem.GetWorldPosition(origin, partition.SizeInCells);

        ConfigurePartition(partition, origin, worldPos);

        _gridSystem.PlaceObject(origin, partition);
    }

    private void ConfigurePartition(Partition partition, Vector2Int origin, Vector3 worldPos)
    {
        partition.transform.SetParent(transform);
        partition.transform.position = worldPos;
        partition.SetGridPosition(origin);

        partition.Destroyed += ClearCell;
    }
}