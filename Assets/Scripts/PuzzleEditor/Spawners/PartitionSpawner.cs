using System.Collections.Generic;
using PuzzleEditor.MovingBlocks.GridEditor;
using PuzzleEditor.Walls.Partitions;
using UnityEngine;
using YG;

namespace PuzzleEditor.Spawners
{
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

        protected override void Awake()
        {
            base.Awake();
        }

        private void OnEnable()
        {
            _gridSystem.Initialized += OnSpawnRandom;

            if (_gridSystem.IsInitialized)
                OnSpawnRandom();
        }

        private void OnDisable()
        {
            _gridSystem.Initialized -= OnSpawnRandom;
        }

        private void OnSpawnRandom()
        {
            _gridHelper = new GridPositionHelper(_gridSystem);
            _chainSpawner = new PartitionChainSpawner(_gridSystem);

            if (_gridHelper == null)
            {
                Debug.LogError("GridPositionHelper: GridHelper is null");
            }

            if (_chainSpawner == null)
            {
                Debug.LogError("PartitionChainSpawner: GridHelper is null");
            }

            if (_gridSystem == null)
            {
                Debug.LogError("GridSystem: GridHelper is null");
            }

            if (YG2.saves.IsUnlockAbilities == false)
                return;

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
                Spacing = _chainSpacing,
            };

            _chainSpawner.TrySpawnChain(_chainSpawnData, () =>
            SpawnObjectWithCurrentIndex(Vector3.zero, transform), PlacePartition);
        }

        private void OnClearCell(IGridOccupant gridOccupant)
        {
            _gridSystem.ClearCell(gridOccupant);

            if (gridOccupant is Partition partition)
            {
                partition.Destroyed -= OnClearCell;
            }
        }

        private Vector2Int GetCentr(List<Vector2Int> availableCenters)
        {
            return availableCenters[Random.Range(0, availableCenters.Count)];
        }

        private bool TryGetAvailableCenters(Vector2Int size, out List<Vector2Int> availableCenters)
        {
            availableCenters = _gridHelper.GetAvailableOrigins(size, _marginFromBorder);

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

            partition.Destroyed += OnClearCell;
        }
    }
}