using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace PuzzleEditor.ObjectPool
{
    public class ObjectPooler<T> : MonoBehaviour
    where T : Component
    {
        [SerializeField] private T _prefab;

        [SerializeField]

        private List<T> _fallbackPrefabs;

        [SerializeField] private bool _collectionCheck = true;
        [SerializeField] private int _defaultPoolSize = 10;
        [SerializeField] private int _maxPoolSize = 50;

        private ObjectPool<T> _pool;
        private Dictionary<int, ObjectPool<T>> _prefabPools;
        private Dictionary<T, int> _objectToPrefabIndex;

        private void Awake()
        {
            CreatePool();
        }

        public T GetFromPrefab(int prefabIndex)
        {
            if (_fallbackPrefabs != null && _fallbackPrefabs.Count > 0)
            {
                int safeIndex = Mathf.Clamp(prefabIndex, 0, _fallbackPrefabs.Count - 1);
                return CreateFromSpecificPrefab(safeIndex);
            }

            return _pool.Get();
        }

        public void Release(T item)
        {
            if (
            _objectToPrefabIndex != null
            && _objectToPrefabIndex.TryGetValue(item, out int prefabIndex)
            )
            {
                if (_prefabPools.TryGetValue(prefabIndex, out var specificPool))
                {
                    specificPool.Release(item);
                    _objectToPrefabIndex.Remove(item);
                    return;
                }
            }

            _pool?.Release(item);
        }

        private void CreatePool()
        {
            _pool = new ObjectPool<T>(
            createFunc: CreatePooledItem,
            actionOnGet: OnTakeFromPool,
            actionOnRelease: OnReturnedToPool,
            actionOnDestroy: OnDestroyPoolObject,
            collectionCheck: _collectionCheck,
            defaultCapacity: _defaultPoolSize,
            maxSize: _maxPoolSize
            );

            if (_fallbackPrefabs != null && _fallbackPrefabs.Count > 1)
            {
                _prefabPools = new Dictionary<int, ObjectPool<T>>();
                _objectToPrefabIndex = new Dictionary<T, int>();

                for (int i = 0; i < _fallbackPrefabs.Count; i++)
                {
                    int index = i;
                    var pool = new ObjectPool<T>(
                    createFunc: () => CreateFromSpecificPrefab(index),
                    actionOnGet: OnTakeFromPool,
                    actionOnRelease: OnReturnedToPool,
                    actionOnDestroy: OnDestroyPoolObject,
                    collectionCheck: _collectionCheck,
                    defaultCapacity: _defaultPoolSize,
                    maxSize: _maxPoolSize
                    );
                    _prefabPools[i] = pool;
                }
            }
        }

        private T CreatePooledItem()
        {
            if (_prefab == null && (_fallbackPrefabs == null || _fallbackPrefabs.Count == 0))
            {
                Debug.LogError($"Prefab for {typeof(T).Name} is not assigned in {gameObject.name}");
                return null;
            }

            T prefabToUse = _prefab ?? _fallbackPrefabs[0];
            T item = Instantiate(prefabToUse, transform);
            item.gameObject.SetActive(false);
            return item;
        }

        private T CreateFromSpecificPrefab(int prefabIndex)
        {
            if (_fallbackPrefabs == null || _fallbackPrefabs.Count == 0)
            {
                Debug.LogError($"No fallback prefabs assigned in {gameObject.name}");
                return null;
            }

            int safeIndex = Mathf.Clamp(prefabIndex, 0, _fallbackPrefabs.Count - 1);
            T prefab = _fallbackPrefabs[safeIndex];

            if (prefab == null)
            {
                Debug.LogError($"Prefab at index {safeIndex} is null in {gameObject.name}");
                return null;
            }

            T item = Instantiate(prefab, transform);
            item.gameObject.SetActive(false);

            if (_objectToPrefabIndex != null)
            {
                _objectToPrefabIndex[item] = safeIndex;
            }

            return item;
        }

        private void OnTakeFromPool(T item) => item.gameObject.SetActive(true);

        private void OnReturnedToPool(T item) => item.gameObject.SetActive(false);

        private void OnDestroyPoolObject(T item)
        {
            if (item == null)
            return;

            _objectToPrefabIndex?.Remove(item);

            Destroy(item.gameObject);
        }

        private void OnDestroy()
        {
            _pool?.Clear();

            if (_prefabPools != null)
            {
                foreach (var pool in _prefabPools.Values)
                {
                    pool?.Clear();
                }
            }
        }
    }
}