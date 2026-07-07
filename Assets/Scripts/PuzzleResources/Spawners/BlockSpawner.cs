using System.Collections;
using System.Collections.Generic;
using PuzzleResources.MovingBlocks;
using PuzzleResources.MovingBlocks.GridLogic;
using PuzzleResources.ObjectPool;
using PuzzleResources.PoolEffects;
using UnityEngine;

namespace PuzzleResources.Spawners
{
    [RequireComponent(typeof(ObjectPooler<Block>))]

    public class BlockSpawner : BaseSpawner<Block>
    {
        [SerializeField] private int _count;
        [SerializeField] private int _index;
        [SerializeField] private GridSystem _gridSystem;
        [SerializeField] private Effecter _effectFalling;

        private GridPositionHelper _gridHelper;
        private WaitForSeconds _timeInterval;
        private WaitForSeconds _waitBeforePuttPlace;
        private System.Func<int> _indexProvider;
        private float _delay;
        private float _transparency;
        private float _delayAppearance;

        public event System.Action<Block> BlockSpawned;

        public event System.Action SpawnerReadyed;

        public List<Block> SpawnedBlocks => SpawnedObjects;

        public int Count => _count;

        protected override void Awake()
        {
            base.Awake();

            _delay = 0.2f;
            _transparency = 1;
            _delayAppearance = 1;
            _waitBeforePuttPlace = new WaitForSeconds(_delayAppearance);
            _timeInterval = new WaitForSeconds(_delay);

            if (_gridSystem == null)
            {
                Debug.LogError("GridSystem not found");
                return;
            }

            _gridHelper = new GridPositionHelper(_gridSystem);
        }

        private void Start()
        {
            SpawnerReadyed?.Invoke();
        }

        public void SetIndexProvider(System.Func<int> provider)
        {
            _indexProvider = provider;
        }

        public void SpawnNecessaryBlocks()
        {
            for (int i = 0; i < _count; i++)
            {
                SpawnBlocks(i);
            }

            StartCoroutine(PutPlace());
        }

        private void SpawnBlocks(int index)
        {
            int finalIndex = _indexProvider != null ? _indexProvider.Invoke() : index;

            ChangeBlockPrefabIndex(finalIndex);
            SpawnSingleBlock();
        }

        private void SpawnSingleBlock()
        {
            Block block = SpawnObject(Vector3.zero, transform, _index);

            if (block == null)
            return;

            Vector2Int? origin = GetRandomAvailableOrigin(block);

            if (origin.HasValue == false)
            {
                Despawn(block);
                return;
            }

            PlaceBlock(block, origin.Value);
        }

        private void ChangeBlockPrefabIndex(int newIndex)
        {
            SetPrefabIndex(newIndex);
            _index = newIndex;
        }

        private Vector2Int? GetRandomAvailableOrigin(Block block)
        {
            List<Vector2Int> availableCenters = _gridHelper.GetAvailableCenters(block.SizeInCells);

            if (availableCenters.Count == 0)
            return null;

            Vector2Int centerCell = availableCenters[Random.Range(0, availableCenters.Count)];
            return _gridSystem.GetOriginFromCenter(centerCell, block.SizeInCells);
        }

        private void PlaceBlock(Block block, Vector2Int origin)
        {
            if (block.TryGetComponent(out ColorableObject anim))
            {
                anim.TurnOffRender();
            }

            Vector3 worldPos = _gridSystem.GetWorldPosition(origin, block.SizeInCells);

            ConfigureBlock(block, origin, worldPos);
            _gridSystem.PlaceObject(origin, block);
        }

        private IEnumerator PutPlace()
        {
            yield return _waitBeforePuttPlace;

            foreach (Block block in SpawnedObjects)
            {
                if (block.TryGetComponent(out SpawnDropAnimation anim))
                {
                    anim.Create(_effectFalling);
                }
            }

            StartCoroutine(PutBackPlace());
        }

        private IEnumerator PutBackPlace()
        {
            foreach (Block block in SpawnedObjects)
            {
                yield return _timeInterval;

                if (block.TryGetComponent(out SpawnDropAnimation anim))
                {
                    if (block.TryGetComponent(out ColorableObject colorableObject))
                    {
                        colorableObject.TurnOnRender();
                        colorableObject.SetAlpha(_transparency);
                    }

                    anim.gameObject.SetActive(true);
                    block.TurnOnCollider();
                }
            }
        }

        private void ConfigureBlock(Block block, Vector2Int origin, Vector3 worldPos)
        {
            block.transform.SetParent(transform);
            block.transform.position = worldPos;
            block.SetGridPosition(origin);

            BlockSpawned?.Invoke(block);
        }
    }
}