using UnityEngine;
using UnityEngine.Pool;

public class ObjectPooler<T> : MonoBehaviour where T : Component
{
    [SerializeField] private T _prefab;
    [SerializeField] private bool _collectionCheck = true;
    [SerializeField] private int _defaultPoolSize = 10;
    [SerializeField] private int _maxPoolSize = 50;

    private ObjectPool<T> _pool;

    private void Awake()
    {
        CreatePool();
    }

    public T Get() => _pool.Get();
    public void Release(T item) => _pool.Release(item);

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
    }

    private T CreatePooledItem()
    {
        if (_prefab == null)
        {
            Debug.LogError($"Prefab for {typeof(T).Name} is not assigned in {gameObject.name}");
            return null;
        }

        T item = Instantiate(_prefab, transform);
        item.gameObject.SetActive(false);
        return item;
    }

    private void OnTakeFromPool(T item) => 
        item.gameObject.SetActive(true);

    private void OnReturnedToPool(T item) => 
        item.gameObject.SetActive(false);

    private void OnDestroyPoolObject(T item)
    {
        if (item != null) return;
            Destroy(item.gameObject);
    }

    private void OnDestroy()
    {
        _pool?.Clear();
    }
}